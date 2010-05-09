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
            myLogger = LoggerFactory.GetLogger();
            myLogger.Info("Created a WorldView.");

            this.World.lookingAtEvent += CreateCurrentView;

            //this.World.LookingAt = this.World.players[0].GetGraphs()[0].baseBuilding.coordinates;
            this.World.LookingAt = new Vector2(0, 0);
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

            for (int i = currentX; i <= Globals.VIEWPORT_WIDTH / Globals.TILE_SIZE; i++)
            {
                for (int j = currentY; j <= Globals.VIEWPORT_HEIGHT / Globals.TILE_SIZE; j++)
                {
                    tileCollection.Add(tiles[i,j]);
                }
            }

            myLogger.Info("Done with the Tiles!");
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
			Building b;
            foreach(Tile t in tileCollection)
            {
                int x = (int) (t.position.X - (128 * World.LookingAt.X));
                int y = (int) (t.position.Y - (128 * World.LookingAt.Y));

                Rectangle r = new Rectangle(x, y, Globals.TILE_SIZE, Globals.TILE_SIZE);
                this.drawTexture(spriteBatch, Recellection.textureMap.GetTexture(t.GetTerrainType().GetEnum()), r);
                b = t.GetBuilding();
                if (b != null)
                {
                    x = (int)t.position.X;
                    y = (int)t.position.Y;
                    this.drawTexture(spriteBatch, b.GetSprite(), new Rectangle(x * 128 + 32, y * 128 + 32, b.GetSprite().Width, b.GetSprite().Height));
                }
            }
		}
		override public void Update(GameTime passedTime)
		{
            KeyboardState ks = Keyboard.GetState();

            float f = 0.5f;
            
            if (ks.IsKeyDown(Keys.Left))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X-f, this.World.LookingAt.Y);
                if (this.World.LookingAt.X < 0)
                {
                    this.World.LookingAt = new Vector2(0, this.World.LookingAt.Y);
                }
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X + f, this.World.LookingAt.Y);
                if (this.World.LookingAt.X > this.World.map.Cols-8)
                {
                    this.World.LookingAt = new Vector2(this.World.map.Cols, this.World.LookingAt.Y-8);
                }
            }
            if (ks.IsKeyDown(Keys.Up))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X, this.World.LookingAt.Y+f);
                if (this.World.LookingAt.Y > this.World.map.Rows-8)
                {
                    this.World.LookingAt = new Vector2(this.World.LookingAt.X, this.World.map.Rows-8);
                }
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X, this.World.LookingAt.Y - f);
                if (this.World.LookingAt.Y < 0)
                {
                    this.World.LookingAt = new Vector2(this.World.LookingAt.X, 0);
                }
            }
		}
	}
}
