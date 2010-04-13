using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recellection.Code.Utility.Logger;
using Recellection.Code.Utility;
using Recellection.Code.Utility.Events;

namespace Recellection.Code.Models
{
	/// <summary>
	/// The Graph component is a storage class for buildings and their weights.
	/// 
	/// Author: Martin Nycander
	/// </summary>
	public class Graph : IModel
	{
		private static Logger logger = LoggerFactory.GetLogger();
		private static int defaultWeight = 1;
		private Dictionary<Building, int> buildings;
		
		public event Publish<Building> weightChanged;
		
		/// <summary>
		/// Constructs and initializes an empty graph.
		/// </summary>
		public Graph()
		{
			logger.Trace("Constructing new graph.");
			buildings = new Dictionary<Building, int>();
		}

		/// <summary>
		/// Adds a building to the graph.
		/// A building that has already been added will be ignored.
		/// </summary>
		/// <param name="building">The building to add.</param>
		public void Add(Building building)
		{
			if (buildings.ContainsKey(building))
			{
				logger.Debug("Can not add building to graph. The building '"+building+"' already exists.");
				return;
			}

			buildings.Add(building, defaultWeight);
			weightChanged(this, new GraphEvent(building, defaultWeight, EventType.ADD));
		}

		/// <summary>
		/// Removes a building from the graph.
		/// </summary>
		/// <param name="building">The building to remove.</param>
		public void Remove(Building building)
		{
			buildings.Remove(building);
			weightChanged(this, new GraphEvent(building, 0, EventType.REMOVE));
		}

		/// <summary>
		/// Sets the weight of a building node in the graph.
		/// The building is added to the graph, if it is not a part of the graph.
		/// </summary>
		/// <param name="building">The building to set weight for.</param>
		/// <param name="weight">The new weight.</param>
		public void SetWeight(Building building, int weight)
		{
			if (! buildings.ContainsKey(building))
			{
				Add(building);
			}
			
			buildings[building] = weight;
			weightChanged(this, new GraphEvent(building, weight, EventType.ALTER));
		}

		/// <param name="building">The building to get weight for.</param>
		/// <returns>the weight of the building.</returns>
		/// <exception cref="ArgumentException">if the building is not a part of the graph.</exception>
		public int GetWeight(Building building)
		{
			int weight;
			
			if (! buildings.TryGetValue(building, out weight))
			{
				throw new ArgumentException("That building does not exist in this graph.");
			}
			
			return weight;
		}

		/// <returns>The number of buildings in this graph.</returns>
		public int CountBuildings()
		{
			return buildings.Count();
		}
	}
}
