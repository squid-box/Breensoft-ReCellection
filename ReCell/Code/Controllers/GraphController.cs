using System;
using System.Collections.Generic;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using Recellection.Code.Utility.Logger;
using Recellection.Code.Views;

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
			logger.SetThreshold(LogLevel.ERROR);
			logger.SetTarget(LoggerSetup.GetLogFileTarget("graphcontrol.log"));
			logger.Info("Constructed a new GraphController.");
		}
		
		/// <summary>
		/// Gets the graph of a fromBuilding by searching through all graphs.
		/// </summary>
		/// <param name="b">The fromBuilding to find the graph for.</param>
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
		/// Removes a fromBuilding from its graph.
		/// </summary>
		/// <param name="fromBuilding">The fromBuilding to remove.</param>
		public void RemoveBuilding(Building building)
		{
			logger.Info("Removed building "+building+".");
            Graph temp = GetGraph(building);
            if (building is BaseBuilding)
            {
                temp.baseBuilding = null;
            }
            
            
			GetGraph(building).Remove(building);

            
            if (temp.CountBuildings() == 0)
            {
                building.owner.GetGraphs().Remove(temp);
                components.Remove(temp);
            }
		}
		
		/// <summary>
		/// Adds a fromBuilding to a the same graph as another fromBuilding.
		/// </summary>
		/// <param name="source">The fromBuilding with a graph.</param>
		/// <param name="newBuilding">A new fromBuilding, not belonging to a graph yet.</param>
		public void AddBuilding(Building source, Building newBuilding)
		{
			logger.Info("Added building " + newBuilding + " with source " + source + ".");
			GetGraph(source).Add(newBuilding);
		}
		
		/// <summary>
		/// Adds a base fromBuilding. Base buildings forms their own graphs, and thus a new graph is created here.
		/// </summary>
		/// <param name="newBaseBuilding">The fresh base fromBuilding to create a graph from.</param>
        /// <param name="sourceBuilding">The fromBuilding using to build this, if the source fromBuilding
        /// has no base fromBuilding in its graph create a new one otherwise</param>
		/// <returns>The created graph.</returns>
		public Graph AddBaseBuilding(BaseBuilding newBaseBuilding, Building sourceBuilding)
		{
            if (sourceBuilding == null || (sourceBuilding).baseBuilding.IsAlive())
            {
                return AddBaseBuilding(newBaseBuilding);
            }
            else
            {
                logger.Info("Added base building " + newBaseBuilding + " to a graph missing an alive base building.");
                Graph graph = GetGraph(sourceBuilding);
                graph.baseBuilding = newBaseBuilding;
                graph.Add(newBaseBuilding);
                return graph;
            }
		}

        /// <summary>
        /// Adds a base fromBuilding. Base buildings forms their own graphs, and thus a new graph is created here.
        /// </summary>
        /// <param name="newBaseBuilding">The fresh base fromBuilding to create a graph from.</param>
        /// <returns>The created graph.</returns>
        public Graph AddBaseBuilding(BaseBuilding newBaseBuilding)
        {
            logger.Info("Added base building " + newBaseBuilding + " and created a new graph from it.");
            Graph graph = new Graph(newBaseBuilding);
            components.Add(graph);
            return graph;
        }
		
		/// <summary>
		/// Sets the weight of a fromBuilding.
		/// </summary>
		/// <param name="fromBuilding">The fromBuilding to set the weight for.</param>
		/// <param name="weight">The new weight.</param>
		public void SetWeight(Building building, int weight)
		{
			logger.Debug("Setting weight "+weight+" to building "+building+".");
			GetGraph(building).SetWeight(building, weight);
		}
		
		/// <summary>
		/// Sets the weight of a fromBuilding by creating a menu and asking the user what weight the fromBuilding should have.
		/// </summary>
		/// <param name="b">The fromBuilding to set a weight for.</param>
		public void SetWeight(Building b)
		{
			Dictionary<MenuIcon, int> doptions = new Dictionary<MenuIcon,int>(8);
			
			doptions.Add(new MenuIcon("Non-vital priority"), 1);
			doptions.Add(new MenuIcon("Low priority"), 2);
			doptions.Add(new MenuIcon("Medium low priority"), 4);
			doptions.Add(new MenuIcon("Medium priority"), 8);
			
			doptions.Add(new MenuIcon("Medium high priority"), 16);
			doptions.Add(new MenuIcon("High priority"), 32);
			doptions.Add(new MenuIcon("Very high priority"), 64);
			doptions.Add(new MenuIcon("GET TO THE CHOPPAH!"), 128);

			Menu menu = new Menu(Globals.MenuLayout.NineMatrix, 
							new List<MenuIcon>(doptions.Keys),
							Language.Instance.GetString("SetImportance"));
			
			MenuController.LoadMenu(menu);
			
            Recellection.CurrentState = MenuView.Instance;

			#region GET TO THE CHOPPAH!
			MenuIcon selection = MenuController.GetInput();
			if (128 == doptions[selection])
			{
				Sounds.Instance.LoadSound("choppah").Play();
			}
			#endregion
			
			SetWeight(b, doptions[selection]);
			Recellection.CurrentState = WorldView.Instance;
			MenuController.UnloadMenu();
		}
		
		/// <summary>
		/// Gets the weight of a fromBuilding.
		/// </summary>
		/// <param name="fromBuilding">The fromBuilding to get the weight for.</param>
		/// <returns>The weight of the fromBuilding.</returns>
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

				// Figure out the unit balance for each fromBuilding
				Queue<BuildingBalance> inNeed = new Queue<BuildingBalance>();
				Queue<BuildingBalance> withExcess = new Queue<BuildingBalance>();
				logger.Debug("Figuring out the unit balancing for each building");
				foreach(Building b in g.GetBuildings())
				{
					float factor = (float)g.GetWeight(b) / (float)g.TotalWeight;
					int unitGoal = (int)(((float)totalUnits) * factor);
					int unitBalance = b.CountTotalUnits() - unitGoal;

					logger.Trace("Unit goal for " + b + " ((" + g.GetWeight(b) + " / " + g.TotalWeight + ") * " + totalUnits + ") is " + unitGoal + " which has " + b.CountTotalUnits() + " ("+b.CountUnits()+" here) units. Balance = " + unitBalance + ".");
					if (unitBalance > 0)
					{
						logger.Trace("Building has extra units to give.");
						withExcess.Enqueue(new BuildingBalance(b, unitBalance));
					}
					else if (unitBalance < 0)
					{
						logger.Trace("Building is in need of units. But there are "+b.incomingUnits.Count+" units on their way.");
						unitBalance += b.incomingUnits.Count;
						if (unitBalance < 0)
						{
							logger.Trace("Building is still in need of units.");
							inNeed.Enqueue(new BuildingBalance(b, unitBalance));
						}
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
                    continue;
				}
				
				// If there is nothing to give, don't balance.
				if (withExcess.Count == 0)
				{
					logger.Debug("There is nothing to give, don't balance.");
					continue;
				}

				// Try to even out the unit count in every fromBuilding
				logger.Debug("Trying to even out the unit count in every building.");
				bool balancingIsPossible = inNeed.Count > 0 && withExcess.Count > 0;
				while (balancingIsPossible)
				{
					BuildingBalance want = inNeed.Peek();
					BuildingBalance has = withExcess.Peek();
					int transferableUnits = Math.Min(has.balance, Math.Abs(want.balance));

					logger.Trace("Transferring units "+transferableUnits+" from " + has.building + " to " + want.building + ".");
					
					has.balance -= transferableUnits;
					want.balance += transferableUnits;
					
					MoveUnits(transferableUnits, has.building, want.building);
					
					if (has.balance == 0)
					{
						logger.Trace("Having building "+has.building+" is now satisfied. Removing from balancing.");
						withExcess.Dequeue();
					}
					
					if (want.balance == 0)
					{
						logger.Trace("Wanting building "+want.building + " is now satisfied. Removing from balancing.");
						inNeed.Dequeue();
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
				totalUnits += b.CountTotalUnits();
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
		/// <param name="from">From what fromBuilding to move.</param>
		/// <param name="to">Destination for the units.</param>
		private void MoveUnits(int numberOfUnits, Building from, Building to)
		{
			UnitController.MoveUnits(numberOfUnits, from.controlZone.First.Value, to.controlZone.First.Value);
		}
	}

	/// <summary>
	/// Exception for when a fromBuilding does not appear to belong to a graph.
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
