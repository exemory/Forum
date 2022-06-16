using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }
        
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<PostWithDetailsDto>> GetById(Guid id)
        {
            return await _postService.GetByIdAsync(id);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PostWithDetailsDto>>> GetByThread([FromQuery, Required] Guid threadId)
        {
            var result = await _postService.GetByThreadAsync(threadId);
            return result.ToList();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostWithDetailsDto>> Create(PostCreationDto postDto)
        {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _postService.CreateAsync(postDto, userId);
            return CreatedAtAction(nameof(GetById), new {id = result.Id}, result);
        }
        
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<ActionResult> Update(Guid id, PostUpdateDto postDto)
        {
            if (id != postDto.Id)
            {
                return BadRequest();
            }
            
            await _postService.UpdateAsync(postDto);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Moderator,Administrator")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _postService.DeleteAsync(id);
            return NoContent();
        }
    }
}