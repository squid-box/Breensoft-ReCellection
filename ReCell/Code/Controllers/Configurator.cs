using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Utility.Logger;

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
        private static MenuIcon back;
        private static List<MenuIcon> iconList;

        static Configurator()
        {
            Instance = new Configurator();
            
            mute = new MenuIcon("Mute");
            volume = new MenuIcon("Volume");
            difficulty = new MenuIcon("Difficulty");
            language = new MenuIcon("Language");
            back = new MenuIcon("Back");
            
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
                iconList.Add(back);
            }
            return new Menu(Globals.MenuLayout.NineMatrix, iconList, "Options", Color.Black);
        }

        public void ChangeOptions()
        {
            Menu optionsMenu = BuildMenu();
            MenuController.LoadMenu(optionsMenu);

            Recellection.CurrentState = MenuView.Instance;
            bool notFinished = true;
            while (notFinished)
            {
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
                        SoundsController.changeEffectsVolume(GameOptions.Instance.sfxVolume);
                        SoundsController.changeMusicVolume(GameOptions.Instance.musicVolume);
                    }
                    else
                    {
                        GameOptions.Instance.musicMuted = true;
                        SoundsController.changeEffectsVolume(0.0f);
                        SoundsController.changeMusicVolume(0.0f);
                    }
                    MenuController.UnloadMenu();
                }
                else if (response == volume)
                {
                    // Dat volume
                }
                else if (response == language)
                {
                    ChangeLanguageMenu();
                }
                else if (response == difficulty)
                {
                    ChangeDifficultyMenu();
                }
                else if (response == back)
                {
                    notFinished = false;
                }
            }
            MenuController.UnloadMenu();
        }

        /// <summary>
        /// Builds a Menu for changing languages and processes the result.
        /// </summary>
        private void ChangeLanguageMenu()
        {
            String[] availableLanguages = Language.Instance.GetAvailableLanguages();

            #if DEBUG
            LoggerFactory.GetLogger().Info("Available Languages:");
            #endif

            Dictionary<MenuIcon, String> languageDic = new Dictionary<MenuIcon, String>();

            foreach (String lang in availableLanguages)
            {
                #if DEBUG
                LoggerFactory.GetLogger().Info(lang);
                #endif

                MenuIcon aLanguage = new MenuIcon(lang);
                languageDic.Add(aLanguage, lang);
            }

            List<MenuIcon> iconList = new List<MenuIcon>(languageDic.Keys);
            Menu langMenu = new Menu(Globals.MenuLayout.NineMatrix, iconList, "Choose Language");
            MenuController.LoadMenu(langMenu);
            Recellection.CurrentState = MenuView.Instance;

            MenuIcon choosenLang = MenuController.GetInput();

            Language.Instance.SetLanguage(languageDic[choosenLang]);
            LoggerFactory.GetLogger().Info("Language set to " + languageDic[choosenLang]);
            MenuController.UnloadMenu();
        }

        private void ChangeDifficultyMenu()
        {
            Dictionary<MenuIcon, String> difficultyDic = new Dictionary<MenuIcon, String>();

            #if DEBUG
            LoggerFactory.GetLogger().Info("Available difficulties:");
            #endif

            foreach(String diff in System.Enum.GetNames(typeof(Globals.Difficulty)))
            {
                #if DEBUG
                LoggerFactory.GetLogger().Info(diff);
                #endif

                MenuIcon aDifficulty = new MenuIcon(diff);
                difficultyDic.Add(aDifficulty, diff);
            }

            List<MenuIcon> iconList = new List<MenuIcon>(difficultyDic.Keys);
            Menu diffMenu = new Menu(Globals.MenuLayout.NineMatrix, iconList, "Choose a Difficulty.");
            MenuController.LoadMenu(diffMenu);
            Recellection.CurrentState = MenuView.Instance;

            MenuIcon choosenDiff = MenuController.GetInput();

            GameOptions.Instance.difficulty = (Globals.Difficulty) Enum.Parse(typeof(Globals.Difficulty), difficultyDic[choosenDiff]);
            LoggerFactory.GetLogger().Info("Difficulty set to " + difficultyDic[choosenDiff]);
            MenuController.UnloadMenu();
        }
    }

}