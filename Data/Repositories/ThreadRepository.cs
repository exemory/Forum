using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <inheritdoc cref="IThreadRepository" />
    public class ThreadRepository : Repository<Thread>, IThreadRepository
    {
        /// <summary>
        /// Constructor for initializing a <see cref="ThreadRepository"/> class instance
        /// </summary>
        /// <param name="context">Context of the database</param>
        public ThreadRepository(ForumContext context) : base(context)
        {
        }

        public async Task<Thread> GetByIdWithDetailsAsync(Guid id)
        {
            return await Set.AsNoTracking()
                .Include(t => t.Author)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Thread>> GetAllWithDetailsAsync()
        {
            return await Set
                .AsNoTracking()
                .Include(t => t.Author)
                .OrderByDescending(t => t.CreationDate)
                .ToListAsync();
        }
    }
}