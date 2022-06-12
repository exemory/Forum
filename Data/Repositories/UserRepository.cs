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
    }
}