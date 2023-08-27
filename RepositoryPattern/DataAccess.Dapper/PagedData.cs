using DataAccess.Dapper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dapper
{
    public class PagedData<T> : IPagedData<T>
    {
        public PagedData(int itemsCounts, int pageSize, int currentPage, List<T> data)
        {
            ItemsCount = itemsCounts;
            TotalPages = PageCounts(ItemsCount, pageSize);
            if (currentPage <= 0)
            {
                CurrentPage = 0;
                PreviousPage = 0;
                HasPreviousPage = false;
            }
            else
            {
                CurrentPage = currentPage;
                PreviousPage = currentPage - 1;
                HasPreviousPage = true;
            }

            if (currentPage >= TotalPages)
            {
                CurrentPage = TotalPages;
                NextPage = TotalPages;
                HasNextPage = false;
            }
            else
            {
                CurrentPage = currentPage;
                NextPage = currentPage + 1;
                HasNextPage = true;
            }

            Data = data;
        }

        private int PageCounts(int rowCounts, int pageSize)
        {
            var result = 0;
            if (rowCounts % pageSize > 0)
            {
                result = rowCounts / pageSize + 1;
                return result;
            }
            else
            {
                result = rowCounts / pageSize;
                return result;
            }
        }

        public int ItemsCount { get; private set; }

        public int TotalPages { get; private set; }

        public int CurrentPage { get; private set; }

        public int PreviousPage { get; private set; }

        public int NextPage { get; private set; }

        public bool HasPreviousPage { get; private set; }

        public bool HasNextPage { get; private set; }

        public List<T> Data { get; private set; }
    }
}
