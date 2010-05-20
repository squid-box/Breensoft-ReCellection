using System;
using System.Runtime.CompilerServices;
using Recellection.Code.Utility.Logger;
using Recellection.Code.Utility.Console;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Recellection.Code.Main;
using Recellection.Code.Models;
using System.Diagnostics;

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
            Process p = Process.GetCurrentProcess();
            p.PriorityClass = ProcessPriorityClass.High;

			LoggerSetup.Initialize();

			Language.Instance.SetLanguage("Swedish");

            Recellection game = new Recellection();
            Initializer logic = new Initializer(game.Window.Handle);
            Thread gameLogic = new Thread(logic.Run);

            game.LogicThread = gameLogic;
            Recellection.textureMap = new SpriteTextureMap(game.Content);

            FullScreen(game.Window.Handle);
            

            game.Run();
			gameLogic.Abort();
			Environment.Exit(0);
			// MOAR EXITS! MOAR!!! DEATH TO ALL APPLICATION!
        }

        /// <summary>
        /// ugly way to force fullscreen
        /// used to keep tobii feedback active
        /// </summary>
        static void FullScreen(IntPtr handle)
        {

            System.Windows.Forms.Form form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(handle);
            form.Location = new System.Drawing.Point(0, 0);
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.Focus();
            form.TopMost = true;
            Globals.VIEWPORT_HEIGHT = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Height;
            Globals.VIEWPORT_WIDTH = System.Windows.Forms.SystemInformation.PrimaryMonitorSize.Width;
            Recellection.graphics.PreferredBackBufferWidth = Globals.VIEWPORT_WIDTH;
            Recellection.graphics.PreferredBackBufferHeight = Globals.VIEWPORT_HEIGHT;
            Recellection.graphics.ApplyChanges();

        }
        
        /// <summary>
        /// this does not work!
        /// </summary>
        [Obsolete("Do not use, it does not work, it anti-works")]
        static void GoodFullScreen() 
        { 
            Recellection.graphics.ToggleFullScreen();
            Process[] processlist = Process.GetProcesses();

            foreach(Process theprocess in processlist){
                if(theprocess.ProcessName == "Tobii.TecSDK.Server")
                {
                    System.Windows.Forms.Form form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(theprocess.MainWindowHandle);
                    form.Focus();
                }
            }
        }
    }
}

