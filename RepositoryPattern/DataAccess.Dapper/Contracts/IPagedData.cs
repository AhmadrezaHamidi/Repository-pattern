using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Dapper.Contracts
{
    public interface IPagedData<T>
    {
        int ItemsCount { get; }

        int TotalPages { get; }

        int CurrentPage { get; }

        int PreviousPage { get; }

        int NextPage { get; }

        bool HasPreviousPage { get; }

        bool HasNextPage { get; }

        List<T> Data { get; }
    }
}
