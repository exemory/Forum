using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Service.DataTransferObjects;
using Service.Exceptions;

namespace Service.Interfaces
{
    /// <summary>
    /// Service for threads
    /// </summary>
    public interface IThreadService
    {
        /// <summary>
        /// Gets a specific thread by id
        /// </summary>
        /// <param name="id">Guid of the thread to be retrieved</param>
        /// <returns>The thread mapped into <see cref="PostWithDetailsDto"/></returns>
        /// <exception cref="NotFoundException">
        /// Thrown when the thread with specified <paramref name="id"/> does not exist
        /// </exception>
        public Task<ThreadWithDetailsDto> GetByIdAsync(Guid id);
        
        /// <summary>
        /// Gets all threads
        /// </summary>
        /// <returns>The list of threads mapped into <see cref="ThreadWithDetailsDto"/></returns>
        public Task<IEnumerable<ThreadWithDetailsDto>> GetAllAsync();
        
        /// <summary>
        /// Creates new thread
        /// </summary>
        /// <param name="threadDto">Thread creation data</param>
        /// <param name="authorId">Author guid of the new thread</param>
        /// <returns>Created thread mapped into <see cref="ThreadWithDetailsDto"/></returns>
        /// <exception cref="NotFoundException">
        /// The user who creates the thread does not exist
        /// </exception>
        public Task<ThreadWithDetailsDto> CreateAsync(ThreadCreationDto threadDto, Guid authorId);

        /// <summary>
        /// Updates the thread
        /// </summary>
        /// <param name="id">Guid of the thread to be updated</param>
        /// <param name="threadDto">Thread update data</param>
        /// <exception cref="NotFoundException">
        /// Thrown when the thread specified by <paramref name="id"/> does not exist
        /// </exception>
        public Task UpdateAsync(Guid id, ThreadUpdateDto threadDto);
        
        /// <summary>
        /// Deletes the thread
        /// </summary>
        /// <param name="id">Guid of the thread to be deleted</param>
        /// <exception cref="NotFoundException">
        /// Thrown when the thread specified by <paramref name="id"/> does not exist
        /// </exception>
        public Task DeleteAsync(Guid id);

        /// <summary>
        /// Updates the status of the thread
        /// </summary>
        /// <param name="id">Guid of the thread whose status to be updated</param>
        /// <param name="statusDto">Thread status update data</param>
        /// <exception cref="NotFoundException">
        /// Thrown when the thread specified by <paramref name="id"/> does not exist
        /// </exception>
        public Task UpdateStatusAsync(Guid id, ThreadStatusUpdateDto statusDto);
    }
}