using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Interfaces
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> GetAllAsync();
    }
}