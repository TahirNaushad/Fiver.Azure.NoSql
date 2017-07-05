using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace Fiver.Azure.NoSql
{
    public interface IAzureNoSqlRepository<T>
    {
        Task<List<T>> GetList();
        Task<List<T>> GetList(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetList(string sql);
        Task<T> GetItem(string id);
        Task<Document> Insert(T item);
        Task<Document> Update(string id, T item);
        Task<Document> InsertOrUpdate(T item);
        Task Delete(string id);
    }
}