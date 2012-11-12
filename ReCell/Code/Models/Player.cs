namespace Recellection.Code.Models
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    using global::Recellection.Code.Controllers;

    /// <summary>
    /// Enum with available player colors.
    /// </summary>
    public enum PlayerColour
    {
        /// <summary>
        /// Player color red.
        /// </summary>
        Red,

        /// <summary>
        /// Player color blue.
        /// </summary>
        Blue,

        /// <summary>
        /// Player color green.
        /// </summary>
        Green,

        /// <summary>
        /// Player color yellow.
        /// </summary>
        Yellow,

        /// <summary>
        /// Player color purple.
        /// </summary>
        Purple
    }

    /// <summary>
    /// Class representing a player in the world. Holds the buildings networks owned by the player
    /// and some game related information.
    /// </summary>
    public class Player : IModel
    {
        #region Fields

        /// <summary>
        /// The fromBuilding networks owned by a player
        /// </summary>
        private readonly List<Graph> graphs;

        /// <summary>
        /// The units owned by this player.
        /// </summary>
        private readonly HashSet<Unit> units;

        /// <summary>
        /// The name of the player
        /// </summary>
        private string name;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class with the given color and name. 
        /// </summary>
        /// <param name="colour">
        /// The Colour associated with the player
        /// </param>
        /// <param name="name">
        /// The name of the player
        /// </param>
        public Player(PlayerColour colour, string name)
        {
            this.name = name;
            this.Colour = colour;

            this.graphs = new List<Graph>();
            this.units = new HashSet<Unit>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="color">
        /// The color of the player.
        /// </param>
        /// <param name="name">
        /// The name of the player.
        /// </param>
        public Player(Color color, string name)
        {
            this.name = name;
            this.Color = color;
            this.UnitAcc = new UnitAccountant(this);
            this.graphs = new List<Graph>();
            this.units = new HashSet<Unit>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class. 
        /// Will construct a default player for testing purposes. DO NOT use in game.
        /// </summary>
        public Player()
        {
            this.name = "Vict0r Turner, aka John Doe";
            this.Colour = PlayerColour.Purple;
            this.graphs = new List<Graph>();
            this.UnitAcc = new UnitAccountant(this);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the enemy of this player.
        /// </summary>
        public Player Enemy { get; set; }

        /// <summary>
        /// Gets or sets the power level of this player.
        /// </summary>
        public float PowerLevel { get; set; }

        /// <summary>
        /// Gets or sets the speed level of this player.
        /// </summary>
        public float SpeedLevel { get; set; }

        /// <summary>
        /// Gets the color of this player?.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Gets the color of the player?
        /// </summary>
        public PlayerColour Colour { get; private set; }

        /// <summary>
        /// Gets or sets the unit accountant for this player.
        /// </summary>
        public UnitAccountant UnitAcc { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds a graph to this player.
        /// </summary>
        /// <param name="g">
        /// The graph.
        /// </param>
        public void AddGraph(Graph g)
        {
            if (!this.graphs.Contains(g))
            {
                this.graphs.Add(g);
            }
        }

        /// <summary>
        /// Adds a unit to this player.
        /// </summary>
        /// <param name="u">
        /// The unit to add.
        /// </param>
        public void AddUnit(Unit u)
        {
            lock (this.units)
            {
                this.units.Add(u);
            }
        }

        /// <summary>
        /// Adds a list of units to this player.
        /// </summary>
        /// <param name="recruits">
        /// The units to add.
        /// </param>
        public void AddUnits(List<Unit> recruits)
        {
            lock (this.units)
            {
                foreach (Unit u in recruits)
                {
                    this.units.Add(u);
                }
            }
        }

        /// <summary>
        /// This method calculates how many buildings of the specified type the player have.
        /// </summary>
        /// <param name="type">The type of building to count</param>
        /// <returns>Number of buildings of the specified type this player owns.</returns>
        public uint CountBuildingsOfType(Globals.BuildingTypes type)
        {
            uint retur = 0;
            foreach (Graph g in this.graphs)
            {
                foreach (Building b in g.GetBuildings())
                {
                    if (b.type == type)
                    {
                        retur += 1;
                    }
                }
            }

            return retur;
        }

        /// <summary>
        /// Counts the number of units this player owns.
        /// </summary>
        /// <returns>
        /// Number of units.
        /// </returns>
        public uint CountUnits()
        {
            return (uint)this.units.Count;
        }

        /// <summary>
        /// Retrieve the buildings networks.
        /// </summary>
        /// <returns>List of all the graphs owned by this player.</returns>
        public List<Graph> GetGraphs()
        {
            return this.graphs;
        }

        /// <summary>
        /// Removes a specified unit from this player.
        /// </summary>
        /// <param name="u">Unit to remove.</param>
        public void RemoveUnit(Unit u)
        {
            lock (this.units)
            {
                this.units.Remove(u);
            }
        }

        /// <summary>
        /// Removes a list of units from this player.
        /// </summary>
        /// <param name="corpses">List of units to remove.</param>
        public void RemoveUnits(List<Unit> corpses)
        {
            lock (corpses)
            {
                foreach (Unit u in corpses)
                {
                    this.units.Remove(u);
                }
            }
        }

        #endregion
    }
}
