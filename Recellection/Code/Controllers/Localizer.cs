using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// The purpose of the Localizer is to load the translations into the 
    /// Language model, it will also set the current language for the application.
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-05-04</date>
    public class Localizer
    {
        // Keeps track of languages
        private Language languageModel;

        /// <summary>
        /// Constructs a Localizer.
        /// </summary>
        public Localizer()
        {
            this.languageModel = Language.Instance;
        }

        /// <summary>
        /// Loads all strings into the language model
        /// </summary>
        private void LoadTranslations()
        {
            languageModel.ReloadFromFile();
        }
        
        /// <summary>
        /// Changes the language
        /// </summary>
        /// <param name="language">New language to set.</param>
        public void SetLanguage(String language)
        {
            languageModel.SetLanguage(language);
        }
    }
}
