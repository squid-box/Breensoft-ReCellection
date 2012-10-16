// Xna Console
// www.codeplex.com/XnaConsole
// Copyright (c) 2008 Samuel Christie
namespace Recellection.Code.Utility.Console
{
    using System;
    using System.IO;
    using System.Text;

    using IronPython.Hosting;
    using IronPython.Modules;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <remarks>
    /// This class implements an interpreter using IronPython
    /// </remarks>
    public class PythonInterpreter : DrawableGameComponent
    {
        #region Constants

        const string Prompt = "> ";
        const string PromptCont = "... ";

        #endregion

        #region Fields

        public XnaConsoleComponent Console;

        private readonly ASCIIEncoding ASCIIEncoder;

        private readonly PythonEngine PythonEngine;
		private readonly MemoryStream PythonOutput;

        string multi;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Creates a new PythonInterpreter
        /// </summary>
        public PythonInterpreter(Game game, SpriteFont font) : base(game)
        {
            this.PythonEngine = new PythonEngine();
            this.PythonOutput = new MemoryStream();
            this.PythonEngine.SetStandardOutput(this.PythonOutput);
            this.ASCIIEncoder = new ASCIIEncoding();

			
            var clr = this.PythonEngine.Import("clr") as ClrModule;
            clr.AddReference("Microsoft.Xna.Framework");
            clr.AddReference("Microsoft.Xna.Framework.Game");

            this.PythonEngine.Execute("from Microsoft.Xna.Framework import *");
            this.PythonEngine.Execute("from Microsoft.Xna.Framework.Graphics import *");
            this.PythonEngine.Execute("from Microsoft.Xna.Framework.Content import *");
            this.multi = string.Empty;

            this.Console = new XnaConsoleComponent(game, font);
            game.Components.Add(this.Console);
            this.Console.Prompt(this.Execute);
            this.AddGlobal("Console", this.Console);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds a global variable to the environment of the interpreter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddGlobal(string name, object value)
        {
            this.PythonEngine.Globals.Add(name, value);
        }

        /// <summary>
        /// Executes python commands from the console.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Returns the execution results or error messages.</returns>
        public void Execute(string input)
        {
            try
            {
                if ((input != string.Empty) && ((input[input.Length - 1].ToString() == ":") || (this.multi != string.Empty)))
                {
                    // multiline block incomplete, ask for more
                    this.multi += input + "\n";
                    this.Console.Prompt(this.Execute);
                }
                else if (this.multi != string.Empty && input == string.Empty)
                {
                    // execute the multiline code after block is finished
                    string temp = this.multi; // make sure that multi is cleared, even if it returns an error
                    this.multi = string.Empty;
                    this.PythonEngine.Execute(temp);
                    this.Console.WriteLine(this.getOutput());
                    this.Console.Prompt(this.Execute);
                }
                else
                {
                    // if (multi == "" && input != "") execute single line expressions or statements
                    this.PythonEngine.Execute(input);
                    this.Console.WriteLine(this.Console.Chomp(this.getOutput()));
                    this.Console.Prompt(this.Execute);
                }
            }
            catch (Exception ex)
            {
                this.Console.WriteLine("Error: " + ex.Message + ".");
                this.Console.Prompt(this.Execute);
            }

        }

        public bool IsActive()
        {
            return this.Console.Visible;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get string output from IronPythons MemoryStream standard out
        /// </summary>
        /// <returns></returns>
        private string getOutput()
        {
            var statementOutput = new byte[this.PythonOutput.Length];
            this.PythonOutput.Position = 0;
            this.PythonOutput.Read(statementOutput, 0, (int)this.PythonOutput.Length);
            this.PythonOutput.Position = 0;
            this.PythonOutput.SetLength(0);
            
            return this.ASCIIEncoder.GetString(statementOutput);
        }

        #endregion
    }
}
