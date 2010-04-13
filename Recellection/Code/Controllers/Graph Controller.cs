using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

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
				int totalUnits = 0;
				g.ForEachBuilding(delegate(Building b)
				{
					totalUnits += b.GetUnits().Count;
				});
				
				
				g.ForEachBuilding(delegate(Building b)
				{
					int unitGoal = totalUnits / g.GetWeight(new BaseBuilding());
					int unitBalance = b.GetUnits().Count - unitGoal;
					
				});
			}
		}
		
		/// <summary>
		/// The Graph Controller will call the Unit Controller with orders about changes in unit positioning.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		private void MoveUnits(int number, int x, int y)
		{
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
