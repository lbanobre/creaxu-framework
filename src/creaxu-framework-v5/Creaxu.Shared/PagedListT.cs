using System;
using System.Collections.Generic;

namespace Creaxu.Shared
{
    public class PagedList<T> : PagedList
    {
        public List<T> List { get; set; }

        public PagedList()
        {
            List = new List<T>();
        }
    }
}
