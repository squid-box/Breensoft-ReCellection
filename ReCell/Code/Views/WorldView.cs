using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Recellection.Code.Utility.Events;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Views
{
    /// <summary>
    /// The purpose of the World View is to provide the necessary data to render the game
    /// screen as described i the SRD 3.3. It also stores the information of the game state available
    /// to the player. The World View contains the information that is relevant to a single player, and
    /// therefore has a reference to a Player-object.
    /// </summary>
    class WorldView : IRenderable
    {
        public Logger myLogger;
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
            myLogger = LoggerFactory.GetLogger();
            myLogger.SetTarget(Console.Out);
            myLogger.Info("Created a WorldView.");
            this.Map = world.map;
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
        
        public List<DrawData> GetDrawData(ContentManager content)
        {
            List<DrawData> ret = new List<DrawData>();

            myLogger.Info("Getting tiles from World.map.");
            // First, add all tiles from the map:
            Tile[,] tiles = this.Map.map;
            myLogger.Info("Got tiles.");

            myLogger.Info("Iterating over tiles.");
            foreach (Tile t in tiles)
            {
                int x = (int) t.position.X;
                int y = (int) t.position.Y;
                ret.Add(new DrawData(Recellection.textureMap.GetTexture(t.GetTerrainType().GetEnum()),new Rectangle(x*Globals.TILE_SIZE, y*Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE)));
            }
            myLogger.Info("Tiles done..");

            return ret;
        }

        #endregion
    }
}
