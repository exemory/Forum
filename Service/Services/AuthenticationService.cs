using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Service.DataTransferObjects;
using Service.Exceptions;
using Service.Interfaces;

namespace Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public AuthenticationService(UserManager<User> userManager, IMapper mapper, IConfiguration config)
        {
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
        }

        public async Task<IdentityResult> SignUpAsync(SignUpDto signUpDto)
        {
            var user = _mapper.Map<User>(signUpDto);
            var result = await _userManager.CreateAsync(user, signUpDto.Password);

            if (!result.Succeeded)
            {
                return result;
            }

            result = await _userManager.AddToRoleAsync(user, "User");
            return result;
        }

        public async Task<SessionDto> SignInAsync(SingInDto signInDto)
        {
            var user = await _userManager.FindByNameAsync(signInDto.Login);
            user ??= await _userManager.FindByEmailAsync(signInDto.Login);
            
            if (user == null || !(await _userManager.CheckPasswordAsync(user, signInDto.Password)))
            {
                throw new AuthenticationException();
            }

            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            
            var sessionDto = new SessionDto
            {
                Token = token
            };

            return sessionDto;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_config["Jwt:Secret"]);
            var key = new SymmetricSecurityKey(secret);
            return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        }

        private async Task<List<Claim>> GetClaims(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };
            
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken
            (
                issuer: _config["Jwt:ValidIssuer"],
                audience: _config["Jwt:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow + _config.GetValue<TimeSpan>("Jwt:Lifetime"),
                signingCredentials: signingCredentials
            );
            
            return tokenOptions;
        }
    }
}