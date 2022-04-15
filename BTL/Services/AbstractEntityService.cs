using BTL.Data;
using BTL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BTL.Services
{
    public abstract class AbstractEntityService<T> : IAbstractEntityService<T> where T : BaseEntity
    {
        protected readonly ShopDbContext _db;
        public AbstractEntityService(ShopDbContext db)
        {
            _db = db;
        }
        public IQueryable<T> Table => _db.Set<T>().AsQueryable();

        protected virtual IQueryable<T> ApplyDeletedFilter(IQueryable<T> source, bool includeDeleted)
        {
            if (includeDeleted)
                return source;

            return source;
        }
        public virtual async Task<T> GetByIdAsync(int id, bool includeDeleted = false)
        {
            if(id == 0) return null;    

            var query = ApplyDeletedFilter(Table, includeDeleted);

            return await query.FirstOrDefaultAsync(p => p.Id == id);
        }

        public virtual async Task<IList<T>> GetByIdsAsync(int[] ids, bool includeDeleted = false)
        {
            if(ids == null) throw new ArgumentNullException(nameof(ids));

            if (!ids.Any()) return null;

            var query = ApplyDeletedFilter(Table, includeDeleted);

            return await query.Where(p => ids.Contains(p.Id)).ToListAsync();
        }

        public virtual async Task<IList<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>> func = null, bool includeDeleted = false)
        {
            var query = ApplyDeletedFilter(Table, includeDeleted);

            query = func == null ? query : func(query);

            return await query.ToListAsync();
        }

        public virtual async Task<IList<T>> GetAllAsync(Func<IQueryable<T>, Task<IQueryable<T>>> func = null, bool includeDeleted = false)
        {
            var query = ApplyDeletedFilter(Table, includeDeleted);

            query = func == null ? query : await func(query);

            return await query.ToListAsync();
        }
    }
}