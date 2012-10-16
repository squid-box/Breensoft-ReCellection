/**
 * 
 * Author: co
 * 
 **/

namespace Recellection.Code.Models
{
    using System;

    using global::Recellection.Code.Controllers;

    /// <summary>
    /// This model is used to save all the options choices.
    /// </summary>
    public sealed class GameOptions : IModel
    {
        // from http://www.yoda.arachsys.com/csharp/singleton.html
        #region Static Fields

        static readonly object padlock = new object();

        static GameOptions instance;

        #endregion

        #region Fields

        private readonly Localizer choosenLanguage;

        #endregion

        #region Constructors and Destructors

        public GameOptions()
        {
            this.musicVolume = 1.0f;
            this.sfxVolume = 1.0f;
            this.choosenLanguage = new Localizer();
        }

        #endregion

        #region Public Properties

        public static GameOptions Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameOptions();
                    }

                    return instance;
                }
            }
        }

        public Globals.Difficulty difficulty { get; set; }

        public bool musicMuted { get; set; }

        public float musicVolume { get; set; }

        public bool sfxMuted { get; set; }

        public float sfxVolume { get; set; }

        #endregion

        #region Public Methods and Operators

        public void SetLanguage(string language)
        {
            this.choosenLanguage.SetLanguage(language);
        }

        #endregion
    }
}
