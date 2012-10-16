namespace Recellection.Code.Views
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using global::Recellection.Code.Controllers;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Events;

    using global::Recellection.Code.Utility.Logger;

    /// <summary>
    /// The purpose of the World View is to provide the necessary data to render the game
    /// screen as described x the SRD 3.3. It also stores the information of the game state available
    /// to the player. The World View contains the information that is relevant to a single player, and
    /// therefore has a reference to a Player-object.
    /// </summary>
    public class WorldView : IView
    {
        #region Static Fields

        public static bool doGrain = true;

        public static bool doLights = false;

        private static readonly int maxCols = (int)(Recellection.viewPort.Width / (float)Globals.TILE_SIZE);
        private static readonly int maxRows = (int)(Recellection.viewPort.Height / (float)Globals.TILE_SIZE);

        #endregion

        #region Fields

        public Logger myLogger;

        private readonly RenderTarget2D backgroundTarget;

        private readonly GrainSystem gs;

        private readonly LightParticleSystem lps;

        private readonly List<Tile> tileCollection;

        private Texture2D backgroundTex;

        private Color[,] cMatrix;

        private bool doRenderThisPass = true;

        #endregion

        #region Constructors and Destructors

        private WorldView()
        {
            this.backgroundTex = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);
            this.backgroundTarget = new RenderTarget2D(
                Recellection.graphics.GraphicsDevice, 
                Recellection.viewPort.Width, 
                Recellection.viewPort.Height, 
                true, 
                SurfaceFormat.Color, 
                DepthFormat.Depth24); // TODO: Double check this, might be wrong.
            this.lps = new LightParticleSystem(0.05f, Recellection.textureMap.GetTexture(Globals.TextureTypes.Light));
            this.gs = new GrainSystem(0.01f, 0.2f, 0.3f, Recellection.contentMngr);

            this.myLogger = LoggerFactory.GetLogger();

            // myLogger.SetTarget(LoggerSetup.GetLogFileTarget("WorldView.log"));
            this.myLogger.SetThreshold(LogLevel.FATAL);
            this.myLogger.Info("Created a WorldView.");

            this.tileCollection = new List<Tile>();

            // this.World.LookingAt = new Vector2(0, 0);
            Instance = this;
        }

        #endregion

        #region Public Properties

        public static WorldView Instance { get;	set; }

        public World World { get; private set; }

        #endregion

        #region Public Methods and Operators

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


            // Color c1 = new Color(0xb2, 0xc9, 0x9f);
            // Color c2 = new Color(0x9f, 0xc4, 0xc9);
            Color c1 = Color.HotPink;// new Color(0xac, 0x33, 0x2d);
            Color c2 = Color.Crimson;// new Color(0xea, 0xe4, 0x7c);
            Instance.cMatrix = Instance.generateColorMatrix(c1, c2);
		}

        override public void Draw(SpriteBatch spriteBatch)
        {
            
			this.Layer = 1.0f;
			this.DrawTexture(spriteBatch, this.backgroundTex, new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));
            
			
            lock (this.tileCollection)
            {
                this.Layer = 0.75f;

                // Go through each players graphs and draw lines between buildings.
                var lookAt = new Vector2(this.World.LookingAt.X, this.World.LookingAt.Y);
                foreach (Player p in this.World.players)
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

                            foreach (Building b in g.GetBuildings())
                            {
                                if (b == bb)
                                {
                                    continue;
                                }

                                Vector2 drawFrom = bb.position;
                                if (b.Parent != null)
                                {
                                    drawFrom = b.Parent.position;
                                }

                                this.myLogger.Info("Drawing line between " + bb + " and " + b + ".");
                                this.DrawLine(
                                    spriteBatch, 
                                    this.TileToPixels(drawFrom - lookAt), 
                                    this.TileToPixels(b.position - lookAt), 
                                    new Color(b.owner.color.R, b.owner.color.G, b.owner.color.B, 80), 
                                    10);
                            }
                        }
                    }
                }

                foreach (Tile t in this.tileCollection)
                {
                    // Building? On my Tile?! It's more likely than you think.
                    Building b = t.GetBuilding();
                    if (b != null)
                    {
                        lock (b)
                        {
                            this.myLogger.Info("Found a building on the tile.");
                            this.Layer = 0.1f;
                            Texture2D spr = b.GetSprite();
                            float size = 0.75f + 0.75f * Math.Min(100f, GraphController.Instance.GetWeight(b)) / 100f;
                            int bx = (int)Math.Round((b.position.X - this.World.LookingAt.X) * Globals.TILE_SIZE)
                                     - (int)Math.Round(spr.Width * size) / 2;
                            int by = (int)Math.Round((b.position.Y - this.World.LookingAt.Y) * Globals.TILE_SIZE)
                                     - (int)Math.Round(spr.Height * size) / 2;
                            this.DrawTexture(
                                spriteBatch, 
                                spr, 
                                new Rectangle(
                                    bx, by, (int)Math.Round(spr.Width * size), (int)Math.Round(spr.Height * size)), 
                                b.owner.color);

                            var xyhpr1 =
                                new Vector2(
                                    ((b.position.X - this.World.LookingAt.X) * Globals.TILE_SIZE) + 14 - 64, 
                                    (float)Math.Round((b.position.Y - this.World.LookingAt.Y) * Globals.TILE_SIZE) + 100
                                    - 64);
                            var xyhpr2 =
                                new Vector2(
                                    ((b.position.X - this.World.LookingAt.X) * Globals.TILE_SIZE) + 114 - 64, 
                                    (float)Math.Round((b.position.Y - this.World.LookingAt.Y) * Globals.TILE_SIZE) + 100
                                    - 64);
                            var xyhpg2 =
                                new Vector2(
                                    ((b.position.X - this.World.LookingAt.X) * Globals.TILE_SIZE) + 14
                                    + b.GetHealthPercentage() - 64, 
                                    (float)Math.Round((b.position.Y - this.World.LookingAt.Y) * Globals.TILE_SIZE) + 100
                                    - 64);
                            this.Layer = 0.102f;
                            this.DrawLine(spriteBatch, xyhpr1, xyhpr2, Color.Red, 8);
                            this.Layer = 0.101f;
                            this.DrawLine(spriteBatch, xyhpr1, xyhpg2, Color.Green, 8);
                            this.Layer = 0.103f;
                            this.DrawLine(
                                spriteBatch, xyhpr1 - new Vector2(1, 0), xyhpr2 + new Vector2(1, 0), Color.Black, 10);

                            // Number of units drawage
                            var x = (int)(t.position.X - this.World.LookingAt.X);
                            var y = (int)(t.position.Y - this.World.LookingAt.Y);
                            var r = new Rectangle(
                                x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);
                            float fontX, fontY;
                            Vector2 stringSize;
                            string infosz;

                            this.Layer = 0.11f;

                            infosz = b.GetUnits().Count.ToString();
                            if (b.incomingUnits.Count > 0)
                            {
                                infosz += " (" + b.incomingUnits.Count + ")";
                            }

                            stringSize = Recellection.worldFont.MeasureString(infosz);
                            fontX = (r.X + r.Width / 2) - stringSize.X / 2;
                            fontY = (r.Y + r.Height / 4) - stringSize.Y;
                            spriteBatch.DrawString(
                                Recellection.worldFont, 
                                infosz, 
                                new Vector2(fontX, fontY), 
                                Color.White, 
                                0, 
                                new Vector2(0f), 
                                1.0f, 
                                SpriteEffects.None, 
                                this.Layer);
#if DEBUG
                            infosz = GraphController.Instance.GetWeight(b).ToString();
                            stringSize = Recellection.worldFont.MeasureString(infosz);
                            fontX = (r.X + r.Width / 2) - stringSize.X / 2;
                            fontY = r.Y + r.Height - stringSize.Y;
                            spriteBatch.DrawString(
                                Recellection.worldFont, 
                                infosz, 
                                new Vector2(fontX, fontY), 
                                Color.White, 
                                0, 
                                new Vector2(0f), 
                                1.0f, 
                                SpriteEffects.None, 
                                this.Layer);
#endif
                        }
                    }

                    // Find those units!
                    List<Unit> units = t.GetUnits();
                    lock (units)
                    {
                        if (units.Count != 0)
                        {
                            this.myLogger.Info("Found unit(s) on the tile.");
                            foreach (Unit u in units)
                            {
                                this.Layer = 0.5f;
                                Texture2D spr = u.GetSprite();
                                int ux = (int)Math.Round((u.position.X - this.World.LookingAt.X) * Globals.TILE_SIZE)
                                         - spr.Width / 2;
                                int uy = (int)Math.Round((u.position.Y - this.World.LookingAt.Y) * Globals.TILE_SIZE)
                                         - spr.Height / 2;

                                float amount = 0.3f + (u.PowerLevel * 0.7f);

                                // c = Color.Lerp(c, Color.HotPink, 0.3f + u.PowerLevel * 0.5f);
                                Color c = Color.Lerp(u.GetOwner().color, Color.White, amount);

                                this.DrawTexture(spriteBatch, spr, new Rectangle(ux, uy, spr.Width, spr.Height), c);

                                // powerlevel debug: this.DrawCenteredString(spriteBatch, ""+u.PowerLevel, new Vector2(ux, uy - 30), Color.White);
                            }
                        }
                    }
                }

                if (this.World.DrawConstructionLines != null)
                {
                    foreach (Point p in this.World.DrawConstructionLines)
                    {
                        Tile tile = this.World.map.GetTile(p.X, p.Y);
                        List<Vector2> points = tile.GetDrawPoints();
                        for (int line = 0; line <= 2; line += 2)
                        {
                            this.DrawLine(
                                spriteBatch, 
                                this.TileToPixels(points[line] - lookAt), 
                                this.TileToPixels(points[line + 1] - lookAt), 
                                Color.ForestGreen, 
                                10);
                        }
                    }
                }
            }

            // Draw scrollregions
            this.Layer = 0.0f;
            
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

        public void RenderToTex(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (this.doRenderThisPass)
            {
                Recellection.graphics.GraphicsDevice.SetRenderTarget(this.backgroundTarget);
                Recellection.graphics.GraphicsDevice.Clear(Color.Black);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                lock (this.tileCollection)
                {
                    foreach (Tile t in this.tileCollection)
                    {
                        var x = (int)(t.position.X - this.World.LookingAt.X);
                        var y = (int)(t.position.Y - this.World.LookingAt.Y);

                        var r = new Rectangle(x * Globals.TILE_SIZE, y * Globals.TILE_SIZE, Globals.TILE_SIZE, Globals.TILE_SIZE);

                        spriteBatch.Draw(t.GetSprite(), r, this.cMatrix[(int)t.position.X, (int)t.position.Y]); // here be dragons and (tile texture2D)

                        if (t.active)
                        {
                            spriteBatch.Draw(Recellection.textureMap.GetTexture(Globals.TextureTypes.ActiveTile), r, Color.White);
                        }
                    }

                    if (doLights)
                        this.lps.UpdateAndDraw(gameTime, spriteBatch);

                    if (doGrain)
                        this.gs.UpdateAndDraw(gameTime, spriteBatch);

                    spriteBatch.End();

                    Recellection.graphics.GraphicsDevice.SetRenderTarget(null);
                    this.backgroundTex = this.backgroundTarget;

                    this.doRenderThisPass = false;
                }
            }
        }

        override public void Update(GameTime passedTime)
        {
            KeyboardState ks = Keyboard.GetState();

            int f = 1;

            if (ks.IsKeyDown(Keys.X))
            {
                this.World.LookingAt = new Point(
						(int)this.World.players[0].GetGraphs()[0].baseBuilding.position.X, 
						(int)this.World.players[0].GetGraphs()[0].baseBuilding.position.Y);
            }

			int x = this.World.LookingAt.X;
			int y = this.World.LookingAt.Y;

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
            
            x = (int)MathHelper.Clamp(x, 0, this.World.map.width - maxCols);
            y = (int)MathHelper.Clamp(y, 0, this.World.map.height - maxRows);
            
			this.World.LookingAt = new Point(x, y);
        }

        public void UpdateBg(object publisher, Event<Point> ev)
        {
            this.doRenderThisPass = true;
        }

        /// <summary>
        /// I have no idea what this is supposed to do.
        /// </summary>
        [Obsolete]
        public void UpdateMapMatrix()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        private void CreateCurrentView(object o, Event<Point> ev)
        {
            // First, add all tiles from the map:
            this.myLogger.Info("Getting tiles from World.map.");
            Tile[,] tiles = this.World.map.map;

            // Vector2 copyLookingAt = alignViewport(ev.subject);
            lock (this.tileCollection)
            {
                this.tileCollection.Clear();

                int currentX = this.World.LookingAt.X;
                int currentY = this.World.LookingAt.Y;

                this.myLogger.Info("Rendering for X:" + currentX + " and Y:" + currentY + ".");
                this.myLogger.Info("Width:" + maxCols + " and Height:" + maxRows + ".");
                for (int x = currentX; x < currentX + maxCols; x++)
                {
                    for (int y = currentY; y < currentY + maxRows; y++)
                    {
                        // if (! tiles[x, y].IsVisible(this.World.players[0]))
                        // 	continue;
                        try
                        {
                            this.tileCollection.Add(tiles[x, y]);
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            this.myLogger.Fatal("OMG FAIL | " + e.GetType() + " : " +e.Message);
                        }
                    }
                }
            }
        }

        private Vector2 TileToPixels(Vector2 tileCoords)
        {
            var pixelCoords = new Vector2();
            Vector2.Multiply(ref tileCoords, Globals.TILE_SIZE, out pixelCoords);
            return pixelCoords;
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
        /// takes two colors and blens them for each tile.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        private Color[,] generateColorMatrix(Color c1, Color c2)
        {
            var rnd = new Random();
            var colorM = new Color[this.World.map.width, this.World.map.height];
            for (int ix = 0; ix < this.World.map.width; ix++)
            {
                for (int iy = 0; iy < this.World.map.height; iy++)
                {
                   colorM[ix, iy] = Color.Lerp(c1, c2, (float) rnd.NextDouble());
                }
            }

            return colorM;
        }

        #endregion
    }
}
