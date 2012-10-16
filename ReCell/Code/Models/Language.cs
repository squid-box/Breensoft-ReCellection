namespace Recellection.Code.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The purpose of the Language component is to store different translations 
    /// for strings in the application. The Language component provides 
    /// functionality for getting strings in the correct language. It does this 
    /// by providing a static function for every other component which handles 
    /// strings in the application.
	/// 
	/// Author: Joel Ahlgren
	/// Signed: Martin Nycander (2010-05-04)
    /// </summary>
    /// <date>2010-05-04</date>
    public sealed class Language : IModel
    {
        // from http://www.yoda.arachsys.com/csharp/singleton.html
        #region Constants

        private const string EXTENSION = "txt";

        #endregion

        #region Static Fields

        static readonly object padlock = new object();

        static Language instance;

        #endregion

        #region Fields

        private string currentLanguage;
        private Dictionary<string, Dictionary<string, string>> translations;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Create a Language object. Defaults to English.
        /// </summary>
        private Language()
        {
            this.currentLanguage = "Swedish";
            this.translations = new Dictionary<string, Dictionary<string, string>>();
            this.ReadLanguagesFromFile();
        }

        #endregion

        #region Public Properties

        public static Language Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new Language();
                    }

                    return instance;
                }
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <returns>A list of currently available languages.</returns>
        public string[] GetAvailableLanguages()
        {
            return this.translations.Keys.ToArray();
        }

        /// <returns>The language current in use.</returns>
        public string GetLanguage()
        {
            return this.currentLanguage;
        }

        /// <summary>
        /// Returns a string attached to the label you supply in the
        /// currently selected language.
        /// </summary>
        /// <param name="label">Label of the text string you want.</param>
        /// <returns>Requested text in the currently active language.</returns>
        public string GetString(string label)
        {
            return this.translations[this.currentLanguage][label];
        }

        /// <summary>
        /// Reloads information from language files.
        /// </summary>
        public void ReloadFromFile()
        {
            this.ReadLanguagesFromFile();
        }

        /// <summary>
        /// Set a new active language.
        /// </summary>
        public void SetLanguage(string newLanguage)
        {
            if (!this.GetAvailableLanguages().Contains(newLanguage))
            {
                throw new ArgumentException("No such language, make sure the language file is present.");
            }
            else
            {
                this.currentLanguage = newLanguage;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Reads all language-files from the Content/Languages-directory.
        /// </summary>
        private void ReadLanguagesFromFile()
        {
            // Reset translations, just in case.
            this.translations = new Dictionary<string, Dictionary<string, string>>();

            // Get list of language-files in Content directory.
            var di = new DirectoryInfo("Content/Languages");
            FileInfo[] fi = di.GetFiles("*." + EXTENSION);

            Console.Error.WriteLine(fi[0].Name);

            // Variables for looping.
            StreamReader sr;
            string language;
            string tempLine;
            string[] tmp;

            foreach (FileInfo f in fi)
            {
                sr = new StreamReader(new FileStream("Content/Languages/" + f.Name, FileMode.Open));
                if (!f.Name.Equals(string.Empty))
                {
                    language = f.Name.Split('.')[0];

                    // Make sure the language exists
                    if (!this.translations.ContainsKey(language))
                    {
                        this.translations.Add(language, new Dictionary<string, string>());
                    }

                    while (!sr.EndOfStream)
                    {
                        tempLine = sr.ReadLine();
                        tempLine = tempLine.Replace("\\n", "\n");
                        if (!(tempLine.StartsWith("[") || tempLine.Equals(string.Empty) || tempLine.StartsWith(";")))
                        {
                            tmp = tempLine.Split('=');
                            this.SetString(language, tmp[0], tmp[1]);
                        }
                    }
                }

                sr.Close();
            }
        }

        /// <summary>
        /// Saves all translated texts.
        /// </summary>
        [Obsolete("Language files should never be changed during runtime.")]
        private void SaveLanguagesToFile(){}

        /// <summary>
        /// Add a new string to the model.
        /// </summary>
        /// <param name="language">Language of the new string.</param>
        /// <param name="label">Label of the new string.</param>
        /// <param name="translation">Text to be added. (The new string.)</param>
        private void SetString(string language, string label, string translation)
        {
            if (this.translations.ContainsKey(language))
            {
                this.translations[language].Add(label, translation);
            }
            else
            {
                throw new ArgumentException("Language does not exist!");
            }
        }

        #endregion
    }
}
