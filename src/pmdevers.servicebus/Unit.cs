// Copyright (c) Patrick Evers. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;

namespace PMDEvers.Servicebus
{
	public struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
	{
		public static readonly Unit Value = new Unit();
		public static readonly Task<Unit> Task = System.Threading.Tasks.Task.FromResult(Value);

		public int CompareTo(object obj)
		{
			return 0;
		}

		public int CompareTo(Unit other)
		{
			return 0;
		}

		public bool Equals(Unit other)
		{
			return true;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public override bool Equals(object obj)
		{
			return obj is Unit;
		}

		public static bool operator ==(Unit first, Unit second)
		{
			return true;
		}

		public static bool operator !=(Unit first, Unit second)
		{
			return false;
		}

		public override string ToString()
		{
			return "()";
		}
	}
}
