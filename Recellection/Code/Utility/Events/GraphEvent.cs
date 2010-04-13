using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
	public class GraphEvent : Event<Building>
	{
		public int weight { get; private set; }
		
		public GraphEvent(Building building, int weight, Type type) : base(building, type)
		{
			this.weight = weight;
		}
	}
}
