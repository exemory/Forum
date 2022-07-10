using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.DataTransferObjects;
using Service.Interfaces;

namespace WebApi.Controllers
{
    /// <summary>
    /// Posts controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        /// <summary>
        /// Constructor for initializing a <see cref="PostsController"/> class instance
        /// </summary>
        /// <param name="postService">Post service</param>
        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        /// <summary>
        /// Gets a specific post by id
        /// </summary>
        /// <param name="id">Guid of the post to be retrieved</param>
        /// <returns>The post specified by <paramref name="id"/></returns>
        /// <response code="200">Returns the post specified by <paramref name="id"/></response>
        /// <response code="404">Post with specified <paramref name="id"/> not found</response>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostWithDetailsDto>> GetById(Guid id)
        {
            return await _postService.GetByIdAsync(id);
        }

        /// <summary>
        /// Gets all posts of the thread specified by <paramref name="threadId"/>
        /// </summary>
        /// <param name="threadId">Guid of the thread whose posts are to be retrieved</param>
        /// <returns>Array of posts related to the thread</returns>
        /// <response code="200">Returns the array of posts</response>
        /// <response code="404">Thread with specified <paramref name="threadId"/> not found</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<PostWithDetailsDto>>> GetByThread(
            [FromQuery, Required] Guid threadId)
        {
            return Ok(await _postService.GetByThreadAsync(threadId));
        }

        /// <summary>
        /// Creates new post
        /// </summary>
        /// <param name="postDto">Post creation data</param>
        /// <returns>Newly created post</returns>
        /// <response code="201">Returns the newly created post</response>
        /// <response code="400">Thread specified by <paramref name="postDto.ThreadId"/> is closed for posting</response>
        /// <response code="404">Thread specified by <paramref name="postDto.ThreadId"/> not found</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostWithDetailsDto>> Create(PostCreationDto postDto)
        {
            var result = await _postService.CreateAsync(postDto);
            return CreatedAtAction(nameof(GetById), new {id = result.Id}, result);
        }

        /// <summary>
        /// Updates the post
        /// </summary>
        /// <param name="id">Guid of the post to be updated</param>
        /// <param name="postDto">Post update data</param>
        /// <remarks>
        /// Moderators and administrators have permission to edit any
        /// posts, while users can only edit their own posts
        /// </remarks>
        /// <response code="204">Post has been updated</response>
        /// <response code="403">User tries to update someone else's post</response>
        /// <response code="404">Post specified by <paramref name="id"/> not found</response>
        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(Guid id, PostUpdateDto postDto)
        {
            await _postService.UpdateAsync(id, postDto);
            return NoContent();
        }

        /// <summary>
        /// Deletes the post
        /// </summary>
        /// <param name="id">Guid of the post to be deleted</param>
        /// <remarks>
        /// Moderators and administrators have permission to delete any
        /// posts, while users can only delete their own posts
        /// </remarks>
        /// <response code="204">Post has been deleted</response>
        /// <response code="403">User tries to delete someone else's post</response>
        /// <response code="404">Post specified by <paramref name="id"/> not found</response>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _postService.DeleteAsync(id);
            return NoContent();
        }
    }
}