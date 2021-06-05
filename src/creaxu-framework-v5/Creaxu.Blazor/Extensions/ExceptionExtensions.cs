using System;
using System.Collections.Generic;
using System.Linq;

namespace Creaxu.Blazor.Extensions
{
    public static class ExceptionExtensions
    {

        /// <summary>
        /// Returns a list of all the exception messages from the top-level
        /// exception down through all the inner exceptions. Useful for making
        /// logs and error pages easier to read when dealing with exceptions.
        /// Usage: Exception.Messages()
        /// </summary>
        private static IEnumerable<string> Messages(this Exception ex)
        {
            // return an empty sequence if the provided exception is null
            if (ex == null) { yield break; }
            // first return THIS exception's message at the beginning of the list
            yield return ex.Message;
            // then get all the lower-level exception messages recursively (if any)
            var innerExceptions = Enumerable.Empty<Exception>();

            if (ex is AggregateException exception && exception.InnerExceptions.Any())
            {
                innerExceptions = exception.InnerExceptions;
            }
            else if (ex.InnerException != null)
            {
                innerExceptions = new Exception[] { ex.InnerException };
            }

            foreach (var innerEx in innerExceptions)
            {
                foreach (var msg in innerEx.Messages())
                {
                    yield return msg;
                }
            }
        }

        public static string GetAllMessages(this Exception ex)
        {
            return string.Join(Environment.NewLine, ex.Messages());
        }
    }
}
