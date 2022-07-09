using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.DataTransferObjects;
using Service.Exceptions;

namespace Service.Interfaces
{
    /// <summary>
    /// Service for application users
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Gets information about user by specified username
        /// </summary>
        /// <param name="username">Username of the user whose information is to be retrieved</param>
        /// <returns>User profile information mapped into <see cref="UserProfileInfoDto"/></returns>
        /// <exception cref="NotFoundException">
        /// Thrown when the user with specified <paramref name="username"/> does not exist
        /// </exception>
        public Task<UserProfileInfoDto> GetInfoByUsernameAsync(string username);

        /// <summary>
        /// Gets all users including their roles
        /// </summary>
        /// <returns>The list of users mapped into <see cref="UserWithDetailsDto"/></returns>
        public Task<IEnumerable<UserWithDetailsDto>> GetAllAsync();

        /// <summary>
        /// Updates the user role
        /// </summary>
        /// <param name="id">Guid of the user whose status to be updated</param>
        /// <param name="roleDto">User role update data</param>
        /// <exception cref="NotFoundException">
        /// Thrown when the user with specified <paramref name="id"/> does not exist
        /// </exception>
        /// <exception cref="ForumException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>Trying to update role of administrator</description></item>
        /// <item><description>The user already in this role</description></item>
        /// </list>
        /// </exception>
        /// <remarks>Unable to update role of user in Administrator role</remarks>
        public Task UpdateRoleAsync(Guid id, UserRoleUpdateDto roleDto);

        /// <summary>
        /// Deletes the user
        /// </summary>
        /// <param name="id">Guid of the user to be deleted</param>
        /// <exception cref="NotFoundException">
        /// Thrown when the user with specified <paramref name="id"/> does not exist
        /// </exception>
        /// <exception cref="ForumException">
        /// Thrown when the user tries to delete administrator
        /// </exception>
        /// <remarks>Unable to delete user in Administrator role</remarks>
        public Task DeleteAsync(Guid id);
    }
}