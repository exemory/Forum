using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Service.DataTransferObjects;
using Service.Exceptions;
using Service.Interfaces;

namespace Service.Services
{
    /// <inheritdoc />
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for initializing a <see cref="AccountService"/> class instance
        /// </summary>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="mapper">Mapper</param>
        public AccountService(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<UserWithDetailsDto> GetInfoAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ForumException($"User with id {userId} does not exist");
            }

            var result = _mapper.Map<UserWithDetailsDto>(user);
            result.Roles = await _userManager.GetRolesAsync(user);

            return result;
        }

        public async Task UpdateAsync(Guid userId, AccountUpdateDto accountDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ForumException($"User with id {userId} does not exist");
            }

            if (!await _userManager.CheckPasswordAsync(user, accountDto.CurrentPassword))
            {
                throw new ForumException($"Incorrect password");
            }

            _mapper.Map(accountDto, user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var sb = new StringBuilder();
                sb.AppendJoin(' ', result.Errors.Select(e => e.Description));

                throw new ForumException(sb.ToString());
            }
        }

        public async Task ChangePasswordAsync(Guid userId, PasswordChangeDto passwordDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new ForumException($"User with id {userId} does not exist");
            }

            var result = await _userManager.ChangePasswordAsync(user, passwordDto.CurrentPassword, passwordDto.NewPassword);

            if (!result.Succeeded)
            {
                var sb = new StringBuilder();
                sb.AppendJoin(' ', result.Errors.Select(e => e.Description));

                throw new ForumException(sb.ToString());
            }
        }
    }
}