using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class ThreadRepository : Repository<Thread>, IThreadRepository
    {
        public ThreadRepository(ForumContext context) : base(context)
        {
        }

        public async Task<Thread> GetByIdWithDetailsAsync(Guid threadId)
        {
            return await Set.Include(t => t.Author)
                .FirstOrDefaultAsync(t => t.Id == threadId);
        }

        public async Task<IEnumerable<Thread>> GetAllWithDetailsAsync()
        {
            return await Set.Include(t => t.Author)
                .OrderByDescending(t => t.CreationDate)
                .ToListAsync();
        }
    }
}