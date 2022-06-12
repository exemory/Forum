using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Interfaces
{
    public interface IRepository<T> where T : EntityBase
    {
        Task<IEnumerable<T>> GetAllAsync();

        Task<T> GetByIdAsync(Guid id);

        void Add(T entity);
        
        void Update(T entity);
        
        void Delete(T entity);
        
        void Delete(Guid id);
    }
}