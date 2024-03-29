﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Entities;
using Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories
{
    /// <inheritdoc />
    public class Repository<T> : IRepository<T> where T : EntityBase, new()
    {
        protected readonly ForumContext Context;
        protected readonly DbSet<T> Set;

        /// <summary>
        /// Constructor for initializing a <see cref="Repository{T}"/> class instance
        /// </summary>
        /// <param name="context">Context of the database</param>
        public Repository(ForumContext context)
        {
            Context = context;
            Set = Context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Set.AsNoTracking()
                .ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await Set.FindAsync(id);
        }

        public void Add(T entity)
        {
            Set.Add(entity);
        }

        public void Update(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            Set.Remove(entity);
        }

        public void DeleteById(Guid id)
        {
            var entity = new T {Id = id};
            Set.Remove(entity);
        }
    }
}