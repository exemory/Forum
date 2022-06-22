using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <inheritdoc cref="IPostRepository" />
    public class PostRepository : Repository<Post>, IPostRepository
    {
        /// <summary>
        /// Constructor for initializing a <see cref="PostRepository"/> class instance
        /// </summary>
        /// <param name="context">Context of the database</param>
        public PostRepository(ForumContext context) : base(context)
        {
        }

        public async Task<Post> GetByIdWithDetailsAsync(Guid id)
        {
            return await Set.AsNoTracking()
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetThreadPostsWithDetailsAsync(Guid threadId)
        {
            return await Set.AsNoTracking()
                .Include(p => p.Author)
                .Where(p => p.ThreadId == threadId)
                .OrderBy(p => p.PublishDate)
                .ToListAsync();
        }
    }
}