﻿using System.Linq.Expressions;

namespace DataStorageCore.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    }
}