using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ConfigurationService.Data
{
    public interface IDocumentRepository<T, in TId> 
        where T : class 
        where TId : struct

    {
        Task<T> RemoveAsync(TId id, string ETag, RequestOptions requestOptions = null);
        Task<T> CreateAsync(T entity, RequestOptions requestOptions = null);
        Task<T> UpdateAsync(TId id, T entity, RequestOptions requestOptions = null);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(TId id);
        Task<T> FirstOrDefaultAsync(Func<T, bool> predicate);
        Task<IQueryable<T>> WhereAsync(Expression<Func<T, bool>> predicate);
        Task<IQueryable<T>> QueryAsync();
    }
}
