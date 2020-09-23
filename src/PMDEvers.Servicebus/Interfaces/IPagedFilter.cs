using System;
using System.Collections.Generic;
using System.Text;

namespace PMDEvers.Servicebus.Interfaces
{
    public interface IPagedFilter<TResult, in TQuery> where TQuery : IQuery
    {
        IPagedResult<TResult> Filter(IEnumerable<TResult> values, TQuery query);
    }
}
