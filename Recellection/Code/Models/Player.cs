using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection.Code.Models
{
    /// <summary>
    /// Enum with available player colors
    /// </summary>
    public enum PlayerColour {RED, BLUE, GREEN, YELLOW, PURPLE}

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
        protected PlayerColour colour {get; private set;}

        /// <summary>
        /// The building networks owned by a player
        /// </summary>
        private List<Graph> graphs;

        /// <summary>
        /// The level of upgrades of the player
        /// </summary>
        protected int upgradeLevel { get; private set; }

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
        }

        /// <summary>
        /// Construct a default player for testing purposes. DO NOT use in game.
        /// </summary>
        public Player()
        {
            this.name = "John doe";
            this.colour = PlayerColour.PURPLE;
            this.graphs = new List<Graph>();
        }

        /// <summary>
        /// Retrieve the buildings networks 
        /// </summary>
        /// <returns></returns>
        public List<Graph> GetGraphs()
        {
            return graphs;
        }

        public void AddGraph(Graph g)
        {
            if (!graphs.Contains(g))
            {
                graphs.Add(g);
            }
        }


    }
}
