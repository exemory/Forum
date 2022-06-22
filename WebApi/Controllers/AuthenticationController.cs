using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DataTransferObjects;
using Service.Interfaces;

namespace WebApi.Controllers
{
    /// <summary>
    /// Authentication controller
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;

        /// <summary>
        /// Constructor for initializing a <see cref="AuthenticationController"/> class instance
        /// </summary>
        /// <param name="authService">Authentication service</param>
        public AuthenticationController(IAuthenticationService authService)
        {
            _authService = authService;
        }
        
        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="signUpDto">Sign up data</param>
        /// <response code="201">New user successfully registered</response>
        /// <response code="400">Registration failed, errors returned</response>
        [HttpPost("sign-up")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> SignUp([FromBody] SignUpDto signUpDto)
        {
            var result = await _authService.SignUpAsync(signUpDto);
            
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                
                return BadRequest(ModelState);
            }

            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Authenticates a user by provided credentials
        /// </summary>
        /// <param name="signInDto">Sign in credentials</param>
        /// <returns>Session information, including access token</returns>
        /// <response code="200">Successfully authenticated</response>
        /// <response code="401">Invalid credentials are provided</response>
        [HttpPost("sign-in")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SessionDto>> SingIn([FromBody] SingInDto signInDto)
        {
            return await _authService.SignInAsync(signInDto);
        }
    }
}