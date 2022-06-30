using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <inheritdoc />
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor for initializing a <see cref="UserService"/> class instance
        /// </summary>
        /// <param name="unitOfWork">Unit of work</param>
        /// <param name="userManager">Identity user manager</param>
        /// <param name="mapper">Mapper</param>
        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserWithDetailsDto>> GetAllAsync()
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

        public async Task UpdateRoleAsync(Guid id, UserRoleUpdateDto roleDto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException($"User with id '{id}' not found");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles.Contains("Administrator", StringComparer.OrdinalIgnoreCase))
            {
                throw new ForumException("Impossible to change role of administrator");
            }

            if (userRoles.Contains(roleDto.Role, StringComparer.OrdinalIgnoreCase))
            {
                throw new ForumException($"User '{user.UserName}' already in role '{roleDto.Role}'");
            }

            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRoleAsync(user, roleDto.Role);
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new NotFoundException($"User with id '{id}' not found");
            }

            if (await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                throw new ForumException("Unable to delete administrator");
            }

            await _userManager.DeleteAsync(user);
        }
    }
}