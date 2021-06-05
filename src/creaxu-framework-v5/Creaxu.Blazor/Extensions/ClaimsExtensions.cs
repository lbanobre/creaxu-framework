using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;

namespace Creaxu.Blazor.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetClaimValue(this ClaimsPrincipal user, string type)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == type);

            return claim?.Value;
        }

        public static T GetClaimValue<T>(this ClaimsPrincipal user, string type)
        {
            var claim = user.Claims.FirstOrDefault(c => c.Type == type);

            if (claim == null || string.IsNullOrEmpty(claim.Value)) 
                return default(T);
            
            var converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(claim.Value);
        }
    }
}
