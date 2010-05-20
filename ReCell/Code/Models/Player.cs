using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Controllers;

namespace Recellection.Code.Models
{
    /// <summary>
    /// Enum with available player colors
    /// </summary>
    public enum PlayerColour { RED, BLUE , GREEN , YELLOW , PURPLE }

    /// <summary>
    /// Class representing a player in the world. Holds the buildings networks owned by the player
    /// and some game related information.
    /// </summary>
    public class Player : IModel
    {
        /// <summary>
        /// The name of the player
        /// </summary>
        private string name;

        /// <summary>
        /// The color of the player
        /// </summary>
        public PlayerColour colour {get; private set;}

        public Color color { get; private set; }
        public float PowerLevel { get; set; }
		public float SpeedLevel { get; set; }
        public UnitAccountant unitAcc { get; set; }
        
        public Player Enemy { get; set; }
        
        /// <summary>
        /// The fromBuilding networks owned by a player
        /// </summary>
        private List<Graph> graphs;

        private HashSet<Unit> units;

        /// <summary>
        /// Initializes a player with the given colour and name
        /// </summary>
        /// <param name="colour">The colour associated with the player</param>
        /// <param name="name">The name of the player</param>
        public Player(PlayerColour colour, string name)
        {
            this.name = name;
            this.colour = colour;

            this.graphs = new List<Graph>();
            this.units = new HashSet<Unit>();
        }

        public Player(Color color, string name)
        {
            this.name = name;
            this.color = color;
            this.unitAcc = new UnitAccountant(this);
            this.graphs = new List<Graph>();
            this.units = new HashSet<Unit>();
        }

        /// <summary>
        /// Construct a default player for testing purposes. DO NOT use in game.
        /// </summary>
        public Player()
        {
            this.name = "Vict0r Turner, aka John Doe";
            this.colour = PlayerColour.PURPLE;
            this.graphs = new List<Graph>();
            this.unitAcc = new UnitAccountant(this);
        }

        /// <summary>
        /// Retrieve the buildings networks 
        /// </summary>
        /// <returns></returns>
        public List<Graph> GetGraphs()
        {
            return graphs;
        }

        /*public Color GetColor()
        {
            switch (colour)
            {
                case PlayerColour.RED:
                    return Color.Red;
                case PlayerColour.BLUE:
                    break;
                case PlayerColour.GREEN:
                    break;
                case PlayerColour.YELLOW:
                    break;
                case PlayerColour.PURPLE:
                    break;

            }
        }*/

        public void AddGraph(Graph g)
        {
            if (!graphs.Contains(g))
            {
                graphs.Add(g);
            }
        }

        /// <summary>
        /// This method calculates how many buildings of the specified type the player have.
        /// </summary>
        /// <param name="type">The type of building to count</param>
        public uint CountBuildingsOfType(Globals.BuildingTypes type)
        {
            uint retur = 0;
            foreach (Graph g in graphs)
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

        public uint CountUnits()
        {
            return (uint)units.Count;
        }


        public void AddUnit(Unit u)
        {
            lock (units)
            {
                units.Add(u);
            }

        }

        public void AddUnits(List<Unit> units)
        {
            lock (this.units)
            {
                foreach (Unit u in units)
                {
                    this.units.Add(u);
                }
            }
        }

        public void RemoveUnit(Unit u)
        {
            lock (units)
            {
                this.units.Remove(u);
            }
        }

        public void RemoveUnits(List<Unit> units)
        {
            lock (units)
            {
                foreach(Unit u in units)
                {
                    this.units.Remove(u);
                }
            }
        }
    }
}
