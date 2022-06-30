using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Service.DataTransferObjects;
using Service.Exceptions;

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
        /// <exception cref="RegistrationException">
        /// Thrown when registration failed
        /// </exception>
        public Task SignUpAsync(SignUpDto signUpDto);

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