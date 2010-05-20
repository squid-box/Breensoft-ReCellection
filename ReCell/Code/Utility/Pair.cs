using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Utility
{
	class Pair<T>
	{
		public T First {get; set; }
		public T Second { get; set; }

		public Pair(T first, T second)
		{
			this.First = first;
			this.Second = second;
		}
	}
}
