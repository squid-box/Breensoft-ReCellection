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

            Recellection game = new Recellection();
            Initializer logic = new Initializer(game.Window.Handle);
            Thread gameLogic = new Thread(logic.Run);

            game.LogicThread = gameLogic;
            Recellection.textureMap = new SpriteTextureMap(game.Content);

            #region let's fullscreen this bastard
            
            
            System.Windows.Forms.Form form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(game.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Focus();
            form.TopMost = true;
            Recellection.graphics.PreferredBackBufferWidth = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            Recellection.graphics.PreferredBackBufferHeight = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            Recellection.graphics.ApplyChanges();

            #endregion

            game.Run();
			gameLogic.Abort();
			Environment.Exit(0);
			// MOAR EXITS! MOAR!!! DEATH TO ALL APPLICATION!
        }
    }
}

