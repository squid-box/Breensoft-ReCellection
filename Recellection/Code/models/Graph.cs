using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recellection.Code.Main;

namespace Recellection.Code.Models
{
	/**
	 * The Graph component is a storage class for buildings and their weights.
	 * 
	 * TODO: Implement Publisher-pattern.
	 * 
	 * @author Martin Nycander
	 */
	public class Graph
	{
		private static Logger logger = Logger.getLogger();
		private static int defaultWeight = 1;
		private Dictionary<Building, int> buildings;

		/**
		 * Constructs and initializes an empty graph.
		 */
		public Graph()
		{
			logger.Trace("Constructing new graph.");
			buildings = new Dictionary<Building, int>();
		}

		/**
		 * Adds a building to the graph.
		 * A building that has already been added will be ignored.
		 * 
		 * @param building the building to add
		 */
		public void Add(Building building)
		{
			if (buildings.ContainsKey(building))
			{
				logger.Debug("Can not add building to graph. The building '"+building+"' already exists.");
				return;
			}
			
			buildings.Add(building, defaultWeight);
		}

		/**
		 * Removes a building from the graph.
		 * 
		 * @param building the building to remove
		 */
		public void Remove(Building building)
		{
			buildings.Remove(building);
		}

		/**
		 * Sets the weight of a building node in the graph.
		 * The building is added to the graph, if it is not a part of the graph.
		 * 
		 * @param building the building to set weight for.
		 * @param weight the new weight.
		 */
		public void SetWeight(Building building, int weight)
		{
			if (! buildings.ContainsKey(building))
			{
				Add(building);
			}
			
			buildings[building] = weight;
		}

		/**
		 * @return the weight of the building.
		 * @throws ArgumentException if the building is not a part of the graph.
		 */
		public int GetWeight(Building building)
		{
			int weight;
			
			if (! buildings.TryGetValue(building, out weight))
			{
				throw new ArgumentException("That building does not exist in this graph.");
			}
			
			return weight;
		}
		
		/**
		 * @return the number of buildings in this graph.
		 */
		public int CountBuildings()
		{
			return buildings.Count();
		}
	}
}
