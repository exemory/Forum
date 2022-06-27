using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Interfaces
{
    /// <summary>
    /// Repository of <see cref="Post"/> entities
    /// </summary>
    public interface IPostRepository : IRepository<Post>
    {
        /// <summary>
        /// Retrieves post specified by <paramref name="id"/>
        /// </summary>
        /// <param name="id">Guid of the post to be retrieved</param>
        /// <returns>
        /// <see cref="Post"/> including its author, if exists, <c>null</c> otherwise/>
        /// </returns>
        public Task<Post> GetByIdWithDetailsAsync(Guid id);
        
        /// <summary>
        /// Retrieves all posts related to the thread specified by <paramref name="threadId"/>
        /// </summary>
        /// <param name="threadId">Guid of the thread whose posts to be retrieved</param>
        /// <returns>List of posts, including their authors, ordered by publish date by ascending order</returns>
        public Task<IEnumerable<Post>> GetByThreadIdWithDetailsAsync(Guid threadId);
    }
}