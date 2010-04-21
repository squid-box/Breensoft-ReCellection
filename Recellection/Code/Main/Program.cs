using System;
using System.Runtime.CompilerServices;
using Recellection.Code.Utility.Logger;
using Recellection.Code.Utility.Console;
using Microsoft.Xna.Framework.Graphics;

// We should be able to test internals
//[assembly: InternalsVisibleTo("RecellectionTests")]

namespace Recellection
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
		{
			Recellection game = new Recellection();
			
			LoggerSetup.Initialize();
			
			game.Run();

			//LoggerSetup.target = game.console.Console.OutputBuffer;
        }
    }
}

