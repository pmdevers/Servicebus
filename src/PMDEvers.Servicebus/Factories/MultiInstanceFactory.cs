using System;
using System.Collections.Generic;

namespace PMDEvers.Servicebus
{
    public delegate IEnumerable<object> MultiInstanceFactory(Type serviceType);
}
