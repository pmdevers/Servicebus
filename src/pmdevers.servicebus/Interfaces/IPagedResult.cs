using System;
using System.Collections.Generic;

namespace PMDEvers.Servicebus.Interfaces
{
    public interface IPagedResult<T>
    {
        IEnumerable<T> Items { get; }
        bool IsEmpty { get; }
        bool IsNotEmpty { get; }
        int CurrentPage { get; }
        int ResultsPerPage { get; }
        int TotalPages { get; }
        long TotalResults { get; }

        IPagedResult<U> Map<U>(Func<T, U> map);
    }
}