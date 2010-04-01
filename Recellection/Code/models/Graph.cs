using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
	/**
	* The Graph component is a storage class for buildings and their weights.
	*/
	class Graph
	{
		private static int defaultWeight = 1;

		private Dictionary<Building, int> buildings;

		public Graph()
		{
			buildings = new Dictionary<Building, int>();
		}

		public void Add(Building building)
		{
			buildings.Add(building, defaultWeight);
		}

		public void Remove(Building building)
		{
			buildings.Remove(building);
		}

		public void setWeight(Building building, int weight)
		{
			if (! buildings.ContainsKey(building))
			{
				Add(building);
			}
			
			buildings[building] = weight;
		}

		public int getWeight(Building building)
		{
			int weight;
			
			if (! buildings.TryGetValue(building, out weight))
			{
				throw new ArgumentException("That building does not exist.");
			}
			
			return weight;
		}
	}
}
