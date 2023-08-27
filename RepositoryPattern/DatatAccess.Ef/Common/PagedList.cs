using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatatAccess.Ef.Common
{
    public class PagedList<T>
    {
        public int CurrentPage { get; }

        public int TotalPages { get; }

        public int PageSize { get; }

        public int TotalCount { get; }

        public List<T> Items { get; }

        public PagedList()
        {
        }

        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items.ToList();
        }

        public PagedList<TDest> MapTo<TDest>(IMapper mapper)
        {
            var items = mapper.Map<List<TDest>>(Items);
            return new PagedList<TDest>(items, TotalCount, CurrentPage, PageSize);
        }

        public static PagedList<T> Create(IQueryable<T> query, int pageNumber, int pageSize, int count)
        {
            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int pageNumber, int pageSize, int count)
        {
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync().ConfigureAwait(false);
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
