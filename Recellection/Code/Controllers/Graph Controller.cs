using System;
using System.Collections.Generic;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Controllers
{
	/// <summary>
	/// TODO: Implement Dictionary instead of List for better performance.
	/// </summary>
	public class GraphController
	{
		/// <summary>A list of all the graph components.</summary>
		private List<Graph> components;
		
		/// <summary>
		/// Creates an empty graph controller.
		/// Nothing to it, really.
		/// </summary>
		public GraphController()
		{
			components = new List<Graph>();
		}
		
		/// <summary>
		/// Gets the graph of a building by searching through all graphs.
		/// </summary>
		/// <param name="b">The building to find the graph for.</param>
		/// <returns>The found graph.</returns>
		private Graph GetGraph(Building b)
		{
			foreach(Graph g in components)
			{
				if (g.HasBuilding(b))
				{
					return g;
				}
			}
			
			throw new GraphLessBuildingException();
		}
		
		/// <summary>
		/// Removes a building from its graph.
		/// </summary>
		/// <param name="building">The building to remove.</param>
		public void RemoveBuilding(Building building)
		{
			GetGraph(building).Remove(building);
		}
		
		/// <summary>
		/// Adds a building to a the same graph as another building.
		/// </summary>
		/// <param name="source">The building with a graph.</param>
		/// <param name="newBuilding">A new building, not belonging to a graph yet.</param>
		public void AddBuilding(Building source, Building newBuilding)
		{
			GetGraph(source).Add(newBuilding);
		}
		
		/// <summary>
		/// Adds a base building. Base buildings forms their own graphs, and thus a new graph is created here.
		/// </summary>
		/// <param name="newBaseBuilding">The fresh base building to create a graph from.</param>
		/// <returns>The created graph.</returns>
		public Graph AddBaseBuilding(BaseBuilding newBaseBuilding)
		{
			Graph graph = new Graph(newBaseBuilding);
			components.Add(graph);
			return graph;
		}
		
		/// <summary>
		/// Sets the weight of a building.
		/// </summary>
		/// <param name="building">The building to set the weigh for.</param>
		/// <param name="weight">The new weight.</param>
		public void SetWeight(Building building, int weight)
		{
			GetGraph(building).SetWeight(building, weight);
		}
		
		/// <summary>
		/// for each Graph:
		///		get the total number of units in the graph;
		///		for each building in the graph:
		///			check that the ratio between the units in that building and its
		///			weight is proportional to the total number of units.
		/// </summary>
		public void CalculateWeights()
		{
			foreach(Graph g in components)
			{
				List<Building> buildings = g.GetBuildings();

				int totalUnits = SumUnitsInGraph(g);

				// Figure out the unit balance for each building
				LinkedList<BuildingBalance> inNeed = new LinkedList<BuildingBalance>();
				LinkedList<BuildingBalance> withExcess = new LinkedList<BuildingBalance>();
				foreach(Building b in buildings)
				{
					int unitGoal = totalUnits / g.GetWeight(new BaseBuilding());
					int unitBalance = b.GetUnits().Count - unitGoal;
					
					if (unitBalance > 0)
					{
						withExcess.AddLast(new BuildingBalance(b, unitBalance));
					}
					else if (unitBalance < 0)
					{
						inNeed.AddLast(new BuildingBalance(b, unitBalance));
					}
				}
				
				// If there is no need, don't balance.
				if (inNeed.Count == 0)
					return;
				
				// If there is nothing to give, don't balance.
				if (withExcess.Count == 0)
					return;
				
				// Try to even out the unit count in every building
				bool balancingIsPossible = inNeed.Count > 0 && withExcess.Count > 0;
				while (balancingIsPossible)
				{
					BuildingBalance want = inNeed.First.Value;
					BuildingBalance has = withExcess.First.Value;
					
					int transferableUnits = Math.Min(has.balance, Math.Abs(want.balance));
					
					has.balance -= transferableUnits;
					want.balance += transferableUnits;
					
					if (has.balance == 0)
					{
						withExcess.Remove(has);
					}
					
					if (want.balance == 0)
					{
						inNeed.Remove(want);
					}
					
					MoveUnits(transferableUnits, want.building, has.building);
					
					balancingIsPossible = (inNeed.Count > 0 && withExcess.Count > 0);
				}
			}
		}

		private static int SumUnitsInGraph(Graph g)
		{
			List<Building> buildings = g.GetBuildings();
			// Calculate total number of units
			int totalUnits = 0;
			foreach (Building b in buildings)
			{
				totalUnits += b.GetUnits().Count;
			}
			return totalUnits;
		}
		
		private struct BuildingBalance : IComparer<BuildingBalance>
		{
			public Building building;
			public int balance;
			
			public BuildingBalance(Building b, int bal)
			{
				building = b;
				balance = bal;
			}
			
			int IComparer<BuildingBalance>.Compare(BuildingBalance bb1, BuildingBalance bb2)
			{
				return bb1.balance - bb2.balance;
			}
		}
		
		/// <summary>
		/// </summary>
		/// <param name="number"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void MoveUnits(int numberOfUnits, Building from, Building to)
		{
			// TODO: call the Unit Controller with orders about changes in unit positioning.
		}
	}

	/// <summary>
	/// Exception for when a building does not appear to belong to a graph.
	/// This is serious enough to have its own exception.
	/// </summary>
	public class GraphLessBuildingException : Exception
	{
		private static string msg = "A building does not belong to a graph."+
									"All buildings should belong to a graph.";
		
		public GraphLessBuildingException() : base(msg)
		{
		}
	}
}
