// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace PMDEvers.Servicebus
{
	public delegate IEnumerable<object> MultiInstanceFactory(Type serviceType);
}
