using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DbSet<User> _set;

        public UserRepository(ForumContext context)
        {
            _set = context.Users;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _set.ToListAsync();
        }
    }
}