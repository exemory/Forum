using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        public PostRepository(ForumContext context) : base(context)
        {
        }

        public async Task<Post> GetByIdWithDetailsAsync(Guid id)
        {
            return await Set.Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetThreadPostsWithDetailsAsync(Guid threadId)
        {
            return await Set.Include(p => p.Author)
                .Where(p => p.ThreadId == threadId)
                .OrderBy(p => p.PublishDate)
                .ToListAsync();
        }
    }
}