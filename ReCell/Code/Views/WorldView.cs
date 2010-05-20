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

        private Texture2D backgroundTex;
        private RenderTarget2D backgroundTarget;

        private bool doRenderThisPass = true;
        public static bool doLights = true;
        public static bool doGrain = true;

        private Texture2D activeTile;
        private LightParticleSystem lps;
        private GrainSystem gs;
        private Tile active;
        private Color[,] cMatrix;

		private static int maxCols = (int)((float)Recellection.viewPort.Width / (float)Globals.TILE_SIZE);
		private static int maxRows = (int)((float)Recellection.viewPort.Height / (float)Globals.TILE_SIZE);
		
        public World World { get; private set; }

		public static WorldView Instance { get;	set; }

		public static void Initiate(World world)
		{
			if (Instance == null)
			{
				Instance = new WorldView();
			}
			Instance.World = world;
			world.lookingAtEvent += Instance.UpdateBg;
			world.lookingAtEvent += Instance.CreateCurrentView;
			Instance.CreateCurrentView(Instance, new Event<Point>(world.LookingAt, EventType.ALTER));
			Instance.alignViewport();


            //Color c1 = new Color(0xb2, 0xc9, 0x9f);
            //Color c2 = new Color(0x9f, 0xc4, 0xc9);
            Color c1 = Color.HotPink;//new Color(0xac, 0x33, 0x2d);
            Color c2 = Color.Crimson;//new Color(0xea, 0xe4, 0x7c);
            Instance.cMatrix = Instance.generateColorMatrix(c1, c2);
		}

        private WorldView()
        {
            backgroundTex = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);
            backgroundTarget = new RenderTarget2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width, Recellection.viewPort.Height, 1, Recellection.graphics.GraphicsDevice.DisplayMode.Format);
            lps = new LightParticleSystem(0.05f, Recellection.textureMap.GetTexture(Globals.TextureTypes.Light));
            gs = new GrainSystem(0.01f, 0.2f, 0.3f, Recellection.contentMngr);

            myLogger = LoggerFactory.GetLogger();
            //myLogger.SetTarget(LoggerSetup.GetLogFileTarget("WorldView.log"));
            myLogger.SetThreshold(LogLevel.FATAL);
            myLogger.Info("Created a WorldView.");

            tileCollection = new List<Tile>();

            //this.World.LookingAt = new Vector2(0, 0);
            

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
			Layer = 1.0f;
			DrawTexture(spriteBatch, backgroundTex, new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));
            #endregion
			
            lock (tileCollection)
			{
				Layer = 0.75f;
				// Go through each players graphs and draw lines between buildings.
				Vector2 lookAt = new Vector2(World.LookingAt.X, World.LookingAt.Y);
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

								Vector2 drawFrom = bb.position;
								if (b.Parent != null)
									drawFrom = b.Parent.position;
								
								myLogger.Info("Drawing line between "+bb+" and "+b+".");
								DrawLine(spriteBatch, TileToPixels(drawFrom-lookAt), TileToPixels(b.position-lookAt), 
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
                        lock (b)
                        {
                            myLogger.Info("Found a building on the tile.");
                            this.Layer = 0.1f;
                            Texture2D spr = b.GetSprite();
                            float size = 0.75f + 0.75f*Math.Min(100f, (float)GraphController.Instance.GetWeight(b))/100f;
                            int bx = (int)Math.Round((b.position.X - World.LookingAt.X) * Globals.TILE_SIZE) - (int)Math.Round((float)spr.Width * size) / 2;
                            int by = (int)Math.Round((b.position.Y - World.LookingAt.Y) * Globals.TILE_SIZE) - (int)Math.Round((float)spr.Height * size) / 2;
                            this.DrawTexture(spriteBatch, spr,
                                new Rectangle(bx, by, (int)Math.Round((float)spr.Width * size), (int)Math.Round((float)spr.Height * size)),
                                b.owner.color);

                            Vector2 xyhpr1 = new Vector2((float)((b.position.X - World.LookingAt.X) * Globals.TILE_SIZE) +14 -64, (float) Math.Round((b.position.Y - World.LookingAt.Y) * Globals.TILE_SIZE) +100);
                            Vector2 xyhpr2 = new Vector2((float)((b.position.X - World.LookingAt.X) * Globals.TILE_SIZE) + 114-64, (float)Math.Round((b.position.Y - World.LookingAt.Y) * Globals.TILE_SIZE) + 100);
                            Vector2 xyhpg2 = new Vector2((float)((b.position.X - World.LookingAt.X) * Globals.TILE_SIZE) + 14 + b.GetHealthPercentage() -64, (float)Math.Round((b.position.Y - World.LookingAt.Y) * Globals.TILE_SIZE) + 100-64);
                            Layer = 0.102f;
                            DrawLine(spriteBatch, xyhpr1, xyhpr2, Color.Red, 8);
                            Layer = 0.101f;
                            DrawLine(spriteBatch, xyhpr1, xyhpg2, Color.Green, 8);
                            Layer = 0.103f;
                            DrawLine(spriteBatch, xyhpr1 - new Vector2(1, 0), xyhpr2 + new Vector2(1, 0), Color.Black, 10);
                            //Number of units drawage
                            int x = (int)(t.position.X - (World.LookingAt.X));
                            int y = (int)(t.position.Y - (World.LookingAt.Y));
                            Rectangle r = new Rectangle(x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);
                            float fontX, fontY;
                            Vector2 stringSize;
                            string infosz;

                            this.Layer = 0.11f;

                            infosz = b.GetUnits().Count.ToString();
                            if (b.incomingUnits.Count > 0)
								infosz +=  " ("+b.incomingUnits.Count+")";
                            stringSize = Recellection.worldFont.MeasureString(infosz);
                            fontX = (float)(r.X + r.Width / 2) - stringSize.X / 2;
                            fontY = (float)(r.Y + r.Height / 4) - stringSize.Y;
                            spriteBatch.DrawString(Recellection.worldFont, infosz, new Vector2(fontX, fontY), Color.White, 0, new Vector2(0f), 1.0f, SpriteEffects.None, Layer);
#if DEBUG
                            infosz = GraphController.Instance.GetWeight(b).ToString();
                            stringSize = Recellection.worldFont.MeasureString(infosz);
                            fontX = (float)(r.X + r.Width / 2) - stringSize.X / 2;
                            fontY = (float)(r.Y + r.Height - stringSize.Y);
                            spriteBatch.DrawString(Recellection.worldFont, infosz, new Vector2(fontX, fontY), Color.White, 0, new Vector2(0f), 1.0f, SpriteEffects.None, Layer);
#endif
                        }
                    }

                    // Find those units!
                    List<Unit> units = t.GetUnits();
                    lock (units)
                    {
                        if (units.Count != 0)
                        {
                            myLogger.Info("Found unit(s) on the tile.");
                            foreach (Unit u in units)
                            {
                                this.Layer = 0.5f;
                                Texture2D spr = u.GetSprite();
                                int ux = (int)Math.Round((u.position.X - World.LookingAt.X) * Globals.TILE_SIZE) - spr.Width / 2;
                                int uy = (int)Math.Round((u.position.Y - World.LookingAt.Y) * Globals.TILE_SIZE) - spr.Height / 2;

								float amount = (0.3f + (u.PowerLevel*0.7f));
                                //c = Color.Lerp(c, Color.HotPink, 0.3f + u.PowerLevel * 0.5f);
								Color c = Color.Lerp(u.GetOwner().color, Color.White, amount);
                                
                                this.DrawTexture(spriteBatch, spr, new Rectangle(ux, uy, spr.Width, spr.Height), c);
                                //powerlevel debug: this.DrawCenteredString(spriteBatch, ""+u.PowerLevel, new Vector2(ux, uy - 30), Color.White);
                            }
                        }
                    }
				}

                if (World.DrawConstructionLines != null)
                {
                    foreach (Point p in World.DrawConstructionLines)
                    {
                         
                        Tile tile = World.map.GetTile(p.X, p.Y);
                        List<Vector2> points = tile.GetDrawPoints();
                        for (int line = 0; line <= 2; line+= 2)
                        {
                            DrawLine(spriteBatch, TileToPixels(points[line] - lookAt), TileToPixels(points[line+1] - lookAt),
                                                    Color.ForestGreen, 10);
                        }
                        
                    }
                }
            }
            
            // Draw scrollregions
            Layer = 0.0f;
            
            this.DrawTexture(spriteBatch, Recellection.textureMap.GetTexture(Globals.TextureTypes.ScrollUp),
				new Rectangle(128, 0, Globals.VIEWPORT_WIDTH - 256, 128));
			this.DrawTexture(spriteBatch, Recellection.textureMap.GetTexture(Globals.TextureTypes.ScrollDown),
				new Rectangle(128, Globals.VIEWPORT_HEIGHT - 128, Globals.VIEWPORT_WIDTH - 256, 128));
			this.DrawTexture(spriteBatch, Recellection.textureMap.GetTexture(Globals.TextureTypes.ScrollLeft),
				new Rectangle(0, 128, 128, Globals.VIEWPORT_HEIGHT - 256));
			this.DrawTexture(spriteBatch, Recellection.textureMap.GetTexture(Globals.TextureTypes.ScrollRight),
				new Rectangle(Globals.VIEWPORT_WIDTH - 128, 128, 128, Globals.VIEWPORT_HEIGHT - 256));

			this.DrawTexture(spriteBatch, Recellection.textureMap.GetTexture(Globals.TextureTypes.ScrollUpLeft),
				new Rectangle(0, 0, 128, 128));
			this.DrawTexture(spriteBatch, Recellection.textureMap.GetTexture(Globals.TextureTypes.ScrollUpRight),
				new Rectangle(Globals.VIEWPORT_WIDTH - 128, 0, 128, 128));
			this.DrawTexture(spriteBatch, Recellection.textureMap.GetTexture(Globals.TextureTypes.ScrollDownLeft),
				new Rectangle(0, Globals.VIEWPORT_HEIGHT - 128, 128, 128));
			this.DrawTexture(spriteBatch, Recellection.textureMap.GetTexture(Globals.TextureTypes.ScrollDownRight),
				new Rectangle(Globals.VIEWPORT_WIDTH - 128, Globals.VIEWPORT_HEIGHT - 128, 128, 128));
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

        public void UpdateBg(Object publisher, Event<Point> ev)
        {
            doRenderThisPass = true;
        }

        public void RenderToTex(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (doRenderThisPass)
            {
                Recellection.graphics.GraphicsDevice.SetRenderTarget(0, backgroundTarget);
                Recellection.graphics.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);
                lock (tileCollection)
                {
                    foreach (Tile t in tileCollection)
                    {
                        int x = (int)(t.position.X - (World.LookingAt.X));
                        int y = (int)(t.position.Y - (World.LookingAt.Y));

                        Rectangle r = new Rectangle(x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);

                        spriteBatch.Draw(t.GetSprite(), r, cMatrix[(int)t.position.X, (int)t.position.Y]); //here be dragons and (tile texture2D)

                        if (t.active)
                        {
                            spriteBatch.Draw(Recellection.textureMap.GetTexture(Globals.TextureTypes.ActiveTile), r, Color.White);
                        }
                    }

                    if (doLights)
                        lps.UpdateAndDraw(gameTime, spriteBatch);

                    if (doGrain)
                        gs.UpdateAndDraw(gameTime, spriteBatch);

                    spriteBatch.End();

                    Recellection.graphics.GraphicsDevice.SetRenderTarget(0, null);
                    backgroundTex = backgroundTarget.GetTexture();

                    doRenderThisPass = false;
                }
            }
        }

        /// <summary>
        /// takes two colors and blens them for each tile.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private Color[,] generateColorMatrix(Color c1, Color c2)
        {
            Random rnd = new Random();
            Color[,] colorM = new Color[World.map.width, World.map.height];
            for (int ix = 0; ix < World.map.width; ix++)
            {
                for (int iy = 0; iy < World.map.height; iy++)
                {
                   colorM[ix, iy] = Color.Lerp(c1, c2, (float) rnd.NextDouble());
                }
            }
            return colorM;
        }
	}
}
