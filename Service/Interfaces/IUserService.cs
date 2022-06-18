using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.DataTransferObjects;

namespace Service.Interfaces
{
    public interface IUserService
    {
        public Task<IEnumerable<UserWithDetailsDto>> GetWithDetailsAsync();
        public Task PromoteToModerator(Guid id);
        public Task DemoteToUser(Guid id);
        public Task DeleteAsync(Guid id, Guid requestUserId);
    }
}