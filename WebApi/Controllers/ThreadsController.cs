using System;
using System.Collections.Generic;
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
    /// Threads controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ThreadsController : ControllerBase
    {
        private readonly IThreadService _threadService;

        /// <summary>
        /// Constructor for initializing a <see cref="ThreadsController"/> class instance
        /// </summary>
        /// <param name="threadService">Thread service</param>
        public ThreadsController(IThreadService threadService)
        {
            _threadService = threadService;
        }

        /// <summary>
        /// Gets a specific thread by id
        /// </summary>
        /// <param name="id">Guid of the thread to be retrieved</param>
        /// <returns>The thread specified by <paramref name="id"/></returns>
        /// <response code="200">Returns the thread specified by <paramref name="id"/></response>
        /// <response code="404">Thread with specified <paramref name="id"/> not found</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<ActionResult<ThreadWithDetailsDto>> GetById(Guid id)
        {
            return await _threadService.GetByIdAsync(id);
        }

        /// <summary>
        /// Gets all threads
        /// </summary>
        /// <returns>Array of threads</returns>
        /// <response code="200">Returns the array of threads</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ThreadWithDetailsDto>>> GetAll()
        {
            return Ok(await _threadService.GetAllAsync());
        }

        /// <summary>
        /// Creates new thread
        /// </summary>
        /// <param name="threadDto">Thread creation data</param>
        /// <returns>Newly created thread</returns>
        /// <response code="201">Returns the newly created thread</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<ThreadWithDetailsDto>> Create(ThreadCreationDto threadDto)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _threadService.CreateAsync(threadDto, userId);
            return CreatedAtAction(nameof(GetById), new {id = result.Id}, result);
        }

        /// <summary>
        /// Updates the thread
        /// </summary>
        /// <param name="id">Guid of the thread to be updated</param>
        /// <param name="threadDto">Thread update data</param>
        /// <response code="204">Thread has been updated</response>
        /// <response code="404">Thread specified by <paramref name="id"/> not found</response>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Moderator,Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(Guid id, ThreadUpdateDto threadDto)
        {
            await _threadService.UpdateAsync(id, threadDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes the thread
        /// </summary>
        /// <param name="id">Guid of the thread to be deleted</param>
        /// <response code="204">Thread has been deleted</response>
        /// <response code="404">Thread specified by <paramref name="id"/> not found</response>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Moderator,Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _threadService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Updates the thread status
        /// </summary>
        /// <param name="id">Guid of the thread whose status to be updated</param>
        /// <param name="statusDto">Thread status update data</param>
        /// <response code="204">Thread status has been updated</response>
        /// <response code="404">Thread specified by <paramref name="id"/> not found</response>
        [HttpPut("{id:guid}/status")]
        [Authorize(Roles = "Moderator,Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] ThreadStatusUpdateDto statusDto)
        {
            await _threadService.UpdateStatusAsync(id, statusDto);
            return NoContent();
        }
    }
}