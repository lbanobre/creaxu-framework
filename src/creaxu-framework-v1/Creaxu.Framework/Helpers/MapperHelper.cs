using System;
using Newtonsoft.Json;

namespace Creaxu.Framework.Helpers
{
    public static class MapperHelper
    {
        public static T Map<T>(object source)
        {
            if (source == null)
                return default;

            var json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
