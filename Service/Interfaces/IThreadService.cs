using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.DataTransferObjects;

namespace Service.Interfaces
{
    public interface IThreadService
    {
        public Task<ThreadWithDetailsDto> GetByIdAsync(Guid id);
        public Task<IEnumerable<ThreadWithDetailsDto>> GetAllAsync();
        public Task<ThreadWithDetailsDto> CreateAsync(ThreadCreationDto threadDto, Guid userId);
        public Task UpdateAsync(ThreadUpdateDto threadDto);
        public Task DeleteAsync(Guid id);
        public Task CloseAsync(Guid id);
        public Task OpenAsync(Guid id);
    }
}