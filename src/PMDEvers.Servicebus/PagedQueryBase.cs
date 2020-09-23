using System;
using System.Collections.Generic;
using System.Text;

using PMDEvers.Servicebus.Interfaces;

namespace PMDEvers.Servicebus
{
    public abstract class PagedQueryBase : IPagedQuery
    {
        public int Page { get; set; }
        public int Results { get; set; }
        public string OrderBy { get; set; }
        public string SortOrder { get; set; }
    }
}
