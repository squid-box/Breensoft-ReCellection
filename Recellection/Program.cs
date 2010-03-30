using System;

namespace Recellection
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Recellection game = new Recellection())
            {
                game.Run();
            }
        }
    }
}

