using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Recellection.Code.Utility.Events;

namespace Recellection.Code.Views
{
    /// <summary>
    /// The purpose of the World View is to provide the necessary data to render the game
    /// screen as described i the SRD 3.3. It also stores the information of the game state available
    /// to the player. The World View contains the information that is relevant to a single player, and
    /// therefore has a reference to a Player-object.
    /// </summary>
    class WorldView : IDrawable
    {
        /// <summary>
        /// The player whose view of the world this is.
        /// </summary>
        public Player Player { get; private set; }
        /// <summary>
        /// The map of the world being viewed
        /// </summary>
        public World.Map Map { get; private set; }

        // TODO Figure out the Rectangle class...
        // public Rect CurrentScreen { get; set; }

        public WorldView(World world, Player player)
        {
            this.Player = player;
            world.MapEvent += OnMapEvent;
            world.GetMap().TileEvent += OnTileEvent;
        }

        /// <summary>
        /// Defines the action to be taken when the MapEvent is invoked in World.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ev"></param>
        public void OnMapEvent(Object o, Event<World.Map> ev)
        {
            Map = ev.subject; 
        }

        public void OnTileEvent(Object o, Event<Tile> ev)
        {
            
        }

        /// <summary>
        /// I have no idea what this is supposed to do.
        /// </summary>
        public void UpdateScreen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// I have no idea what this is supposed to do.
        /// </summary>
        public void UpdateMapMatrix()
        {
            throw new NotImplementedException();
        }

        #region IDrawable Members
        
        public List<DrawData> GetDrawData()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
