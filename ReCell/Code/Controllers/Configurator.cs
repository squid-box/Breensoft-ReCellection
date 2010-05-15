using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// Handles state management for the GameOptions Model.
    /// 
    /// Author: Mattias Mikkola
    /// </summary>
    public sealed class Configurator
    {
        public static Configurator Instance { get; private set; }
        private static MenuIcon mute;
        private static MenuIcon volume;
        private static MenuIcon difficulty;
        private static MenuIcon language;
        private static List<MenuIcon> iconList;

        static Configurator()
        {
            Instance = new Configurator();
            
            mute = new MenuIcon("Mute", null, Color.Black);
            volume = new MenuIcon("Volume", null, Color.Black);
            difficulty = new MenuIcon("Difficulty", null, Color.Black);
            language = new MenuIcon("Language", null, Color.Black);
            
            iconList = new List<MenuIcon>();
            
            
        }

        private Menu BuildMenu()
        {
            if (iconList.Count == 0)
            {
                iconList.Add(mute);
                iconList.Add(volume);
                iconList.Add(difficulty);
                iconList.Add(language);
            }
            return new Menu(Globals.MenuLayout.FourMatrix, iconList, "Options", Color.Black);
        }

        public void ChangeOptions()
        {
            Menu optionsMenu = BuildMenu();
            MenuController.LoadMenu(optionsMenu);

            Recellection.CurrentState = MenuView.Instance;

            MenuIcon response = MenuController.GetInput();

            if (response == mute)
            {
            }
            //else if (response == ??)
            //{
            // Off-screen quit option? 
            //    MenuController.UnloadMenu();
            //}
            else if (response == volume)
            {
                // ChangeVolumeMenu()
            }
            else if (response == language)
            {
                // ChangeLanguageMenu()
            }
            else if (response == difficulty)
            {
                // ChangeDifficultyMenu()
            }
            else
            {
                // What?
            }
        }

    }

}