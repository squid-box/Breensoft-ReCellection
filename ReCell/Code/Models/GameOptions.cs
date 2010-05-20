using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Controllers;

/**
 * 
 * Author: co
 * 
 **/

namespace Recellection.Code.Models
{
    /// <summary>
    /// This model is used to save all the options choices.
    /// </summary>
    public sealed class GameOptions : IModel
    {

        #region Singleton-stuff

        // from http://www.yoda.arachsys.com/csharp/singleton.html
        static GameOptions instance = null;
        static readonly object padlock = new object();

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

        public GameOptions()
        {
            musicVolume = 1.0f;
            sfxVolume = 1.0f;
            this.choosenLanguage = new Localizer();
        }

        #endregion

        public Globals.Difficulty difficulty { get; set; }
        public float musicVolume { get; set; }
        public bool musicMuted { get; set; }

        public float sfxVolume { get; set; }
        public bool sfxMuted { get; set; }

        private Localizer choosenLanguage;

        public void SetLanguage(String language)
        {
            choosenLanguage.SetLanguage(language);
        }
    }
}
