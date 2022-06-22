using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <inheritdoc />
    public class UserRepository : IUserRepository
    {
        private readonly DbSet<User> _set;

        /// <summary>
        /// Constructor for initializing a <see cref="UserRepository"/> class instance
        /// </summary>
        /// <param name="context">Context of the database</param>
        public UserRepository(ForumContext context)
        {
            _set = context.Users;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _set.AsNoTracking()
                .ToListAsync();
        }
    }
}