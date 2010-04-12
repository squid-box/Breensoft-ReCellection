using System;
using Recellection.Code.Utility;

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

