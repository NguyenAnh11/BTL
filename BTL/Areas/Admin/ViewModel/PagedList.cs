using System;
using System.Linq;
using System.Collections.Generic;

namespace BTL.Areas.Admin.ViewModel
{
    public class PagedList<T>: List<T>, IPagedList<T>
    {
        public PagedList(IList<T> source, int pageIndex, int pageSize, int? totalCount = null)
        {
            pageSize = Math.Max(pageSize, 1);

            TotalCount = totalCount ?? source.Count;
            TotalPages = TotalCount / pageSize;

            if (TotalCount % pageSize > 0)
                TotalPages++;

            PageSize = pageSize;
            Page = pageIndex;
            
            AddRange(source);
        }

        public int Page { get; }
        public int PageSize { get; }
        public int TotalCount { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage => Page > 0;
        public bool HasNextPage => Page + 1 < TotalPages;
    }
}