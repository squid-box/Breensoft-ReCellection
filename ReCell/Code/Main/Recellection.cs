namespace Recellection
{
    using System;
    using System.Globalization;
    using System.Threading;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using global::Recellection.Code.Controllers;
    using global::Recellection.Code.Models;
    using global::Recellection.Code.Utility.Console;
    using global::Recellection.Code.Utility.Logger;
    using global::Recellection.Code.Views;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Recellection : Game
    {
        #region Static Fields

        /// <summary>
        /// The color Breen.
        /// </summary>
        public static Color Breen = new Color(new Vector3(0.4f, 0.3f, 0.1f));

        public static ContentManager contentMngr;

        public static GraphicsDeviceManager graphics;

        public static KeyboardState publicKeyBoardState;

        public static SpriteFont screenFont;

        public static SpriteTextureMap textureMap;

        public static Viewport viewPort;

        public static IntPtr windowHandle;

        public static SpriteFont worldFont;

        // Current state!
        private static IView currentState;

        private static Logger logger = LoggerFactory.GetLogger("XNA");

        #endregion

        #region Fields

        private readonly TobiiController tobiiController;

        // Sounds and music
        private AudioPlayer audioPlayer;

        private PythonInterpreter console;

        private SpriteFont consoleFont;

        private TimeSpan elapsedTime;

        private int frameCounter;

        private int frameRate;

        // Debug Input 
        KeyboardState keyboardState;

        KeyboardState lastKeyboardState;

        MouseState lastMouseState;

        MouseState mouseState;

        SpriteBatch spriteBatch;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Recellection"/> class.
        /// </summary>
        public Recellection()
        {
            this.tobiiController = TobiiController.GetInstance();
            this.tobiiController.Init();
            graphics = new GraphicsDeviceManager(this);            
            this.Content.RootDirectory = "Content";

            contentMngr = this.Content;
        }

        #endregion

        #region Public Properties

        public static IView CurrentState
        { 
            get { return currentState; }
            set { currentState = value; }
        }

        public Thread LogicThread { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Plays a section from Beethoven's |which one was it now..?|.
        /// </summary>
        public static void PlayBeethoven()
        {
            Console.Beep(659, 120);  // Treble E
            Console.Beep(622, 120);  // Treble D#

            Thread.Sleep(60);

            Console.Beep(659, 120);  // Treble E
            Console.Beep(622, 120);  // Treble D#
            Console.Beep(659, 120);  // Treble E
            Console.Beep(494, 120);  // Treble B
            Console.Beep(587, 120);  // Treble D
            Console.Beep(523, 120);  // Treble C

            Thread.Sleep(70);

            Console.Beep(440, 120);  // Treble A
            Console.Beep(262, 120);  // Middle C
            Console.Beep(330, 120);  // Treble E
            Console.Beep(440, 120);  // Treble A

            Thread.Sleep(70);

            Console.Beep(494, 120);  // Treble B
            Console.Beep(330, 120);  // Treble E
            Console.Beep(415, 120);  // Treble G#
            Console.Beep(494, 120);  // Treble B

            Thread.Sleep(70);

            Console.Beep(523, 120);  // Treble C
            Console.Beep(330, 120);  // Treble E
            Console.Beep(659, 120);  // Treble E
            Console.Beep(622, 120);  // Treble D#

            Thread.Sleep(70);

            Console.Beep(659, 120);  // Treble E
            Console.Beep(622, 120);  // Treble D#
            Console.Beep(659, 120);  // Treble E
            Console.Beep(494, 120);  // Treble B
            Console.Beep(587, 120);  // Treble D
            Console.Beep(523, 120);  // Treble C

            Thread.Sleep(70);

            Console.Beep(440, 120);  // Treble A
            Console.Beep(262, 120);  // Middle C
            Console.Beep(330, 120);  // Treble E
            Console.Beep(440, 120);  // Treble A

            Thread.Sleep(70);

            Console.Beep(494, 120);  // Treble B
            Console.Beep(330, 120);  // Treble E
            Console.Beep(523, 120);  // Treble C
            Console.Beep(494, 120);  // Treble B
            Console.Beep(440, 120);  // Treble A
        }

        /// <summary>
        /// Prints key shortcuts to the console.
        /// </summary>
        public void Help()
        {
            this.console.Console.WriteLine("M: Toggle music\nI: Turn SFX off\nO: Turn SFX on\nA: Acid sound\nB: Explosion sound\nF1: Toggle Console\nF: \"full\" screen");
        }

        /// <summary>
        /// Prints a list of available loggers to the console.
        /// </summary>
        public void ListLoggers()
        {
            this.console.Console.WriteLine(LoggerFactory.ListLoggers());
        }

        /// <summary>
        /// Enables logging of the AI.
        /// </summary>
        public void LogAI()
        {
            this.ToggleLogger("Recellection.Code.Controllers.AIPlayer");
            this.ToggleLogger("Recellection.Code.AIView");
        }

        public void SetLoggers(int i)
        {
            bool active = false;
            if (i != 0)
                active = false;
            LoggerFactory.SetAll(active);
        }

        public void ToggleLogger(string s)
        {
            if (LoggerFactory.HasLogger(s))
            {
                LoggerFactory.GetLogger(s).Active = !LoggerFactory.GetLogger(s).Active;
            }
            else
            {
                this.console.Console.WriteLine("Logger does not exist");
            }

        }

        public void lawl(string sound)
		{
			this.audioPlayer.PlaySound(sound);
        }

        public void tudeloo()
        {
            PlayBeethoven();
        }

        #endregion

        #region Methods

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (currentState is WorldView)
            {
                WorldView.Instance.RenderToTex(this.spriteBatch, gameTime);
            }

            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (currentState != null)
            {
                currentState.Draw(this.spriteBatch);
            }

#if DEBUG
            this.spriteBatch.DrawString(screenFont, this.frameRate.ToString(CultureInfo.InvariantCulture), Vector2.Zero, Color.Red);
#endif
            this.spriteBatch.End();

            // Do the FPS dance
            this.frameCounter++;

            base.Draw(gameTime);
        }

        /// <summary>
        /// Initialize all classes and set renderstates
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

#if DEBUG

            // Initialize the python console
            this.console = new PythonInterpreter(this, this.consoleFont);
            this.console.AddGlobal("game", this);
#endif

            windowHandle = this.Window.Handle;

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new TimeSpan(10000000L / 30L);

            // graphics.ApplyChanges();
            this.LogicThread.Start();
        }

        /// <summary>
        /// Create models and load game data
        /// </summary>
        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

            screenFont = this.Content.Load<SpriteFont>("Fonts/ScreenFont");
            this.consoleFont = this.Content.Load<SpriteFont>("Fonts/ConsoleFont");
            worldFont = this.Content.Load<SpriteFont>("Fonts/WorldFont");

            this.audioPlayer = new AudioPlayer(this.Content);
            this.audioPlayer.PlaySong(Globals.Songs.Theme);

            viewPort = graphics.GraphicsDevice.Viewport;
        }

        /// <summary>
        /// Not Used
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (currentState != null)
            {
                currentState.Update(gameTime);
            }

            this.HandleDebugInput();

            this.elapsedTime += gameTime.ElapsedGameTime;
            if (this.elapsedTime > TimeSpan.FromSeconds(1))
            {
                this.elapsedTime -= TimeSpan.FromSeconds(1);
                this.frameRate = this.frameCounter;
                this.frameCounter = 0;
            }
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This method allows us to use the keyboard and mouse to test functionality in the program
        /// </summary>
        private void HandleDebugInput()
        {
#if DEBUG

            // If the console is open, we ignore input.
            if (this.console.IsActive())
            {
                return;
            }

#endif

            this.lastKeyboardState = this.keyboardState;
            this.lastMouseState = this.mouseState;
            publicKeyBoardState = Keyboard.GetState();
            this.keyboardState = publicKeyBoardState;
            this.mouseState = Mouse.GetState();

            if (this.keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            if (this.keyboardState.IsKeyDown(Keys.End))
            {
                WorldController.finished = true;
            }

            if (this.keyboardState.IsKeyDown(Keys.F) && this.lastKeyboardState.IsKeyUp(Keys.F))
            {
                var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
                if (form.Width > 800)
                {
                    form.Location = new System.Drawing.Point(0, 0);
                    form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    form.TopMost = true;
                    graphics.PreferredBackBufferWidth = 800;
                    graphics.PreferredBackBufferHeight = 600;
                    graphics.ApplyChanges();
                }
                else
                {
                    form.Location = new System.Drawing.Point(0, 0);
                    form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    form.TopMost = true;
                    graphics.PreferredBackBufferWidth = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
                    graphics.PreferredBackBufferHeight = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
                    graphics.ApplyChanges();
                }
            }
        }

        #endregion
    }
}
