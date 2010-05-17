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
using Recellection.Code.Utility.Console;
using Recellection.Code.Controllers;

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
		private Texture2D activeTile;
        private float calmRippleLowerBound = 1.5f;
        private float calmRippleWaveDivider;
        private float calmRippleMovementRate = 0.01f;
        private float calmRippleDistortion = 0.5f;
        private float crawler = 0;
        private LightParticleSystem lps;
		private Tile active;

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
            //myLogger.SetTarget(LoggerSetup.GetLogFileTarget("WorldView.log"));
            myLogger.SetThreshold(LogLevel.FATAL);
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
		
		private Vector2 TileToPixels(Vector2 tileCoords)
		{
			Vector2 pixelCoords = new Vector2();
			Vector2.Multiply(ref tileCoords, (float)Globals.TILE_SIZE, out pixelCoords);
			return pixelCoords;
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
			Layer = 1.0f;
			drawTexture(spriteBatch, backgroundTex, new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));
            #endregion
			
            lock (tileCollection)
			{
				Layer = 0.75f;
				// Go through each players graphs and draw lines between buildings.
				foreach (Player p in World.players)
				{
					List<Graph> graphs = p.GetGraphs();
					lock (graphs)
					{
						foreach (Graph g in graphs)
						{
							BaseBuilding bb = g.baseBuilding;
							
							if (bb == null)
							{
								// Graph is broken :(
								continue;
							}
							
							foreach(Building b in g.GetBuildings())
							{
								if (b == bb)
									continue;
								Vector2 lookAt = new Vector2(World.LookingAt.X, World.LookingAt.Y);
								myLogger.Info("Drawing line between "+bb+" and "+b+".");
								DrawLine(spriteBatch, TileToPixels(bb.position-lookAt), TileToPixels(b.position-lookAt), 
											new Color(b.owner.color, 80), 10);
							}
						}
					}
				}
				
                foreach (Tile t in tileCollection)
                {
                    // Building? On my Tile?! It's more likely than you think.
                    Building b = t.GetBuilding();
                    if (b != null)
                    {
                        myLogger.Info("Found a building on the tile.");
						this.Layer = 0.0f;
						Texture2D spr = b.GetSprite();
						int bx = (int)Math.Round((b.position.X - World.LookingAt.X) * Globals.TILE_SIZE) - spr.Width/2;
						int by = (int)Math.Round((b.position.Y - World.LookingAt.Y) * Globals.TILE_SIZE) - spr.Height/2;
						this.drawTexture(spriteBatch, spr,
							new Rectangle(bx, by, spr.Width, spr.Height),
                            b.owner.color);
                    }

                    // Find those units!
                    HashSet<Unit> units = t.GetUnits();
                    if (units.Count != 0)
                    {
                        myLogger.Info("Found unit(s) on the tile.");
                        foreach (Unit u in units)
                        {
                            this.Layer = 0.5f;
                            Texture2D spr = u.GetSprite();
							int ux = (int)Math.Round((u.position.X - World.LookingAt.X) * Globals.TILE_SIZE) - spr.Width/2;
							int uy = (int)Math.Round((u.position.Y - World.LookingAt.Y) * Globals.TILE_SIZE) - spr.Height/2;
							
							Color c = u.GetOwner().color;
							if (u.powerLevel > 1f)
							{
								c = Color.Lerp(c, Color.HotPink, 0.5f);
							}
                            this.drawTexture(spriteBatch, spr, new Rectangle(ux, uy, spr.Width, spr.Height), u.GetOwner().color);
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

					if (t.active)
					{
						spriteBatch.Draw(Recellection.textureMap.GetTexture(Globals.TextureTypes.ActiveTile), r, Color.White);
					}
					Building b = t.GetBuilding();
					if (b != null)
					{
						float fontX, fontY;
						Vector2 stringSize;
						string infosz;

						infosz = b.GetUnits().Count.ToString();
						stringSize = Recellection.worldFont.MeasureString(infosz);
						fontX = (float)(r.X + r.Width/2) - stringSize.X/2;
						fontY = (float)(r.Y + r.Height/4) - stringSize.Y;
						spriteBatch.DrawString(Recellection.worldFont, infosz, new Vector2(fontX, fontY), Color.Black);

						infosz = GraphController.Instance.GetWeight(b).ToString();
						stringSize = Recellection.worldFont.MeasureString(infosz);
						fontX = (float)(r.X + r.Width / 2) - stringSize.X / 2;
						fontY = (float)(r.Y + 3*r.Height / 4) - stringSize.Y;
						spriteBatch.DrawString(Recellection.worldFont, infosz, new Vector2(fontX, fontY), Color.Black);


					}
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
