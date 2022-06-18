using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Data.Entities;
using Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Service.DataTransferObjects;
using Service.Exceptions;
using Service.Interfaces;

namespace Service.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<UserWithDetailsDto>> GetWithDetailsAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var result = new List<UserWithDetailsDto>();

            foreach (var user in users)
            {
                var userDto = _mapper.Map<UserWithDetailsDto>(user);
                userDto.Roles = await _userManager.GetRolesAsync(user);
                result.Add(userDto);
            }
            
            return result;
        }

        public async Task PromoteToModerator(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException();
            }

            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Moderator"))
            {
                throw new ForumException("The user is already a moderator");
            }
            
            if (roles.Contains("Administrator"))
            {
                throw new ForumException("The administrator cannot be a moderator");
            }

            await _userManager.RemoveFromRoleAsync(user, "User");
            await _userManager.AddToRoleAsync(user, "Moderator");
        }

        public async Task DemoteToUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException();
            }
            
            var roles = await _userManager.GetRolesAsync(user);
            
            if (!roles.Contains("Moderator"))
            {
                throw new ForumException("The user is not already a moderator");
            }
            
            await _userManager.RemoveFromRoleAsync(user, "Moderator");
            await _userManager.AddToRoleAsync(user, "User");
        }

        public async Task DeleteAsync(Guid id, Guid requestUserId)
        {
            if (id == requestUserId)
            {
                throw new ForumException("Unable to delete current user");
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException();
            }

            await _userManager.DeleteAsync(user);
        }
    }
}