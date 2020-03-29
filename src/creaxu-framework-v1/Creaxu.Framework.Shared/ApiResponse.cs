using System;

namespace Creaxu.Framework.Shared
{
    public class ApiResponse
    {
        public bool Success => Error == null;

        public ApiError Error { get; set; }

        public void SetError(string status)
        {
            SetError(status, status);
        }

        public void SetError(Exception exception)
        {
            SetError("Unhandled", exception.Message);
        }

        public void SetError(string status, string message)
        {
            Error = new ApiError { Status = status, Message = message };
        }
    }
}
