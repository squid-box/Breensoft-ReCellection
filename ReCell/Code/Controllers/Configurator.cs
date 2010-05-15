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
            
            mute = new MenuIcon("Mute");
            volume = new MenuIcon("Volume");
            difficulty = new MenuIcon("Difficulty");
            language = new MenuIcon("Language");
            
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

            //if (response == ??)
            //{
            // Off-screen quit option? 
            //    MenuController.UnloadMenu();
            //}(response == mute)

            if (response == mute)
            {
                if (GameOptions.Instance.musicMuted)
                {
                    GameOptions.Instance.musicMuted = false;
                }
                else
                {
                    GameOptions.Instance.musicMuted = true;
                }
                MenuController.UnloadMenu();
            }
            else if (response == volume)
            {
                // Dat volume
            }
            else if (response == language)
            {
                String[] availableLanguages = Language.Instance.GetAvailableLanguages();
                Dictionary<MenuIcon, String> languageDic = new Dictionary<MenuIcon,string>();

                foreach (String lang in availableLanguages)
                {
                    MenuIcon aLanguage = new MenuIcon(lang);
                    languageDic.Add(aLanguage, lang);
                }
                List<MenuIcon> iconList = new List<MenuIcon>(languageDic.Keys);
                Menu langMenu = new Menu(Globals.MenuLayout.NineMatrix, iconList, "Choose Language");
                MenuController.LoadMenu(langMenu);
                Recellection.CurrentState = MenuView.Instance;

                MenuIcon choosenLang = MenuController.GetInput();

                Language.Instance.SetLanguage(languageDic[response]);
                MenuController.UnloadMenu();
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

        #region volume
        #endregion

        #region language
            
        #endregion
    }

}