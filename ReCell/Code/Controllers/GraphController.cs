using System;
using System.Collections.Generic;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Controllers
{
	/// <summary>
	/// TODO: Implement Dictionary instead of List for better performance.
	/// </summary>
	public class GraphController
	{
		private static Logger logger = LoggerFactory.GetLogger();
		/// <summary>A list of all the graph components.</summary>
		private List<Graph> components;
		
		private static GraphController instance;
		public static GraphController Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new GraphController();
				}
				
				return instance;
			}
		}
		
		/// <summary>
		/// Creates an empty graph controller.
		/// Nothing to it, really.
		/// </summary>
		protected GraphController()
		{
			components = new List<Graph>();
			logger.Info("Constructed a new GraphController.");
		}
		
		/// <summary>
		/// Gets the graph of a building by searching through all graphs.
		/// </summary>
		/// <param name="b">The building to find the graph for.</param>
		/// <returns>The found graph.</returns>
		internal Graph GetGraph(Building b)
		{
			foreach(Graph g in components)
			{
				if (g.HasBuilding(b))
				{
					logger.Trace("Found graph "+g+" for building "+b+".");
					return g;
				}
			}
			logger.Error("Tried to fetch a graph for the building '"+b+"' and failed.");
			throw new GraphLessBuildingException();
		}
		
		/// <summary>
		/// Removes a building from its graph.
		/// </summary>
		/// <param name="building">The building to remove.</param>
		public void RemoveBuilding(Building building)
		{
			logger.Info("Removed building "+building+".");
			GetGraph(building).Remove(building);
		}
		
		/// <summary>
		/// Adds a building to a the same graph as another building.
		/// </summary>
		/// <param name="source">The building with a graph.</param>
		/// <param name="newBuilding">A new building, not belonging to a graph yet.</param>
		public void AddBuilding(Building source, Building newBuilding)
		{
			logger.Info("Added building " + newBuilding + " with source " + source + ".");
			GetGraph(source).Add(newBuilding);
		}
		
		/// <summary>
		/// Adds a base building. Base buildings forms their own graphs, and thus a new graph is created here.
		/// </summary>
		/// <param name="newBaseBuilding">The fresh base building to create a graph from.</param>
        /// <param name="sourceBuilding">The building using to build this, if the source building
        /// has no base building in its graph create a new one otherwise</param>
		/// <returns>The created graph.</returns>
		public Graph AddBaseBuilding(BaseBuilding newBaseBuilding, Building sourceBuilding)
		{
            if (sourceBuilding == null || (sourceBuilding).baseBuilding.IsAlive())
            {
                logger.Info("Added base building " + newBaseBuilding + " and created a new graph from it.");

                Graph graph = new Graph(newBaseBuilding);
                components.Add(graph);
                return graph;
            }

            else
            {
                logger.Info("Added base building " + newBaseBuilding + " to a graph missing an alive base building.");
                Graph graph = GetGraph(sourceBuilding);
                graph.baseBuilding = newBaseBuilding;
                return graph;
            }
		}

        /// <summary>
        /// Adds a base building. Base buildings forms their own graphs, and thus a new graph is created here.
        /// </summary>
        /// <param name="newBaseBuilding">The fresh base building to create a graph from.</param>
        /// <returns>The created graph.</returns>
        public Graph AddBaseBuilding(BaseBuilding newBaseBuilding)
        {
            logger.Info("Added base building " + newBaseBuilding + " and created a new graph from it.");
            Graph graph = new Graph(newBaseBuilding);
            components.Add(graph);
            return graph;
        }
		
		/// <summary>
		/// Sets the weight of a building.
		/// </summary>
		/// <param name="building">The building to set the weight for.</param>
		/// <param name="weight">The new weight.</param>
		public void SetWeight(Building building, int weight)
		{
			logger.Debug("Setting weight "+weight+" to building "+building+".");
			GetGraph(building).SetWeight(building, weight);
		}
		
		/// <summary>
		/// Sets the weight of a building by creating a menu and asking the user what weight the building should have.
		/// </summary>
		/// <param name="b">The building to set a weight for.</param>
		public void SetWeight(Building b)
		{
			//Menu menu = new Menu();
			// TODO: Construct the Menu options
			
			//MenuController.LoadMenu(menu);
			
			MenuIcon input = MenuController.GetInput();

			// TODO: Decide what option was opted for.
			
			SetWeight(b, 0);
		}
		
		/// <summary>
		/// Gets the weight of a building.
		/// </summary>
		/// <param name="building">The building to get the weight for.</param>
		/// <returns>The weight of the building.</returns>
		public int GetWeight(Building building)
		{
			logger.Debug("Getting the weight for building " + building + ".");
			return GetGraph(building).GetWeight(building);
		}
		
		/// <summary>
		/// Calculates and carries out distribution of units in the graphs according to the buildings weights.
		/// 
		/// Warning: This method is not healthy. Not even for you. No. Dont.
		/// </summary>
		public void CalculateWeights()
		{
			logger.Info("Calculating weights for all graphs.");
			foreach(Graph g in components)
			{
				logger.Debug("Calculating weights for graph "+g+".");
				int totalUnits = SumUnitsInGraph(g);

				// Figure out the unit balance for each building
				LinkedList<BuildingBalance> inNeed = new LinkedList<BuildingBalance>();
				LinkedList<BuildingBalance> withExcess = new LinkedList<BuildingBalance>();
				logger.Debug("Figuring out the unit balancing for each building");
				foreach(Building b in g.GetBuildings())
				{
					float factor = (float)g.GetWeight(b) / (float)g.TotalWeight;
					int unitGoal = (int)(((float)totalUnits) * factor);
					int unitBalance = b.CountUnits() - unitGoal;

					logger.Trace("Unit goal for " + b + " ((" + g.GetWeight(b) + " / " + g.TotalWeight + ") * " + totalUnits + ") is " + unitGoal + " which has " + b.CountUnits() + " units. Balance = " + unitBalance + ".");
					if (unitBalance > 0)
					{
						logger.Trace("Building has extra units to give.");
						withExcess.AddLast(new BuildingBalance(b, unitBalance));
					}
					else if (unitBalance < 0)
					{
						logger.Trace("Building is in need of units.");
						inNeed.AddLast(new BuildingBalance(b, unitBalance));
					}
					else
					{
						logger.Trace("Building is satisfied.");
					}
				}
				
				// If there is no need, don't balance.
				if (inNeed.Count == 0)
				{
					logger.Debug("There is no need, don't balance.");
					return;
				}
				
				// If there is nothing to give, don't balance.
				if (withExcess.Count == 0)
				{
					logger.Debug("There is nothing to give, don't balance.");
					return;
				}

				// Try to even out the unit count in every building
				logger.Debug("Trying to even out the unit count in every building.");
				bool balancingIsPossible = inNeed.Count > 0 && withExcess.Count > 0;
				while (balancingIsPossible)
				{
					BuildingBalance want = inNeed.First.Value;
					BuildingBalance has = withExcess.First.Value;
					int transferableUnits = Math.Min(has.balance, Math.Abs(want.balance));

					logger.Trace("Transferring units "+transferableUnits+" from " + want + " to " + has + ".");
					
					has.balance -= transferableUnits;
					want.balance += transferableUnits;
					
					MoveUnits(transferableUnits, want.building, has.building);
					
					if (has.balance == 0)
					{
						logger.Trace("Having building "+has+" is now satisfied. Removing from balancing.");
						
						withExcess.Remove(has);
					}
					
					if (want.balance == 0)
					{
						logger.Trace("Wanting building "+want + " is now satisfied. Removing from balancing.");
						inNeed.Remove(want);
					}
					
					
					balancingIsPossible = (inNeed.Count > 0 && withExcess.Count > 0);
				}
				logger.Trace("Done calculating weights for graph "+g+".");
			}
			logger.Debug("Done calculating weights for all graphs.");
		}

		internal static int SumUnitsInGraph(Graph g)
		{
			// Calculate total number of units
			int totalUnits = 0;
			foreach (Building b in g.GetBuildings())
			{
				totalUnits += b.CountUnits();
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
		
		/// <param name="numberOfUnits">The number of units to move.</param>
		/// <param name="from">From what building to move.</param>
		/// <param name="to">Destination for the units.</param>
		private void MoveUnits(int numberOfUnits, Building from, Building to)
		{
			UnitController.MoveUnits(numberOfUnits, from.controlZone.First.Value, to.controlZone.First.Value);
		}
	}

	/// <summary>
	/// Exception for when a building does not appear to belong to a graph.
	/// This is serious enough to have its own exception.
	/// </summary>
	public class GraphLessBuildingException : Exception
	{
		private static string msg = "A building does not belong to a graph. "+
									"All buildings should belong to a graph.";
		
		public GraphLessBuildingException() : base(msg)
		{
		}
	}
}
