using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.DataTransferObjects;
using Service.Exceptions;

namespace Service.Interfaces
{
    /// <summary>
    /// Service for thread posts
    /// </summary>
    public interface IPostService
    {
        /// <summary>
        /// Gets a specific post by id
        /// </summary>
        /// <param name="id">Guid of the post to be retrieved</param>
        /// <returns>The post mapped into <see cref="PostWithDetailsDto"/></returns>
        /// <exception cref="NotFoundException">
        /// Thrown when the post with specified <paramref name="id"/> does not exist
        /// </exception>
        public Task<PostWithDetailsDto> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Gets all posts of the thread specified by <paramref name="threadId"/>
        /// </summary>
        /// <param name="threadId">Guid of the thread whose posts are to be retrieved</param>
        /// <returns>The list of posts mapped into <see cref="PostWithDetailsDto"/></returns>
        /// <exception cref="NotFoundException">
        /// Thrown when the thread specified by <paramref name="threadId"/> does not exist
        /// </exception>
        public Task<IEnumerable<PostWithDetailsDto>> GetByThreadAsync(Guid threadId);
        
        /// <summary>
        /// Creates new post
        /// </summary>
        /// <param name="postDto">Post creation data</param>
        /// <returns>Created post mapped into <see cref="PostWithDetailsDto"/></returns>
        /// <exception cref="NotFoundException">
        /// Thrown when the thread specified by <paramref name="postDto.ThreadId"/> does not exist
        /// </exception>
        /// <exception cref="ForumException">
        /// Thrown when:
        /// <list type="bullet">
        /// <item><description>The user who creates the post does not exist</description></item>
        /// <item><description>Thread is closed for posting</description></item>
        /// </list>
        /// </exception>
        public Task<PostWithDetailsDto> CreateAsync(PostCreationDto postDto);

        /// <summary>
        /// Updates the post
        /// </summary>
        /// <param name="id">Guid of the post to be updated</param>
        /// <param name="postDto">Post update data</param>
        /// <exception cref="NotFoundException">
        /// Thrown when the post specified by <paramref name="id"/> does not exist
        /// </exception>
        /// <exception cref="AccessDeniedException">
        /// Thrown when user tries to update someone else's post
        /// </exception>
        /// <remarks>
        /// Moderators and administrators have permission to edit any
        /// posts, while users can only edit their own posts
        /// </remarks>
        public Task UpdateAsync(Guid id, PostUpdateDto postDto);
        
        /// <summary>
        /// Deletes the post
        /// </summary>
        /// <param name="id">Guid of the post to be deleted</param>
        /// <exception cref="NotFoundException">
        /// Thrown when the post with specified <paramref name="id"/> does not exist
        /// </exception>
        /// <exception cref="AccessDeniedException">
        /// Thrown when user tries to delete someone else's post
        /// </exception>
        /// <remarks>
        /// Moderators and administrators have permission to delete any
        /// posts, while users can only delete their own posts
        /// </remarks>
        public Task DeleteAsync(Guid id);
    }
}