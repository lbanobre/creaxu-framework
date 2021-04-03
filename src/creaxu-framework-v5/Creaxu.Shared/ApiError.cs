using System;

namespace Creaxu.Shared
{
    public class ApiError
    {
        public string Status { get; set; }
        public string Message { get; set; }

        public override string ToString()
        {
            return !string.IsNullOrEmpty(Message) ? Message : Status;
        }
    }
}
