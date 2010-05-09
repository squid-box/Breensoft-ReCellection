using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Recellection.Code.Utility.Events;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Recellection.Code.Utility.Logger;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Recellection.Code.Views
{
    /// <summary>
    /// The purpose of the World View is to provide the necessary data to render the game
    /// screen as described i the SRD 3.3. It also stores the information of the game state available
    /// to the player. The World View contains the information that is relevant to a single player, and
    /// therefore has a reference to a Player-object.
    /// </summary>
    class WorldView : IView
    {
        public Logger myLogger;
        private List<Tile> tilecollection;

        public World World { get; private set; }

        public WorldView(World world)
        {
            this.World = world;
            world.MapEvent += OnMapEvent;
            world.GetMap().TileEvent += OnTileEvent;
            myLogger = LoggerFactory.GetLogger();
            myLogger.Info("Created a WorldView.");

            this.World.lookingAt = this.World.players[0].GetGraphs()[0].baseBuilding.coordinates;

            this.tilecollection = CreateCurrentView();
        }

        private List<Tile> CreateCurrentView()
        {
            // First, add all tiles from the map:
            myLogger.Info("Getting tiles from World.map.");
            Tile[,] tiles = this.World.map.map;

            List<Tile> tileCollection = new List<Tile>();
            myLogger.Info("I'm going to start working on those tiles now...");

            int currentX = (int)this.World.lookingAt.X;
            int currentY = (int)this.World.lookingAt.Y;

            for (int i = currentX; i < Globals.VIEWPORT_WIDTH / Globals.TILE_SIZE; i++)
            {
                for (int j = currentY; j < Globals.VIEWPORT_HEIGHT / Globals.TILE_SIZE; j++)
                {
                    tileCollection.Add(tiles[i,j]);
                }
            }

            myLogger.Info("Done with the Tiles!");

            return tileCollection;
        }

        /// <summary>
        /// Defines the action to be taken when the MapEvent is invoked in World.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ev"></param>
        [Obsolete]
        public void OnMapEvent(Object o, Event<World.Map> ev)
        {
            this.World.map = ev.subject; 
        }

        [Obsolete]
        public void OnTileEvent(Object o, Event<Tile> ev)
        {
            
        }

        /// <summary>
        /// I have no idea what this is supposed to do.
        /// </summary>
        [Obsolete]
        public void UpdateScreen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// I have no idea what this is supposed to do.
        /// </summary>
        [Obsolete]
        public void UpdateMapMatrix()
        {
            throw new NotImplementedException();
        }

		override public void Draw(SpriteBatch spriteBatch)
		{
			foreach(Tile t in tilecollection)
            {
                this.drawTexture(spriteBatch, Recellection.textureMap.GetTexture(t.GetTerrainType().GetEnum()), t.GetRectangle());
            }
		}
		override public void Update(GameTime passedTime)
		{
            this.tilecollection = CreateCurrentView();
            
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                this.World.lookingAt = new Vector2(this.World.lookingAt.X-1, this.World.lookingAt.Y);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                this.World.lookingAt = new Vector2(this.World.lookingAt.X + 1, this.World.lookingAt.Y);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                this.World.lookingAt = new Vector2(this.World.lookingAt.X, this.World.lookingAt.Y+1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                this.World.lookingAt = new Vector2(this.World.lookingAt.X, this.World.lookingAt.Y - 1);
            }
		}
	}
}
