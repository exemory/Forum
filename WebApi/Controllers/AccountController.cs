using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DataTransferObjects;
using Service.Interfaces;

namespace WebApi.Controllers
{
    /// <summary>
    /// Account controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        /// <summary>
        /// Constructor for initializing a <see cref="AccountController"/> class instance
        /// </summary>
        /// <param name="accountService">Account service</param>
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Gets information about current user account
        /// </summary>
        /// <returns>Information about current user account</returns>
        /// <response code="200">Returns information about current user account</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserWithDetailsDto>> GetInfo()
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            return await _accountService.GetInfoAsync(userId);
        }

        /// <summary>
        /// Updates current user account
        /// </summary>
        /// <param name="accountDto">Account update data</param>
        /// <response code="204">Account has been updated</response>
        /// <response code="400">Errors occured during updating account</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update(AccountUpdateDto accountDto)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _accountService.UpdateAsync(userId, accountDto);
            return NoContent();
        }

        /// <summary>
        /// Changes current user password
        /// </summary>
        /// <param name="passwordDto">Password change data</param>
        /// <response code="204">Password has been changed</response>
        /// <response code="400">Errors occured during changing password</response>
        [HttpPut("password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ChangePassword(PasswordChangeDto passwordDto)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _accountService.ChangePasswordAsync(userId, passwordDto);
            return NoContent();
        }
    }
}