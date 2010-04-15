using System;
using System.Runtime.CompilerServices;
using Recellection.Code.Utility.Logger;

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
            using (Recellection game = new Recellection())
            {
                game.Run();
            }
        }
    }
}

