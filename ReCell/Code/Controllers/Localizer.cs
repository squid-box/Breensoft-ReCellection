namespace Recellection.Code.Controllers
{
    using System;

    using global::Recellection.Code.Models;

    /// <summary>
    /// The purpose of the Localizer is to load the translations into the 
    /// Language model, it will also set the current language for the application.
	/// 
	/// Signature: Martin Nycander (2010-05-05)
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-05-04</date>
    public class Localizer
    {
        // Keeps track of languages
        #region Fields

        private readonly Language languageModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructs a Localizer.
        /// </summary>
        public Localizer()
        {
            this.languageModel = Language.Instance;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Changes the language
        /// </summary>
        /// <param name="language">New language to set.</param>
        public void SetLanguage(string language)
        {
            this.languageModel.SetLanguage(language);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads all strings into the language model
        /// </summary>
        private void LoadTranslations()
        {
            this.languageModel.ReloadFromFile();
        }

        #endregion
    }
}
