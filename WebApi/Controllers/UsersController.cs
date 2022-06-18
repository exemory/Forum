using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.DataTransferObjects;
using Service.Interfaces;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserWithDetailsDto>>> GetAll()
        {
            var result = await _userService.GetWithDetailsAsync();
            return result.ToList();
        }

        [HttpPut("{id:guid}/promote-to-moderator")]
        public async Task<ActionResult> PromoteToModerator(Guid id)
        {
            await _userService.PromoteToModerator(id);
            return NoContent();
        }
        
        [HttpPut("{id:guid}/demote-to-user")]
        public async Task<ActionResult> DemoteToUser(Guid id)
        {
            await _userService.DemoteToUser(id);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var requestUserId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await _userService.DeleteAsync(id, requestUserId);
            return NoContent();
        }
    }
}