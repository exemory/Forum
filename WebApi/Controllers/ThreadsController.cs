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
    public class ThreadsController : ControllerBase
    {
        private readonly IThreadService _threadService;

        public ThreadsController(IThreadService threadService)
        {
            _threadService = threadService;
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ThreadWithDetailsDto>> GetById(Guid id)
        {
            return await _threadService.GetByIdAsync(id);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ThreadWithDetailsDto>>> GetAll()
        {
            var result = await _threadService.GetAllAsync();
            return result.ToList();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ThreadWithDetailsDto>> Create(ThreadCreationDto threadDto)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _threadService.CreateAsync(threadDto, userId);
            return CreatedAtAction(nameof(GetById), new {id = result.Id}, result);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<ActionResult> Update(Guid id, ThreadUpdateDto threadDto)
        {
            if (id != threadDto.Id)
            {
                return BadRequest();
            }
            
            await _threadService.UpdateAsync(threadDto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _threadService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id:guid}/close")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<ActionResult> Close(Guid id)
        {
            await _threadService.CloseAsync(id);
            return NoContent();
        }
        
        [HttpPut("{id:guid}/open")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<ActionResult> Open(Guid id)
        {
            await _threadService.OpenAsync(id);
            return NoContent();
        }
    }
}