using System;
using System.Collections.Generic;

namespace Creaxu.Framework.Shared
{
    public class PagedList<T>
    {
        public List<T> List { get; set; }

        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public int PageSize => 15;

        public bool HasPreviousPage
        {
            get
            {
                return (Page > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (Page < TotalPages);
            }
        }

        public PagedList()
        {
            List = new List<T>();
        }

        public PagedList(IEnumerable<T> subset, int page, int totalPages, int totalCount) : this()
        {
            Page = page;
            TotalPages = totalPages;
            TotalCount = totalCount;

            List.AddRange(subset);
        }

        public override string ToString()
        {
            if(TotalCount == 0)
            {
                return "Records not found";
            }

            if(Page == 1 && List.Count < PageSize)
            {
                return $"{List.Count} results";
            }

            return $"{((Page - 1) * PageSize) + 1}-{(Page - 1) * PageSize + List.Count} of {TotalCount.ToString("N0")}";
        }
    }
}
