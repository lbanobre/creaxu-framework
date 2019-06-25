using System;
using System.Collections.Generic;

namespace Creaxu.Framework.Shared
{
    public class ApiResponseList<T> : ApiResponse
    {
        public List<T> Data { get; set; }
    }
}
