namespace Recellection.Code.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Microsoft.Xna.Framework;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Logger;

    using global::Recellection.Code.Views;

    /// <summary>
    /// Handles state management for the GameOptions Model.
    /// 
    /// Author: Mattias Mikkola
    /// </summary>
    public sealed class Configurator
    {
        #region Static Fields

        private static readonly MenuIcon back;
		private static readonly MenuIcon credits;

        private static readonly MenuIcon difficulty;

        private static readonly List<MenuIcon> iconList;

        private static readonly MenuIcon language;

        private static readonly MenuIcon mute;
        private static readonly MenuIcon volume;

        #endregion

        #region Constructors and Destructors

        static Configurator()
        {
            Instance = new Configurator();
            
            mute = new MenuIcon(Language.Instance.GetString("Mute"));
            volume = new MenuIcon(Language.Instance.GetString("Volume"));
            difficulty = new MenuIcon(Language.Instance.GetString("Difficulty"));
            language = new MenuIcon(Language.Instance.GetString("Language"));
			credits = new MenuIcon(Language.Instance.GetString("Credits"));
            back = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
            
            iconList = new List<MenuIcon>();
        }

        #endregion

        #region Public Properties

        public static Configurator Instance { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void ChangeOptions()
        {
            Menu optionsMenu = this.BuildMenu();
            MenuController.LoadMenu(optionsMenu);

            Recellection.CurrentState = MenuView.Instance;
            bool notFinished = true;
            while (notFinished)
            {
                MenuIcon response = MenuController.GetInput();

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
                }
                else if (response == volume)
                {
                    this.ChangeVolumeMenu();   
                }
                else if (response == language)
                {
                    this.ChangeLanguageMenu();
                }
                else if (response == difficulty)
                {
                    this.ChangeDifficultyMenu();
                }
                else if (response == back)
                {
                    notFinished = false;
                }
				else if (response == credits)
				{
					this.PlayCredits();
				}
            }

            MenuController.UnloadMenu();
        }

        #endregion

        #region Methods

        private Menu BuildMenu()
        {
            if (iconList.Count == 0)
            {
                iconList.Add(mute);
                iconList.Add(volume);
                iconList.Add(difficulty);
                iconList.Add(language);
                iconList.Add(back);
                iconList.Add(credits);
            }

            return new Menu(Globals.MenuLayout.NineMatrix, iconList, Language.Instance.GetString("Options"), Color.Black);
        }

        private void ChangeDifficultyMenu()
        {
            var difficultyDic = new Dictionary<MenuIcon, string>();

            #if DEBUG
            LoggerFactory.GetLogger().Info("Available difficulties:");
            #endif

            foreach(string diff in System.Enum.GetNames(typeof(Globals.Difficulty)))
            {
                #if DEBUG
                LoggerFactory.GetLogger().Info(diff);
                #endif

                var aDifficulty = new MenuIcon(diff);
                difficultyDic.Add(aDifficulty, diff);
            }

            var iconList = new List<MenuIcon>(difficultyDic.Keys);
            
            var cancel = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
            iconList.Add(cancel);
            
            var diffMenu = new Menu(Globals.MenuLayout.FourMatrix, iconList, Language.Instance.GetString("ChooseADifficulty"));

            MenuController.LoadMenu(diffMenu);
            Recellection.CurrentState = MenuView.Instance;

            MenuIcon choosenDiff = MenuController.GetInput();
			
			if (choosenDiff != cancel)
			{
				GameOptions.Instance.difficulty = (Globals.Difficulty) Enum.Parse(typeof(Globals.Difficulty), difficultyDic[choosenDiff]);
				LoggerFactory.GetLogger().Info("Difficulty set to " + difficultyDic[choosenDiff]);
			}

            MenuController.UnloadMenu();
        }

        /// <summary>
        /// Builds a Menu for changing languages and processes the result.
        /// </summary>
        private void ChangeLanguageMenu()
        {
            string[] availableLanguages = Language.Instance.GetAvailableLanguages();

#if DEBUG
            LoggerFactory.GetLogger().Info("Available Languages:");
#endif

            var languageDic = new Dictionary<MenuIcon, string>();

            foreach (string lang in availableLanguages)
            {
#if DEBUG
                LoggerFactory.GetLogger().Info(lang);
#endif

                var aLanguage = new MenuIcon(lang);
                languageDic.Add(aLanguage, lang);
            }

            var iconList = new List<MenuIcon>(languageDic.Keys);
			
            var cancel = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));
            iconList.Add(cancel);
			
            var langMenu = new Menu(Globals.MenuLayout.NineMatrix, iconList, Language.Instance.GetString("ChooseLanguage"));

            MenuController.LoadMenu(langMenu);
            Recellection.CurrentState = MenuView.Instance;

            MenuIcon choosenLang = MenuController.GetInput();
            if (choosenLang != cancel)
            {
                Language.Instance.SetLanguage(languageDic[choosenLang]);
                LoggerFactory.GetLogger().Info("Language set to " + languageDic[choosenLang]);
            }

            MenuController.UnloadMenu();
        }

        private void ChangeVolumeMenu()
        {

            var musicVolumeUp = new MenuIcon(Language.Instance.GetString("MusicVolumeUp"));
            var musicVolumeDown = new MenuIcon(Language.Instance.GetString("MusicVolumeDown"));
            var sfxVolumeUp = new MenuIcon(Language.Instance.GetString("EffectsVolumeUp"));
            var sfxVolumeDown = new MenuIcon(Language.Instance.GetString("EffectsVolumeDown"));
            var empty = new MenuIcon(string.Empty);
            var done = new MenuIcon(Language.Instance.GetString("Cancel"), Recellection.textureMap.GetTexture(Globals.TextureTypes.No));

            var iconList = new List<MenuIcon>(); 
            iconList.Add(musicVolumeUp);
            iconList.Add(empty);
            iconList.Add(musicVolumeDown);
            iconList.Add(sfxVolumeUp);
            iconList.Add(sfxVolumeDown);
            iconList.Add(done);

            var volumeMenu = new Menu(Globals.MenuLayout.NineMatrix, iconList, string.Empty);

            MenuController.LoadMenu(volumeMenu);

            bool notFinished = true;

            while (notFinished)
            {
                MenuIcon response = MenuController.GetInput();

                if (response == musicVolumeUp)
                {
                    if (GameOptions.Instance.musicVolume <= 1.0f)
                    {
                        SoundsController.changeMusicVolume(GameOptions.Instance.musicVolume + 0.1f);
                    }
                }
                else if (response == musicVolumeDown)
                {
                    SoundsController.changeMusicVolume(GameOptions.Instance.musicVolume - 0.1f);
                }
                else if (response == sfxVolumeUp)
                {
                    if (GameOptions.Instance.sfxVolume <= 1.0f)
                    {
                        SoundsController.changeEffectsVolume(GameOptions.Instance.sfxVolume + 0.1f);
                    }
                }
                else if (response == sfxVolumeDown)
                {
                    SoundsController.changeEffectsVolume(GameOptions.Instance.sfxVolume - 0.1f);
                }
                else if (response == done)
                {
                    notFinished = false;
                }
            }

            MenuController.UnloadMenu();
        }

        private void PlayCredits()
        {
            var credits = new CreditsView();

            IView temp = Recellection.CurrentState;
            Recellection.CurrentState = credits;
			
            while (!credits.Finished)
            {
                Thread.Sleep(10);
            }

            Thread.Sleep(1000);
            Recellection.CurrentState = temp;

        }

        #endregion
    }

}