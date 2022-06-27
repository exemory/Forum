using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Interfaces
{
    /// <summary>
    /// Generic repository
    /// </summary>
    /// <typeparam name="T">Entity type, inherited from <see cref="EntityBase"/></typeparam>
    public interface IRepository<T> where T : EntityBase
    {
        /// <summary>
        /// Retrieves all entities
        /// </summary>
        /// <returns>List of entities</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Retrieves entity specified by <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id of the entity to be retrieved</param>
        /// <returns>
        /// Entity specified by <paramref name="id"/>, if exists, <c>null</c> otherwise
        /// </returns>
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Adds entity
        /// </summary>
        /// <param name="entity">Entity to be started tracking by context as added</param>
        void Add(T entity);
        
        /// <summary>
        /// Updates entity
        /// </summary>
        /// <param name="entity">Entity to be started tracking by context as modified</param>
        void Update(T entity);
        
        /// <summary>
        /// Deletes entity
        /// </summary>
        /// <param name="entity">Entity to be started tracking by context as deleted</param>
        void Delete(T entity);
        
        /// <summary>
        /// Deletes entity specified by <paramref name="id"/>
        /// </summary>
        /// <param name="id">Guid of the entity to be started tracking by context as deleted</param>
        void DeleteById(Guid id);
    }
}