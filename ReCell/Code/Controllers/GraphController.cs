namespace Recellection.Code.Controllers
{
    using System;
    using System.Collections.Generic;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Logger;

    using global::Recellection.Code.Views;

    /// <summary>
	/// TODO: Implement Dictionary instead of List for better performance.
	/// </summary>
	public class GraphController
	{
        #region Static Fields

        private static readonly Logger logger = LoggerFactory.GetLogger();

        private static GraphController instance;

        #endregion

        #region Fields

        /// <summary>A list of all the graph components.</summary>
        private readonly List<Graph> components;

        #endregion

        #region Constructors and Destructors

        /// <summary>
		/// Creates an empty graph controller.
		/// Nothing to it, really.
		/// </summary>
		protected GraphController()
		{
			this.components = new List<Graph>();
			logger.SetThreshold(LogLevel.ERROR);
			logger.SetTarget(LoggerSetup.GetLogFileTarget("graphcontrol.log"));
			logger.Info("Constructed a new GraphController.");
		}

        #endregion

        #region Public Properties

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

        #endregion

        #region Public Methods and Operators

        /// <summary>
		/// Adds a base fromBuilding. Base buildings forms their own graphs, and thus a new graph is created here.
		/// </summary>
		/// <param name="newBaseBuilding">The fresh base fromBuilding to create a graph from.</param>
        /// <param name="sourceBuilding">The fromBuilding using to build this, if the source fromBuilding
        /// has no base fromBuilding in its graph create a new one otherwise</param>
		/// <returns>The created graph.</returns>
		public Graph AddBaseBuilding(BaseBuilding newBaseBuilding, Building sourceBuilding)
		{
            if (sourceBuilding == null || sourceBuilding.baseBuilding.IsAlive())
            {
                return this.AddBaseBuilding(newBaseBuilding);
            }
            else
            {
                logger.Info("Added base building " + newBaseBuilding + " to a graph missing an alive base building.");
                Graph graph = this.GetGraph(sourceBuilding);
                graph.baseBuilding = newBaseBuilding;
                graph.Add(newBaseBuilding);
                foreach (Building b in graph.GetBuildings())
                {
                    if (b is ResourceBuilding)
                    {
                        graph.baseBuilding.RateOfProduction += ((ResourceBuilding)b).RateOfProduction;
                    }
                }

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
            var graph = new Graph(newBaseBuilding);
            this.components.Add(graph);
            return graph;
        }

        /// <summary>
        /// Adds a fromBuilding to a the same graph as another fromBuilding.
        /// </summary>
        /// <param name="source">The fromBuilding with a graph.</param>
        /// <param name="newBuilding">A new fromBuilding, not belonging to a graph yet.</param>
        public void AddBuilding(Building source, Building newBuilding)
        {
            logger.Info("Added building " + newBuilding + " with source " + source + ".");
            this.GetGraph(source).Add(newBuilding);
        }

        /// <summary>
        /// Calculates and carries out distribution of units in the graphs according to the buildings weights.
        /// 
        /// Warning: This method is not healthy. Not even for you. No. Dont.
        /// </summary>
        public void CalculateWeights()
        {
            logger.Info("\nCalculating weights for all graphs.");
            foreach(Graph g in this.components)
            {
                if (g.TotalWeight == 0)
                {
                    logger.Info("Ignoring graph, since there are no weights.");
                    continue;
                }

                logger.Debug("Calculating weights for graph " + g + ".");
                int totalUnits = SumUnitsInGraph(g);

                // Figure out the unit balance for each fromBuilding
                var inNeed = new Queue<BuildingBalance>();
                var withExcess = new Queue<BuildingBalance>();
                logger.Debug("Figuring out the unit balancing for each building");
                foreach (Building b in g.GetBuildings())
                {
                    float factor = g.GetWeight(b) / (float)g.TotalWeight;
                    var unitGoal = (int)(totalUnits * factor);
                    int unitBalance = b.CountUnits() - unitGoal;

                    logger.Trace(
                        "Unit goal for " + b + " ((" + g.GetWeight(b) + " / " + g.TotalWeight + ") * " + totalUnits
                        + ") is " + unitGoal + " which has " + b.CountTotalUnits() + " (" + b.CountUnits()
                        + " here) units. Balance = " + unitBalance + ".");
                    if (unitBalance > 0)
                    {
                        logger.Trace("Building has extra units to give.");
                        withExcess.Enqueue(new BuildingBalance(b, unitBalance));
                    }
                    else if (unitBalance < 0)
                    {
                        logger.Trace(
                            "Building is in need of units. But there are " + b.incomingUnits.Count
                            + " units on their way.");
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

                while (inNeed.Count > 0 && withExcess.Count > 0)
                {
                    BuildingBalance want = inNeed.Peek();
                    BuildingBalance has = withExcess.Peek();
                    int transferableUnits = Math.Min(has.balance, Math.Abs(want.balance));

                    logger.Trace(
                        "Transferring units " + transferableUnits + " from " + has.building + " to " + want.building
                        + ".");

                    has.balance -= transferableUnits;
                    want.balance += transferableUnits;

                    this.MoveUnits(transferableUnits, has.building, want.building);

                    if (has.balance == 0)
                    {
                        logger.Trace("Having building " + has.building + " is now satisfied. Removing from balancing.");
                        withExcess.Dequeue();
                    }

                    if (want.balance == 0)
                    {
                        logger.Trace(
                            "Wanting building " + want.building + " is now satisfied. Removing from balancing.");
                        inNeed.Dequeue();
                    }
                }

                logger.Trace("Done calculating weights for graph " + g + ".");
            }

            logger.Debug("Done calculating weights for all graphs.");
        }

        /// <summary>
        /// Gets the weight of a fromBuilding.
        /// </summary>
        /// <param name="fromBuilding">The fromBuilding to get the weight for.</param>
        /// <returns>The weight of the fromBuilding.</returns>
        public int GetWeight(Building building)
        {
            logger.Debug("Getting the weight for building " + building + ".");
            return this.GetGraph(building).GetWeight(building);
        }

        /// <summary>
        /// Removes a fromBuilding from its graph.
        /// </summary>
        /// <param name="fromBuilding">The fromBuilding to remove.</param>
        public void RemoveBuilding(Building building)
        {
            logger.Info("Removed building "+building+".");
            Graph graph = this.GetGraph(building);
            if (building is BaseBuilding)
            {
                graph.baseBuilding = null;
            }
            
            graph.Remove(building);

            if (graph.CountBuildings() == 0)
            {
                building.owner.GetGraphs().Remove(graph);
                this.components.Remove(graph);
            }
        }

        /// <summary>
		/// Sets the weight of a fromBuilding.
		/// </summary>
		/// <param name="fromBuilding">The fromBuilding to set the weight for.</param>
		/// <param name="weight">The new weight.</param>
		public void SetWeight(Building building, int weight)
		{
			logger.Debug("Setting weight "+weight+" to building "+building+".");
			this.GetGraph(building).SetWeight(building, weight);
		}
		

		/// <summary>
		/// Sets the weight of a fromBuilding by creating a menu and asking the user what weight the fromBuilding should have.
		/// </summary>
		/// <param name="b">The fromBuilding to set a weight for.</param>
		public void SetWeight(Building b)
		{
			var doptions = new Dictionary<MenuIcon, int>(8);

			doptions.Add(new MenuIcon(Language.Instance.GetString("NoPriority"), Recellection.textureMap.GetTexture(Globals.TextureTypes.NoPriority)), 0);
			doptions.Add(new MenuIcon(Language.Instance.GetString("LowPriority"), Recellection.textureMap.GetTexture(Globals.TextureTypes.LowPriority)), 50);
			doptions.Add(new MenuIcon(Language.Instance.GetString("HighPriority"), Recellection.textureMap.GetTexture(Globals.TextureTypes.HighPriority)), 100);
			doptions.Add(new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No)), -1);

			var menu = new Menu(Globals.MenuLayout.FourMatrix, 
							new List<MenuIcon>(doptions.Keys), 
							Language.Instance.GetString("SetImportance"));
			
			MenuController.LoadMenu(menu);
			
            Recellection.CurrentState = MenuView.Instance;

			MenuIcon selection = MenuController.GetInput();
			
			if (doptions[selection] >= 0)
			{
				this.SetWeight(b, doptions[selection]);
			}
			
			Recellection.CurrentState = WorldView.Instance;
			MenuController.UnloadMenu();
		}

        #endregion

        #region Methods

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

        /// <summary>
        /// Gets the graph of a fromBuilding by searching through all graphs.
        /// </summary>
        /// <param name="b">The fromBuilding to find the graph for.</param>
        /// <returns>The found graph.</returns>
        internal Graph GetGraph(Building b)
        {
            foreach(Graph g in this.components)
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

        /// <param name="numberOfUnits">The number of units to move.</param>
		/// <param name="from">From what fromBuilding to move.</param>
		/// <param name="to">Destination for the units.</param>
		private void MoveUnits(int numberOfUnits, Building from, Building to)
		{
			UnitController.MoveUnits(from.owner, from.controlZone.First.Value, to.controlZone.First.Value, numberOfUnits);
		}

        #endregion

        private struct BuildingBalance : IComparer<BuildingBalance>
        {
            #region Fields

            public readonly Building building;
            public int balance;

            #endregion

            #region Constructors and Destructors

            public BuildingBalance(Building b, int bal)
            {
                this.building = b;
                this.balance = bal;
            }

            #endregion

            #region Explicit Interface Methods

            int IComparer<BuildingBalance>.Compare(BuildingBalance bb1, BuildingBalance bb2)
            {
                return bb1.balance - bb2.balance;
            }

            #endregion
        }
	}

	/// <summary>
	/// Exception for when a fromBuilding does not appear to belong to a graph.
	/// This is serious enough to have its own exception.
	/// </summary>
	public class GraphLessBuildingException : Exception
	{
	    #region Static Fields

	    private static string msg = "A building does not belong to a graph. "+
									"All buildings should belong to a graph.";

	    #endregion

	    #region Constructors and Destructors

	    public GraphLessBuildingException() : base(msg)
		{
		}

	    #endregion
	}
}
