using System;
using System.Collections.Generic;
using System.Linq;

namespace Creaxu.Framework.Shared
{
    public class ApiResponsePagedList<T> : ApiResponse
    {
        public PagedList<T> Data { get; set; }
    }
}
