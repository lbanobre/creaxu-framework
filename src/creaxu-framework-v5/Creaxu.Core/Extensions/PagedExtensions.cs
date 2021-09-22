using System;
using System.Linq;
using System.Threading.Tasks;
using Creaxu.Shared;
using Microsoft.EntityFrameworkCore;

namespace Creaxu.Core.Extensions
{
    public static class PagedExtensions
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int page) where T : class
        {
            var pagedList = new PagedList<T>();
            pagedList.Page = page;            
            pagedList.TotalCount = await source.CountAsync();
            pagedList.TotalPages = (int)Math.Ceiling(pagedList.TotalCount / (double)pagedList.PageSize);

            var items = await source.Skip((page - 1) * pagedList.PageSize).Take(pagedList.PageSize).ToListAsync();
            pagedList.List.AddRange(items);

            pagedList.ListCount = items.Count;
            
            return pagedList;
        }
    }
}