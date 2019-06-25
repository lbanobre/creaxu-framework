using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Creaxu.Framework.Shared;

namespace Creaxu.Framework.Extensions
{
    public static class PagedExtension
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int page) where T : class
        {
            var pagedList = new PagedList<T>();
            pagedList.Page = page;            
            pagedList.TotalCount = await source.CountAsync();
            pagedList.TotalPages = (int)Math.Ceiling(pagedList.TotalCount / (double)pagedList.PageSize);

            var items = await source.Skip((page - 1) * pagedList.PageSize).Take(pagedList.PageSize).ToListAsync();
            pagedList.List.AddRange(items);
            
            return pagedList;
        }
    }
}
