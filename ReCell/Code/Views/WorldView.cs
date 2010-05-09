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
        private List<Tile> tileCollection;

        public World World { get; private set; }

        public WorldView(World world)
        {
            this.World = world;
            world.MapEvent += OnMapEvent;
            world.GetMap().TileEvent += OnTileEvent;
            myLogger = LoggerFactory.GetLogger();
            myLogger.Info("Created a WorldView.");

            this.World.lookingAtEvent += CreateCurrentView;

            this.World.LookingAt = this.World.players[0].GetGraphs()[0].baseBuilding.coordinates;

            CreateCurrentView(null, null);
        }

        private void CreateCurrentView(Object o, Event<Vector2> ev)
        {
            // First, add all tiles from the map:
            myLogger.Info("Getting tiles from World.map.");
            Tile[,] tiles = this.World.map.map;

            tileCollection = new List<Tile>();
            myLogger.Info("I'm going to start working on those tiles now...");

            int currentX = (int)this.World.LookingAt.X;
            int currentY = (int)this.World.LookingAt.Y;

            for (int i = currentX; i < Globals.VIEWPORT_WIDTH / Globals.TILE_SIZE; i++)
            {
                for (int j = currentY; j < Globals.VIEWPORT_HEIGHT / Globals.TILE_SIZE; j++)
                {
                    tileCollection.Add(tiles[i,j]);
                }
            }

            myLogger.Info("Done with the Tiles!");
        }

        /// <summary>
        /// Defines the action to be taken when the MapEvent is invoked in World.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="ev"></param>
        [Obsolete]
        public void OnMapEvent(Object o, Event<World.Map> ev)
        {
            //this.World.map = ev.subject; 
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
			foreach(Tile t in tileCollection)
            {
                this.drawTexture(spriteBatch, Recellection.textureMap.GetTexture(t.GetTerrainType().GetEnum()), t.GetRectangle());
            }
		}
		override public void Update(GameTime passedTime)
		{
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X-1, this.World.LookingAt.Y);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X + 1, this.World.LookingAt.Y);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X, this.World.LookingAt.Y+1);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X, this.World.LookingAt.Y - 1);
            }
		}
	}
}
