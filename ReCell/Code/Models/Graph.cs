namespace Recellection.Code.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using global::Recellection.Code.Controllers;

    using global::Recellection.Code.Utility.Events;

    using global::Recellection.Code.Utility.Logger;

    /// <summary>
	/// The Graph component is a storage class for buildings and their weights.
	/// 
	/// Author: Martin Nycander
    /// Signature: John Forsberg (2010-04-20)
    /// Signature: Marco Ahumada Juntune (2010-05-06)
	/// </summary>
	public class Graph : IModel
	{
        #region Static Fields

        private static readonly Logger logger = LoggerFactory.GetLogger();
		private static int defaultWeight = 0;

        #endregion

        #region Fields

        private readonly Dictionary<Building, int> buildings;

        #endregion

        #region Constructors and Destructors

        /// <summary>
		/// Constructs and initializes a graph with a single basebuilding.
		/// </summary>
		public Graph(BaseBuilding baseBuilding) : this()
		{
			this.buildings.Add(baseBuilding, defaultWeight);
            this.baseBuilding = baseBuilding;
			this.TotalWeight += defaultWeight;
		}

        private Graph()
        {
            logger.Trace("Constructing new graph.");
            this.buildings = new Dictionary<Building, int>();
            this.TotalWeight = 0;
        }

        #endregion

        #region Public Events

        public event Publish<Building> weightChanged;

        #endregion

        #region Public Properties

        public int TotalWeight { get; private set; }

        public BaseBuilding baseBuilding { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
		/// Adds a fromBuilding to the graph.
		/// A fromBuilding that has already been added will be ignored.
		/// </summary>
		/// <param name="fromBuilding">The fromBuilding to add.</param>
		/// <exception cref="ArgumentException">If the fromBuilding is a base fromBuilding.</exception>
		public void Add(Building building)
		{
			if (this.buildings.ContainsKey(building))
			{
				logger.Debug("Can not add building to graph. The building '" + building + "' already exists.");
				throw new ArgumentException("The building '" + building + "' already exists in a graph.");
			}
			
			lock(this.buildings)
			{
				this.buildings.Add(building, defaultWeight);
				this.TotalWeight += defaultWeight;
			}

			this.Publish(building, defaultWeight, EventType.ADD);
		}

        /// <returns>The number of buildings in this graph.</returns>
        public int CountBuildings()
        {
            return this.buildings.Count();
        }
		

        /// <returns>An enumerator for all buildings in the graph.</returns>
        public IEnumerable<Building> GetBuildings()
        {
            lock(this.buildings)
            {
                foreach(KeyValuePair<Building, int> b in this.buildings)
                {
                    yield return b.Key;
                }
            }
        }

        /// <param name="fromBuilding">The fromBuilding to get weight for.</param>
		/// <returns>the weight of the fromBuilding.</returns>
		/// <exception cref="ArgumentException">if the fromBuilding is not a part of the graph.</exception>
		public int GetWeight(Building building)
		{
			int weight;
			
			if (! this.buildings.TryGetValue(building, out weight))
			{
				throw new ArgumentException("That building does not exist in this graph.");
			}
			
			return weight;
		}
		

		/// <summary>
		/// Returns the weight factor for a fromBuilding. 
		/// It' pretty much weight / total weight.
		/// </summary>
		/// <param name="fromBuilding"></param>
		/// <returns></returns>
		public double GetWeightFactor(Building building)
		{
			double weight = this.GetWeight(building);
			return weight / this.TotalWeight;
		}
		

		/// <summary>
		/// Checks if a fromBuilding exists in this graph.
		/// </summary>
		/// <param name="b">The fromBuilding to check existance for.</param>
		/// <returns>True if the fromBuilding exists, false if not.</returns>
		public bool HasBuilding(Building b)
		{
			return this.buildings.ContainsKey(b);
		}

        /// <summary>
        /// Removes a fromBuilding from the graph.
        /// </summary>
        /// <param name="fromBuilding">The fromBuilding to remove.</param>
        public void Remove(Building building)
        {
            lock(this.buildings)
            {
                this.TotalWeight -= this.buildings[building];
                this.buildings.Remove(building);
            }
			
            this.Publish(building, 0, EventType.REMOVE);
        }

        /// <summary>
        /// Sets the weight of a fromBuilding node in the graph.
        /// </summary>
        /// <param name="fromBuilding">The fromBuilding to set weight for.</param>
        /// <param name="weight">The new weight.</param>
        /// <exception cref="GraphLessBuildingException">If the fromBuilding does not exist in the graph.</exception>
        public void SetWeight(Building building, int weight)
        {
            if (! this.buildings.ContainsKey(building))
            {
                throw new GraphLessBuildingException();
            }
			
            lock(this.buildings)
            {
                this.TotalWeight -= this.buildings[building];
                this.buildings[building] = weight;
                this.TotalWeight += weight;
            }
			
            this.Publish(building, weight, EventType.ALTER);
        }

        #endregion

        #region Methods

        /// <summary>
		/// Publishes an event of change to all subscribers.
		/// </summary>
		/// <param name="fromBuilding">The fromBuilding that has changed.</param>
		/// <param name="weight">The weight of that fromBuilding.</param>
		/// <param name="t">Type of event.</param>
		private void Publish(Building building, int weight, EventType t)
		{
			if (this.weightChanged != null)
			{
				this.weightChanged(this, new GraphEvent(building, weight, t));
			}
		}

        #endregion
	}
}
