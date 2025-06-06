﻿using System.Linq.Expressions;

namespace EasyFly.Domain.Abstractions
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(Guid id, bool trackChanges);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> condition);
        Task<IEnumerable<T>> GetAllAsync(bool trackChanges);
        Task<IEnumerable<T>> GetPagedAsync(bool trackChanges, int page, int size);
        Task<T?> GetByAsync(Expression<Func<T, bool>> condition);
        Task<bool> InsertAsync(T value);
        Task<bool> UpdateAsync(T value);
        Task<bool> DeleteAsync(T value);
        Task<int> GetPageCount(int size);
    }
}
