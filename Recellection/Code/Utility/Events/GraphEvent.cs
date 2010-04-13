using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Utility.Events
{
	public class GraphEvent : Event<Graph>
	{
		public GraphEvent(Graph subject, Type type) : base(subject, type)
		{
		}
	}
}
