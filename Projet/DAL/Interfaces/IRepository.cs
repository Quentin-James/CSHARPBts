using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> PostAsync(T entity);

        IQueryable<T> Find(Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, IProperty>>? navigationPropertyPath = null,
            bool asNoTracking = true);
    }
}
