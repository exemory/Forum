using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Interfaces
{
    public interface IPostRepository : IRepository<Post>
    {
        public Task<Post> GetByIdWithDetailsAsync(Guid id);
        public Task<IEnumerable<Post>> GetThreadPostsWithDetailsAsync(Guid threadId);
    }
}