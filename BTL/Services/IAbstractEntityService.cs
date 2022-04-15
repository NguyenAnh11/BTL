using BTL.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BTL.Services
{
    public interface IAbstractEntityService<T> where T : BaseEntity
    {
        IQueryable<T> Table { get; }
        Task<T> GetByIdAsync(int id, bool includeDeleted = false);
        Task<IList<T>> GetByIdsAsync(int[] ids, bool includeDeleted = false);
        Task<IList<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> func = null, bool includeDeleted = false);
        Task<IList<T>> GetAllAsync(Func<IQueryable<T>, Task<IQueryable<T>>> func = null, bool includeDeleted = false);
    }
}
