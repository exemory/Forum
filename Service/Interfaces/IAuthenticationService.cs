using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Service.DataTransferObjects;

namespace Service.Interfaces
{
    /// <summary>
    /// Service for authentication and registration
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Registers new user
        /// </summary>
        /// <param name="signUpDto">Registration data</param>
        /// <returns>Result of registration</returns>
        public Task<IdentityResult> SignUpAsync(SignUpDto signUpDto);

        /// <summary>
        /// Logins user by provided credentials
        /// </summary>
        /// <param name="signInDto">Credentials</param>
        /// <returns>
        /// Session information, including access token
        /// </returns>
        public Task<SessionDto> SignInAsync(SignInDto signInDto);
    }
}