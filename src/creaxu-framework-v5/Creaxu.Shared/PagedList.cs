using System;
using System.Collections.Generic;

namespace Creaxu.Shared
{
    public class PagedList
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int ListCount { get; set; }

        public int PageSize => 20;

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

        public override string ToString()
        {
            if(TotalCount == 0)
            {
                return "Records not found";
            }

            if(Page == 1 && ListCount < PageSize)
            {
                return $"{ListCount} results";
            }

            return $"{((Page - 1) * PageSize) + 1}-{(Page - 1) * PageSize + ListCount} of {TotalCount.ToString("N0")}";
        }
    }
}
