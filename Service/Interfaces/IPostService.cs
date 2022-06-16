using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.DataTransferObjects;

namespace Service.Interfaces
{
    public interface IPostService
    {
        public Task<PostWithDetailsDto> GetByIdAsync(Guid id);
        public Task<IEnumerable<PostWithDetailsDto>> GetByThreadAsync(Guid threadId);
        public Task<PostWithDetailsDto> CreateAsync(PostCreationDto postDto, Guid userId);
        public Task UpdateAsync(PostUpdateDto postDto);
        public Task DeleteAsync(Guid id);
    }
}