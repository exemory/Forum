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
        /// <returns>
        /// Account information mapped into <see cref="UserWithDetailsDto"/>
        /// </returns>
        /// <exception cref="ForumException">
        /// Thrown when the user does not exist
        /// </exception>
        public Task<UserWithDetailsDto> GetInfoAsync();
        
        /// <summary>
        /// Updates user's account
        /// </summary>
        /// <param name="accountDto">Account update data</param>
        /// <exception cref="ForumException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>The user does not exist</description></item>
        /// <item><description>User's current password is incorrect</description></item>
        /// <item><description>Errors occured during updating account</description></item>
        /// </list>
        /// </exception>
        public Task UpdateAsync(AccountUpdateDto accountDto);
        
        /// <summary>
        /// Changes user's password
        /// </summary>
        /// <param name="passwordDto">Password change data</param>
        /// <exception cref="ForumException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>The user does not exist</description></item>
        /// <item><description>Errors occured during changing password</description></item>
        /// </list>
        /// </exception>
        public Task ChangePasswordAsync(PasswordChangeDto passwordDto);
    }
}