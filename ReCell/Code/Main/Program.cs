using System;
using System.Runtime.CompilerServices;
using Recellection.Code.Utility.Logger;
using Recellection.Code.Utility.Console;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Recellection.Code.Main;
using Recellection.Code.Models;

// We should be able to test internals
[assembly: InternalsVisibleTo("RecellectionTests")]

namespace Recellection
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
		{
			LoggerSetup.Initialize();

			Language.Instance.SetLanguage("English");

			// This is the bridge between XNA and the logic
			GraphicsRenderer graphicRendering = new GraphicsRenderer();

			Recellection game = new Recellection(graphicRendering);
			Initializer logic = new Initializer(graphicRendering, game.Window.Handle);

			Thread gameLogic = new Thread(logic.Run);

			gameLogic.Start();
			game.Run();
        }
    }
}

