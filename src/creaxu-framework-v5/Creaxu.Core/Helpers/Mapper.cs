using System;
using System.Text.Json;

namespace Creaxu.Core.Helpers
{
    public static class Mapper
    {
        public static T Map<T>(object source)
        {
            if (source == null)
                return default;

            var json = JsonSerializer.Serialize(source);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
