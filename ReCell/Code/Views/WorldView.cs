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
    /// screen as described x the SRD 3.3. It also stores the information of the game state available
    /// to the player. The World View contains the information that is relevant to a single player, and
    /// therefore has a reference to a Player-object.
    /// </summary>
    public class WorldView : IView
    {
        public Logger myLogger;
        private List<Tile> tileCollection;

        private Texture2D backgroundTex = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);

        public static bool doLights = false;
        public static bool doGrain = false;
        public static bool doRipples = false;
        
        private Effect bgShaders;
        private float calmRippleLowerBound = 1.5f;
        private float calmRippleWaveDivider;
        private float calmRippleMovementRate = 0.01f;
        private float calmRippleDistortion = 0.5f;
        private float crawler = 0;
        private LightParticleSystem lps;

		private static int maxCols = (int)((float)Recellection.viewPort.Width / (float)Globals.TILE_SIZE);
		private static int maxRows = (int)((float)Recellection.viewPort.Height / (float)Globals.TILE_SIZE);
		
        public World World { get; private set; }

		public static WorldView Instance { get; set; }

        public WorldView(World world)
        {
            lps = new LightParticleSystem(0.05f, Recellection.textureMap.GetTexture(Globals.TextureTypes.Light));

            this.bgShaders = Recellection.bgShader;

            this.World = world;
            myLogger = LoggerFactory.GetLogger();
            myLogger.SetThreshold(LogLevel.ERROR);
            myLogger.Info("Created a WorldView.");

            //To make sure the lookingAt in world would make the world view draw tiles that does not exists align it.
            alignViewport();

            tileCollection = new List<Tile>();

            this.World.lookingAtEvent += CreateCurrentView;

            //this.World.LookingAt = new Vector2(0, 0);
            CreateCurrentView(this, new Event<Point>(this.World.LookingAt,EventType.ALTER));

			Instance = this;
        }

        private void CreateCurrentView(Object o, Event<Point> ev)
        {
            // First, add all tiles from the map:
            myLogger.Info("Getting tiles from World.map.");
            Tile[,] tiles = this.World.map.map;

            //Vector2 copyLookingAt = alignViewport(ev.subject);

            lock (tileCollection)
            {
                tileCollection.Clear();

                int currentX = (int)this.World.LookingAt.X;
                int currentY = (int)this.World.LookingAt.Y;

                myLogger.Info("Rendering for X:" + currentX + " and Y:" + currentY + ".");
                myLogger.Info("Width:" + maxCols + " and Height:" + maxRows + ".");
                for (int x = currentX; x < currentX + maxCols; x++)
                {
                    for (int y = currentY; y < currentY + maxRows; y++)
                    {
                        try
                        {
                            tileCollection.Add(tiles[x, y]);
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            myLogger.Fatal("OMG FAIL");
                        }
                    }
                }
            }
        }

        private void alignViewport()
        {
            if (this.World.LookingAt.X < 0)
            {
                this.World.LookingAt = new Point(0, this.World.LookingAt.Y);
            }

            if (this.World.LookingAt.X >= this.World.map.width - (Recellection.viewPort.Width / Globals.TILE_SIZE) -1)
            {
				this.World.LookingAt = new Point(
                    this.World.map.height - (Recellection.viewPort.Width / Globals.TILE_SIZE) - 1,
                    this.World.LookingAt.Y);
            }

            if (this.World.LookingAt.Y >= this.World.map.width - (Recellection.viewPort.Height / Globals.TILE_SIZE) - 1)
            {
				this.World.LookingAt = new Point(this.World.LookingAt.X,
                    this.World.map.width - (Recellection.viewPort.Height / Globals.TILE_SIZE) - 1);
            }

            if (this.World.LookingAt.Y < 0)
            {
				this.World.LookingAt = new Point(this.World.LookingAt.X, 0);
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

			//Texture2D back = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);
			//Layer = 1.0f;
			drawTexture(spriteBatch, backgroundTex, new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));
            #endregion
			
            Building b;
            lock (tileCollection)
            {
                foreach (Tile t in tileCollection)
                {/*
                    int x = (int)(t.position.X - (World.LookingAt.X));
                    int y = (int)(t.position.Y - (World.LookingAt.Y));

                    Rectangle r = new Rectangle(x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);
                    this.Layer = 0.9f;
                    this.drawTexture(spriteBatch, t.GetSprite() , r);
                    */
                    // Building? On my Tile?! It's more likely than you think.
                    b = t.GetBuilding();
                    if (b != null)
                    {
                        myLogger.Info("Found a building on the tile.");
						this.Layer = 0.0f;
						int bx = (int)Math.Round((b.position.X - World.LookingAt.X) * Globals.TILE_SIZE);
						int by = (int)Math.Round((b.position.Y - World.LookingAt.Y) * Globals.TILE_SIZE);
                        this.drawTexture(spriteBatch, b.GetSprite(),
                            new Rectangle(bx, by, b.GetSprite().Width, b.GetSprite().Height),
                            b.owner.color);
                    }

                    // Go through each players graphs and draw lines between buildings.
                    foreach (Player p in World.players)
                    {
                        lock (p.GetGraphs())
                        {
                            foreach (Graph g in p.GetGraphs())
                            {
                                Building[] bob = g.GetBuildings().ToArray();
                                if (bob.Length < 1)
                                {
                                    for (int i = 1; i < bob.Length; i++)
                                    {
                                        /* Här tänkte jag ta och dra linjer
                                         * mellan varje byggnad i grafen.
                                         * But man, those lines.
                                         */
                                    }
                                }
                            }
                        }
                    }

                    // Find those units!
                    HashSet<Unit> units = t.GetUnits();
                    if (units.Count != 0)
                    {
                        myLogger.Info("Found unit(s) on the tile.");
                        foreach (Unit u in units)
                        {
                            this.Layer = 0.5f;
							int ux = (int)Math.Round((u.position.X - World.LookingAt.X) * Globals.TILE_SIZE);
							int uy = (int)Math.Round((u.position.Y - World.LookingAt.Y) * Globals.TILE_SIZE);
                            this.drawTexture(spriteBatch, u.GetSprite(), new Rectangle(ux, uy, u.GetSprite().Width, u.GetSprite().Height), u.GetOwner().color);
                        }
                    }
                }
            }
		}
        override public void Update(GameTime passedTime)
        {
            KeyboardState ks = Keyboard.GetState();

            int f = 1;

            if (ks.IsKeyDown(Keys.X))
            {
                World.LookingAt = new Point(
						(int)World.players[0].GetGraphs()[0].baseBuilding.position.X, 
						(int)World.players[0].GetGraphs()[0].baseBuilding.position.Y);
            }

			int x = World.LookingAt.X;
			int y = World.LookingAt.Y;

			if (ks.IsKeyDown(Keys.Left))
			{
				x -= f;
            }
            if (ks.IsKeyDown(Keys.Right))
			{
				x += f;
			}
			if (ks.IsKeyDown(Keys.Up))
			{
				y -= f;
			}
            if (ks.IsKeyDown(Keys.Down))
			{
				y += f;
            }
            
            x = (int)MathHelper.Clamp(x, 0, World.map.width - maxCols);
            y = (int)MathHelper.Clamp(y, 0, World.map.height - maxRows);
            
			this.World.LookingAt = new Point(x, y);
        }

        public void RenderToTex(SpriteBatch spriteBatch, GameTime gameTime)
        {
            RenderTarget2D backgroundTarget = new RenderTarget2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width, Recellection.viewPort.Height, 1, Recellection.graphics.GraphicsDevice.DisplayMode.Format);

            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, backgroundTarget);
            Recellection.graphics.GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            lock (tileCollection)
            {
                foreach (Tile t in tileCollection)
                {
                    int x = (int)(t.position.X - (World.LookingAt.X));
                    int y = (int)(t.position.Y - (World.LookingAt.Y));

                    Rectangle r = new Rectangle(x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);

                    spriteBatch.Draw(t.GetSprite(), r, Color.White);
                }
            }
            
            if(doLights)
                lps.UpdateAndDraw(gameTime, spriteBatch);

            spriteBatch.End();

            Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);
            backgroundTex = backgroundTarget.GetTexture();


            if (doGrain)
            {
                bgShaders.Parameters["Timer"].SetValue((float)gameTime.ElapsedRealTime.TotalSeconds + 1.23f);

                Recellection.graphics.GraphicsDevice.SetRenderTarget(0, backgroundTarget);
                Recellection.graphics.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                bgShaders.CurrentTechnique = bgShaders.Techniques["Grain"];
                bgShaders.Begin();
                bgShaders.CurrentTechnique.Passes[0].Begin();
                spriteBatch.Draw(backgroundTex, Vector2.Zero, Color.White);
                spriteBatch.End();
                bgShaders.CurrentTechnique.Passes[0].End();
                bgShaders.End();

                // Change back to the back buffer, and get our scene
                // as a texture.
                Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);
                backgroundTex = backgroundTarget.GetTexture();
            }

            if (doRipples)
            {
                crawler += calmRippleMovementRate;
                if (crawler > MathHelper.TwoPi)
                    crawler = 0;

                calmRippleWaveDivider = (float)Math.Pow(Math.Sin(crawler), 2) + calmRippleLowerBound;

                bgShaders.Parameters["calmWave"].SetValue((float)Math.PI / calmRippleWaveDivider);
                bgShaders.Parameters["calmDistortion"].SetValue(calmRippleDistortion);
                bgShaders.Parameters["calmCenterCoord"].SetValue(new Vector2(0.5f, 0.5f));

                Recellection.graphics.GraphicsDevice.SetRenderTarget(0, backgroundTarget);
                Recellection.graphics.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                bgShaders.CurrentTechnique = bgShaders.Techniques["Ripple"];
                bgShaders.Begin();
                bgShaders.CurrentTechnique.Passes[0].Begin();
                spriteBatch.Draw(backgroundTex, Vector2.Zero, Color.White);
                spriteBatch.End();
                bgShaders.CurrentTechnique.Passes[0].End();
                bgShaders.End();

                // Change back to the back buffer, and get our scene
                // as a texture.
                Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);
                backgroundTex = backgroundTarget.GetTexture();
            }
        }
	}
}
