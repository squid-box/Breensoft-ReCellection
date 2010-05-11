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

        private Texture2D backgroundTex;

        public World World { get; private set; }

        public WorldView(World world)
        {
            this.World = world;
            myLogger = LoggerFactory.GetLogger();
            myLogger.Info("Created a WorldView.");

            //To make sure the lookingAt in world would make the world view draw tiles that does not exists align it.
            alignViewport();

            this.World.lookingAtEvent += CreateCurrentView;

            //this.World.LookingAt = new Vector2(0, 0);
            CreateCurrentView(this, new Event<Vector2>(this.World.LookingAt,EventType.ALTER));

        }

        private void CreateCurrentView(Object o, Event<Vector2> ev)
        {
            // First, add all tiles from the map:
            myLogger.Info("Getting tiles from World.map.");
            Tile[,] tiles = this.World.map.map;

            //Vector2 copyLookingAt = alignViewport(ev.subject);

            tileCollection = new List<Tile>();

            int currentX = (int)this.World.LookingAt.X;
            int currentY = (int)this.World.LookingAt.Y;

            myLogger.Trace("Rendering for X:" + currentX + " and Y:" + currentY + ".");

            myLogger.Trace("Width:" + (Globals.VIEWPORT_WIDTH / Globals.TILE_SIZE) + " and Height:" + (Globals.VIEWPORT_HEIGHT / Globals.TILE_SIZE) + ".");
            for (int i = currentX; i <= (Globals.VIEWPORT_WIDTH / Globals.TILE_SIZE) + currentX; i++)
            {
                for (int j = currentY; j <= (Globals.VIEWPORT_HEIGHT / Globals.TILE_SIZE) + currentY; j++)
                {
                       tileCollection.Add(tiles[i,j]);
                }
            }
        }

        private void alignViewport()
        {
            if (this.World.LookingAt.X < 0)
            {
                this.World.LookingAt = new Vector2(0, this.World.LookingAt.Y);
            }

            if (this.World.LookingAt.X >= this.World.map.Rows - (Recellection.viewPort.Width / Globals.TILE_SIZE) -1)
            {
                this.World.LookingAt = new Vector2(
                    this.World.map.Rows - (Recellection.viewPort.Width / Globals.TILE_SIZE) - 1,
                    this.World.LookingAt.Y);
            }

            if (this.World.LookingAt.Y >= this.World.map.Cols - (Recellection.viewPort.Height / Globals.TILE_SIZE) -1)
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X,
                    this.World.map.Cols - (Recellection.viewPort.Height / Globals.TILE_SIZE) - 1);
            }

            if (this.World.LookingAt.Y < 0)
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X, 0);
            }
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
            #region THIS IS BACKGROUNDDRAWAGE!

            //spriteBatch.Draw(backgroundTex, new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height), Color.White);

            #endregion

            Building b;
            foreach(Tile t in tileCollection)
            {
                int x = (int) (t.position.X - (World.LookingAt.X));
                int y = (int) (t.position.Y - (World.LookingAt.Y));

                Rectangle r = new Rectangle(x*Globals.TILE_SIZE, y*Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);
                this.drawTexture(spriteBatch, Recellection.textureMap.GetTexture(t.GetTerrainType().GetEnum()), r);
                
                // Building? On my Tile?! It's more likely than you think.
                b = t.GetBuilding();
                if (b != null)
                {
                    myLogger.Info("Found a building on the tile.");
                    this.drawTexture(spriteBatch, b.GetSprite(),
                        new Rectangle(x * Globals.TILE_SIZE + 32, y * Globals.TILE_SIZE + 32, b.GetSprite().Width, b.GetSprite().Height),
                        b.owner.color);
                }

                // Find those units!
                HashSet<Unit> units = t.GetUnits();
                if (units.Count != 0)
                {
                    myLogger.Info("Found unit(s) on the tile.");
                    foreach (Unit u in units)
                    {
                        this.drawTexture(spriteBatch, u.GetSprite(), new Rectangle(x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, u.GetSprite().Width, u.GetSprite().Height), u.GetOwner().color);
                    }
                }
            }
		}
        override public void Update(GameTime passedTime)
        {
            KeyboardState ks = Keyboard.GetState();

            float f = 0.1f;

            if (ks.IsKeyDown(Keys.X))
            {
                this.World.LookingAt = this.World.players[0].GetGraphs()[0].baseBuilding.coordinates;
            }

            if (ks.IsKeyDown(Keys.Left))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X - f, this.World.LookingAt.Y);
                if (this.World.LookingAt.X < 0)
                {
                    this.World.LookingAt = new Vector2(0, this.World.LookingAt.Y);
                }
            }
            if (ks.IsKeyDown(Keys.Right))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X + f, this.World.LookingAt.Y);
                if (this.World.LookingAt.X > this.World.map.Cols - 18)
                {
                    this.World.LookingAt = new Vector2(this.World.map.Cols, this.World.LookingAt.Y);
                }
            }
            if (ks.IsKeyDown(Keys.Down))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X, this.World.LookingAt.Y + f);
                if (this.World.LookingAt.Y > this.World.map.Rows - 12)
                {
                    this.World.LookingAt = new Vector2(this.World.LookingAt.X, this.World.map.Rows - 12);
                }
            }
            if (ks.IsKeyDown(Keys.Up))
            {
                this.World.LookingAt = new Vector2(this.World.LookingAt.X, this.World.LookingAt.Y - f);
                if (this.World.LookingAt.Y < 0)
                {
                    this.World.LookingAt = new Vector2(this.World.LookingAt.X, 0);
                }
            }
        }

        public void RenderToTex(SpriteBatch spriteBatch)
        {
            RenderTarget2D backgroundTarget = new RenderTarget2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width, Recellection.viewPort.Height, 1, Recellection.graphics.GraphicsDevice.DisplayMode.Format);

            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, backgroundTarget);
            Recellection.graphics.GraphicsDevice.Clear(Color.White);

            #region INSERT TILE DRAWING HEAR!!!!11
            #endregion

            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);
            backgroundTex = backgroundTarget.GetTexture();
        }
	}
}
