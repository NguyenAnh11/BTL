using BTL.Data;
using BTL.Models;
using BTL.Models.Interfaces;
using System.Linq;

namespace BTL.Services
{
    public class AbstractSoftDeleteEntityService<T> : AbstractEntityService<T> where T: BaseEntity, ISoftDeletedEntity
    {
        public AbstractSoftDeleteEntityService(ShopDbContext db) : base(db)
        {
        }

        protected override IQueryable<T> ApplyDeletedFilter(IQueryable<T> source, bool includeDeleted)
        {
            var query = base.ApplyDeletedFilter(source, includeDeleted);

            return query.Where(p => !p.IsDeleted);
        }
    }
}