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
    /// to the player.
    /// </summary>
    class WorldView
    {
        private Player player;
        public WorldView(World world, Player player)
        {
            this.player = player;
            world.MapEvent += OnMapEvent;
            world.TileEvent += OnTileEvent;
        }

        public void OnMapEvent(Object o, Event<Tile[,]> ev)
        {

        }

        public void OnTileEvent(Object o, Event<Tile> ev)
        {

        }
    }
}
