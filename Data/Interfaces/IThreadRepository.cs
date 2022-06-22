using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Interfaces
{
    /// <summary>
    /// Repository of <see cref="Thread"/> entities
    /// </summary>
    public interface IThreadRepository : IRepository<Thread>
    {
        /// <summary>
        /// Retrieves thread specified by <paramref name="id"/>
        /// </summary>
        /// <param name="id">Guid of the thread to be retrieved</param>
        /// <returns>
        /// <see cref="Thread"/> including its author, if exists, <c>null</c> otherwise/>
        /// </returns>
        public Task<Thread> GetByIdWithDetailsAsync(Guid id);
        
        /// <summary>
        /// Retrieves all threads
        /// </summary>
        /// <returns>
        /// List of threads, including their authors, ordered by creation date by descending order
        /// </returns>
        public Task<IEnumerable<Thread>> GetAllWithDetailsAsync();
    }
}