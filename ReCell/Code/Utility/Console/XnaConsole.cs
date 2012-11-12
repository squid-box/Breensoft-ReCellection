// Xna Console
// www.codeplex.com/XnaConsole
// Copyright (c) 2008 Samuel Christie
namespace Recellection.Code.Utility.Console
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    using global::Recellection.Code.Utility.Logger;

    public delegate void InputHandler(string str);

    /// <remarks>This class creates a graphical text console for running code while executing a game.</remarks>
    public class XnaConsoleComponent : DrawableGameComponent
    {
        #region Constants

        const double AnimationTime = 0.2;

        const double CursorBlinkTime = 0.3;

        const int LinesDisplayed = 30;

        const string NewLine = "\n";
        const string Version = "Xna Console v.1.0";
        const string prompt = "> ";

        #endregion

        #region Fields

        readonly Texture2D background;

        readonly GraphicsDevice device;

        readonly double firstInterval;

        readonly SpriteFont font;

        readonly Game game;

        readonly History history;

        readonly Dictionary<Keys, double> keyTimes;

        readonly double repeatInterval;

        readonly SpriteBatch spriteBatch;

        KeyboardState CurrentKeyState;

        string InputBuffer;

        KeyboardState LastKeyState;

        ConsoleState State;
        double StateStartTime;

        int consoleXSize;

        int consoleYSize;

        int cursorOffset;

        int cursorPos;

        InputHandler input;

        int lineWidth;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the class, which creates a console for executing commands while running a game.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="interp"></param>
        /// <param name="font"></param>
        public XnaConsoleComponent(Game game, SpriteFont font)
            : base(game)
        {
            this.game = game;
            this.device = game.GraphicsDevice;
            this.spriteBatch = new SpriteBatch(this.device);
            this.font = font;

            /// TODO: Just a guess to fix this stuff...
            // background = new Texture2D(device, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            this.background = new Texture2D(this.device, 1, 1, true, SurfaceFormat.Color);
            
            this.background.SetData(new[] { new Color(0, 0, 0, 125) });
			
            this.InputBuffer = string.Empty;
            this.OutputBuffer = new StringWriter();
            
            this.history = new History();

            this.WriteLine("###");
            this.WriteLine("### " + Version);
			this.WriteLine("### Patched by Breensoft.");
			this.WriteLine("###");

            this.consoleXSize = this.Game.Window.ClientBounds.Right - this.Game.Window.ClientBounds.Left - 20;
            this.consoleYSize = font.LineSpacing * LinesDisplayed + 20;
            this.lineWidth = (int)(this.consoleXSize / font.MeasureString("m").X) - 2; // calculate number of letters that fit on a line, using "a" as example character

            this.State = ConsoleState.Closed;
            this.StateStartTime = 0;
            this.LastKeyState = this.CurrentKeyState = Keyboard.GetState();
            this.firstInterval = 500f;
            this.repeatInterval = 50f;

            // used for repeating keystrokes
            this.keyTimes = new Dictionary<Keys, double>();
            for (int i = 0; i < Enum.GetValues(typeof(Keys)).Length; i++)
            {
                var key = (Keys)Enum.GetValues(typeof(Keys)).GetValue(i);
                this.keyTimes[key] = 0f;
			}

			// lol. hack.
			LoggerFactory.SetGlobalTarget(this.OutputBuffer);
			LoggerFactory.GetLogger().Info("Initialized XNA console.");
        }

        #endregion

        #region Enums

        enum ConsoleState
        {
            Closed, 
            Closing, 
            Open, 
            Opening
        }

        #endregion

        #region Public Properties

        public StringWriter OutputBuffer { get; private set; }

        #endregion

        #region Public Methods and Operators

        public string Chomp(string str)
        {
            if (str.Length > 0 && str.Substring(str.Length - 1, 1) == "\n")
            {
                return str.Substring(0, str.Length - 1);
            }

            return str;
        }

        // check if the key has just been pressed

        /// <summary>
        /// Clears the output.
        /// </summary>
        public void Clear()
        {
            this.OutputBuffer.GetStringBuilder().Remove(0, this.OutputBuffer.ToString().Length);
        }

        /// <summary>
        /// Clears the command history.
        /// </summary>
        public void ClearHistory()
        {
            this.history.Clear();
        }

        public override void Draw(GameTime gameTime)
        {
            // don't draw the console if it's closed
            if (this.State == ConsoleState.Closed)
            {
                this.Visible = false;
                return;
            }

            double now = gameTime.TotalGameTime.TotalSeconds;

            // get console dimensions
            this.consoleXSize = this.Game.Window.ClientBounds.Right - this.Game.Window.ClientBounds.Left;
            this.consoleYSize = this.font.LineSpacing * LinesDisplayed;

            // set the offsets 
            int consoleXOffset = 0;
            int consoleYOffset = 0;

            // run the opening animation
            if (this.State == ConsoleState.Opening)
            {
                int startPosition = 0 - consoleYOffset - this.consoleYSize;
                int endPosition = consoleYOffset;
                consoleYOffset =
                    (int)
                    MathHelper.Lerp(
                        startPosition, endPosition, (float)(now - this.StateStartTime) / (float)AnimationTime);
            }
                
                // run the closing animation
            else if (this.State == ConsoleState.Closing)
            {
                int startPosition = consoleYOffset;
                int endPosition = 0 - consoleYOffset - this.consoleYSize;
                consoleYOffset =
                    (int)
                    MathHelper.Lerp(
                        startPosition, endPosition, (float)(now - this.StateStartTime) / (float)AnimationTime);
            }

            // calculate the number of letters that fit on a line
            this.lineWidth = (int)(this.consoleXSize / this.font.MeasureString("a").X) - 2;
                
                // remeasure lineWidth, incase the screen size changes
            this.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            #region Background Drawing

            this.spriteBatch.Draw(
                this.background, 
                new Rectangle(consoleXOffset, consoleYOffset, this.consoleXSize, this.consoleYSize), 
                Color.White);

            #endregion

            #region Text Drawing

            string cursorString = this.DrawCursor(now);

            this.spriteBatch.DrawString(
                this.font, 
                cursorString, 
                new Vector2(consoleXOffset + 10, consoleYOffset + this.consoleYSize - 10 - this.font.LineSpacing), 
                Color.White);

            int j = 0;
            List<string> lines = this.Render(this.OutputBuffer + prompt + this.InputBuffer);
                
                // show them in the proper order, because we're drawing from the bottom
            foreach (string str in lines)
            {
                // draw each line at an offset determined by the line height and line count
                j++;
                this.spriteBatch.DrawString(
                    this.font, 
                    str, 
                    new Vector2(
                        consoleXOffset + 10, consoleYOffset + this.consoleYSize - 10 - this.font.LineSpacing * j), 
                    Color.White);
            }

            #endregion

            this.spriteBatch.End();

            // reset depth buffer to normal status, so as not to mess up 3d code
            this.game.GraphicsDevice.BlendState = BlendState.Additive;
            this.game.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        public string DrawCursor(double now)
        {
            int spaces = this.cursorOffset;
            if (this.InputBuffer.Length > 0 && this.cursorPos > 0)
            {
                spaces = this.Render(this.InputBuffer.Substring(0, this.cursorPos))[0].Length + this.cursorOffset;
            }

            return new String(' ', spaces) + (((int)(now / CursorBlinkTime) % 2 == 0) ? "_" : string.Empty);
        }

        /// <summary>
        /// Prompts for input asynchronously via callback
        /// </summary>
        /// <param name="str"></param>
        /// <param name="callback"></param>
        public void Prompt(InputHandler callback)
        {
            string[] lines = this.WrapLine(this.OutputBuffer.ToString(), this.lineWidth).ToArray();
            this.input = callback;
            this.cursorOffset = prompt.Length;
        }

        public List<string> Render(string output)
        {
            List<string> lines = this.WrapLine(output, this.lineWidth);
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].Replace("\t", "    ");
            }

            lines.Reverse();
            return lines;
        }

        public override void Update(GameTime gameTime)
        {
            double now = gameTime.TotalGameTime.TotalSeconds;
            double elapsedTime = gameTime.ElapsedGameTime.TotalMilliseconds; // time since last update call

            // get keyboard state
            this.LastKeyState = this.CurrentKeyState;
            this.CurrentKeyState = Keyboard.GetState();

            

            if (this.State == ConsoleState.Closing)
            {
                if (now - this.StateStartTime > AnimationTime)
                {
                    this.State = ConsoleState.Closed;
                    this.StateStartTime = now;
                }

                return;
            }

            if (this.State == ConsoleState.Opening)
            {
                if (now - this.StateStartTime > AnimationTime)
                {
                    this.State = ConsoleState.Open;
                    this.StateStartTime = now;
                }

                return;
            }

            
            #region Closed state management

            if (this.State == ConsoleState.Closed)
            {
                if (this.IsKeyPressed(Keys.F1))
                {
                    // this opens the console
                    this.State = ConsoleState.Opening;
                    this.StateStartTime = now;
                    this.Visible = true;
                }
                else
                {
                    return;
                }
            }

            #endregion
            if (this.State == ConsoleState.Open)
            {
                #region initialize closing animation if user presses ` or ~

                if (this.IsKeyPressed(Keys.F1))
                {
                    this.State = ConsoleState.Closing;
                    this.StateStartTime = now;
                    return;
                }

                #endregion

                // execute current line with the interpreter
                if (this.IsKeyPressed(Keys.Enter))
                {
                    if (this.InputBuffer.Length > 0)
                    {
                        this.history.Add(this.InputBuffer); // add command to history
                    }

                    this.WriteLine(this.InputBuffer);

                    this.input(this.InputBuffer);

                    this.InputBuffer = string.Empty;
                    this.cursorPos = 0;
                }

                // erase previous letter when backspace is pressed
                if (this.KeyPressWithRepeat(Keys.Back, elapsedTime))
                {
                    if (this.cursorPos > 0)
                    {
                        this.InputBuffer = this.InputBuffer.Remove(this.cursorPos - 1, 1);
                        this.cursorPos--;
                    }
                }

                // delete next letter when delete is pressed
                if (this.KeyPressWithRepeat(Keys.Delete, elapsedTime))
                {
                    if (this.InputBuffer.Length != 0)
                    {
                        this.InputBuffer = this.InputBuffer.Remove(this.cursorPos, 1);
                    }
                }

                // cycle backwards through the command history
                if (this.KeyPressWithRepeat(Keys.Up, elapsedTime))
                {
                    this.InputBuffer = this.history.Previous();
                    this.cursorPos = this.InputBuffer.Length;
                }

                // cycle forwards through the command history
                if (this.KeyPressWithRepeat(Keys.Down, elapsedTime))
                {
                    this.InputBuffer = this.history.Next();
                    this.cursorPos = this.InputBuffer.Length;
                }

                // move the cursor to the right
                if (this.KeyPressWithRepeat(Keys.Right, elapsedTime) && this.cursorPos != this.InputBuffer.Length)
                {
                    this.cursorPos++;
                }

                // move the cursor left
                if (this.KeyPressWithRepeat(Keys.Left, elapsedTime) && this.cursorPos > 0)
                {
                    this.cursorPos--;
                }

                // move the cursor to the beginning of the line
                if (this.IsKeyPressed(Keys.Home))
                {
                    this.cursorPos = 0;
                }

                // move the cursor to the end of the line
                if (this.IsKeyPressed(Keys.End))
                {
                    this.cursorPos = this.InputBuffer.Length;
                }

                // get a letter from input
                string nextChar = this.GetStringFromKeyState(elapsedTime);

                // only add it if it isn't null
                if (nextChar != string.Empty)
                {
                    // if the cursor is at the end of the line, add the letter to the end
                    if (this.InputBuffer.Length == this.cursorPos)
                    {
                        this.InputBuffer += nextChar;
                    }
                        
                        // otherwise insert it where the cursor is
                    else
                    {
                        this.InputBuffer = this.InputBuffer.Insert(this.cursorPos, nextChar);
                    }

                    this.cursorPos += nextChar.Length;
                }
            }
        }

        /// <summary>
        /// Write to the console
        /// </summary>
        /// <param name="str"></param>
        public void Write(string str)
        {
            this.OutputBuffer.Write(str);
        }

        /// <summary>
        /// Write a line to the console
        /// </summary>
        /// <param name="str"></param>
        public void WriteLine(string str)
        {
            this.Write(str+NewLine);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Takes keyboard input and returns certain characters as a string
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <returns></returns>
        private string GetStringFromKeyState(double elapsedTime)
        {
            bool shiftPressed = this.CurrentKeyState.IsKeyDown(Keys.LeftShift) || this.CurrentKeyState.IsKeyDown(Keys.RightShift);
            bool altPressed = this.CurrentKeyState.IsKeyDown(Keys.LeftAlt) || this.CurrentKeyState.IsKeyDown(Keys.RightAlt);

            foreach (KeyBinding binding in KeyboardHelper.SwedishBindings)
                if (this.KeyPressWithRepeat(binding.Key, elapsedTime))
                {
                    if (!shiftPressed && !altPressed)
                        return binding.UnmodifiedString;
                    else if (shiftPressed && !altPressed)
                        return binding.ShiftString;
                    else if (!shiftPressed && altPressed)
                        return binding.AltString;
                    else if (shiftPressed && altPressed)
                        return binding.ShiftAltString;
                }

            return string.Empty;
        }

        private bool IsKeyPressed(Keys key)
        {
            return this.CurrentKeyState.IsKeyDown(key) && !this.LastKeyState.IsKeyDown(key);
        }

        // check if a key is pressed, and repeat it at the default repeat rate
        private bool KeyPressWithRepeat(Keys key, double elapsedTime)
        {
            if (this.CurrentKeyState.IsKeyDown(key))
            {
                if (this.IsKeyPressed(key))
                {
                    return true; // if the key has just been pressed, it automatically counts
                }

                this.keyTimes[key] -= elapsedTime; // count down to next repeat
                double keyTime = this.keyTimes[key]; // get the time left
                if (this.keyTimes[key] <= 0)
                {
                    // if the time has run out, repeat the letter
                    this.keyTimes[key] = this.repeatInterval; // reset the timer to the repeat interval
                    return true;
                }
                else
                {
                    return false;
                }
            }
                
                // if the key is not pressed, reset it's time to the first interval, which is usually longer
            else
            {
                this.keyTimes[key] = this.firstInterval;
                return false;
            }
        }

        /// <summary>
        /// This takes a single string and splits it at the newlines and the specified number of columns
        /// </summary>
        /// <param name="line"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private List<string> WrapLine(string line, int columns)
        {
            var wraplines = new List<string>();
            if (line.Length > 0)
            {
                wraplines.Add(string.Empty);
                int lineNum = 0;

                for (int i = 0; i < line.Length; i++)
                {
                    string ch = line.Substring(i, 1);

                    if (ch == "\n" || wraplines[lineNum].Length > columns)
                    {
                        wraplines.Add(string.Empty);
                        lineNum++;
                    }
                    else
                    {
                        wraplines[lineNum] += ch;
                    }
                }
            }

            return wraplines;
        }

        /// <summary>
        /// This takes an array of strings and splits each of them every newline and specified number of columns
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private List<string> WrapLines(string[] lines, int columns)
        {
            var wraplines = new List<string>();
            foreach (string line in lines)
            {
                wraplines.AddRange(this.WrapLine(line, columns));
            }

            return wraplines;
        }

        #endregion

        /// <summary>
        /// Object for storing command history
        /// </summary>
        public class History
        {
            #region Fields

            readonly List<string> history;
            int index;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Make a new history object with capacity maxLength
            /// </summary>
            /// <param name="maxLength"></param>
            public History()
            {
                this.history = new List<string>();
            }

            #endregion

            #region Public Properties

            /// <summary>
            /// Returns the current command in history
            /// </summary>
            public string Current
            {
                get { if (this.index < this.history.Count) { return this.history[this.index]; } else { return string.Empty; } }
            }

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// Add a command to the history
            /// </summary>
            /// <param name="str"></param>
            public void Add(string str)
            {
                this.history.Add(str);
                this.index = this.history.Count;
            }

            /// <summary>
            /// Erase command history
            /// </summary>
            public void Clear()
            {
                this.history.Clear();
            }

            /// <summary>
            /// Cycle forwards through commands in history
            /// </summary>
            /// <returns></returns>
            public string Next()
            {
                if (this.index < this.history.Count - 1)
                {
                    this.index++;
                }

                return this.Current;
            }

            /// <summary>
            /// Cycle backwards through commands in history
            /// </summary>
            /// <returns></returns>
            public string Previous()
            {
                if (this.index > 0)
                {
                    this.index--;
                }

                return this.Current;
            }

            #endregion
        }
    }
}
