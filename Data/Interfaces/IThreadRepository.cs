using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Interfaces
{
    public interface IThreadRepository : IRepository<Thread>
    {
        public Task<Thread> GetByIdWithDetailsAsync(Guid threadId);
        public Task<IEnumerable<Thread>> GetAllWithDetailsAsync();
    }
}