using System;
using System.Threading.Tasks;
using Service.DataTransferObjects;
using Service.Exceptions;

namespace Service.Interfaces
{
    /// <summary>
    /// Service for account management
    /// </summary>
    public interface IAccountService
    {
        /// <summary>
        /// Gets account information of specified user
        /// </summary>
        /// <param name="userId">
        /// Guid of the user whose account information is to be retrieved
        /// </param>
        /// <returns>
        /// Account information mapped into <see cref="UserWithDetailsDto"/>
        /// </returns>
        /// <exception cref="ForumException">
        /// Thrown when the user specified by <paramref name="userId"/> does not exist
        /// </exception>
        public Task<UserWithDetailsDto> GetInfoAsync(Guid userId);
        
        /// <summary>
        /// Updates user's account
        /// </summary>
        /// <param name="userId">Guid of the user whose account is to be updated</param>
        /// <param name="accountDto">Account update data</param>
        /// <exception cref="ForumException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>The user specified by <paramref name="userId"/> does not exist</description></item>
        /// <item><description>User's current password is incorrect</description></item>
        /// <item><description>Errors occured during updating account</description></item>
        /// </list>
        /// </exception>
        public Task UpdateAsync(Guid userId, AccountUpdateDto accountDto);
        
        /// <summary>
        /// Changes user's password
        /// </summary>
        /// <param name="userId">Guid of the user whose password is to be changed</param>
        /// <param name="passwordDto">Password change data</param>
        /// <exception cref="ForumException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>The user specified by <paramref name="userId"/> does not exist</description></item>
        /// <item><description>Errors occured during changing password</description></item>
        /// </list>
        /// </exception>
        public Task ChangePasswordAsync(Guid userId, PasswordChangeDto passwordDto);
    }
}