using System.Collections.Generic;

namespace BTL.Areas.Admin.ViewModel
{
    public interface IPagedList<T> : IList<T>
    {
        int Page { get; }
        int PageSize { get; }
        int TotalCount { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
    }
}
