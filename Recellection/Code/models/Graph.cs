using System;
using System.Collections.Generic;
using System.Linq;

using Recellection.Code.Utility.Logger;
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
		
		private Graph()
		{
			logger.Trace("Constructing new graph.");
			buildings = new Dictionary<Building, int>();
		}
		
		/// <summary>
		/// Constructs and initializes a graph with a single basebuilding.
		/// </summary>
		public Graph(BaseBuilding baseBuilding) : this()
		{
			buildings.Add(baseBuilding, defaultWeight);
		}

		/// <summary>
		/// Adds a building to the graph.
		/// A building that has already been added will be ignored.
		/// </summary>
		/// <param name="building">The building to add.</param>
		/// <exception cref="ArgumentException">If the building is a base building.</exception>
		public void Add(Building building)
		{
			if (building is BaseBuilding)
			{
				throw new ArgumentException("BaseBuildings can not be added to graphs.");
			}
			
			if (buildings.ContainsKey(building))
			{
				logger.Debug("Can not add building to graph. The building '" + building + "' already exists.");
				throw new ArgumentException("The building '" + building + "' already exists in a graph.");
			}

			buildings.Add(building, defaultWeight);

			Publish(building, defaultWeight, EventType.ADD);
		}

		/// <summary>
		/// Removes a building from the graph.
		/// </summary>
		/// <param name="building">The building to remove.</param>
		public void Remove(Building building)
		{
			buildings.Remove(building);
			
			Publish(building, 0, EventType.REMOVE);
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
			
			Publish(building, weight, EventType.ALTER);
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
		
		/// <summary>
		/// Checks if a building exists in this graph.
		/// </summary>
		/// <param name="b">The building to check existance for.</param>
		/// <returns>True if the building exists, false if not.</returns>
		public bool HasBuilding(Building b)
		{
			return buildings.ContainsKey(b);
		}

		/// <returns>The number of buildings in this graph.</returns>
		public int CountBuildings()
		{
			return buildings.Count();
		}
		
		public List<Building> GetBuildings()
		{
			return buildings.Keys.ToList();
		}

		/// <summary>
		/// Publishes an event of change to all subscribers.
		/// </summary>
		/// <param name="building">The building that has changed.</param>
		/// <param name="weight">The weight of that building.</param>
		/// <param name="t">Type of event.</param>
		private void Publish(Building building, int weight, EventType t)
		{
			if (weightChanged != null)
			{
				weightChanged(this, new GraphEvent(building, weight, t));
			}
		}
	}
}
