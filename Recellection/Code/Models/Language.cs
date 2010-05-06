using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Recellection.Code.Models
{
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
        #region Singleton-stuff

        // from http://www.yoda.arachsys.com/csharp/singleton.html
        static Language instance = null;
        static readonly object padlock = new object();

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

        private String currentLanguage;
        private Dictionary<String, Dictionary<String, String>> translations;

        private const string EXTENSION = "txt";
        
        #region Constructors

        /// <summary>
        /// Create a Language object. Defaults to English.
        /// </summary>
        private Language()
        {
            this.currentLanguage = "English";
            this.translations = new Dictionary<String, Dictionary<String, String>>();
            this.ReadLanguagesFromFile();
        }

        #endregion

        /// <summary>
        /// Returns a string attached to the label you supply in the
        /// currently selected language.
        /// </summary>
        /// <param name="label">Label of the text string you want.</param>
        /// <returns>Requested text in the currently active language.</returns>
        public string GetString(string label)
        {
            return translations[currentLanguage][label];
        }
        
        /// <summary>
        /// Add a new string to the model.
        /// </summary>
        /// <param name="language">Language of the new string.</param>
        /// <param name="label">Label of the new string.</param>
        /// <param name="translation">Text to be added. (The new string.)</param>
        private void SetString(String language, String label, String translation)
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

        /// <returns>A list of currently available languages.</returns>
        public String[] GetAvailableLanguages()
        {
            return this.translations.Keys.ToArray();
        }
        
        /// <summary>
        /// Set a new active language.
        /// </summary>
        public void SetLanguage(String newLanguage)
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

        /// <returns>The language current in use.</returns>
        public String GetLanguage()
        {
            return this.currentLanguage;
        }

        /// <summary>
        /// Reads all language-files from the Content/Languages-directory.
        /// </summary>
        private void ReadLanguagesFromFile()
        {
            // Reset translations, just in case.
            this.translations = new Dictionary<string, Dictionary<string, string>>();
            
            // Get list of language-files in Content directory.
            DirectoryInfo di = new DirectoryInfo("Content/Languages");
            FileInfo[] fi = di.GetFiles("*."+EXTENSION);

            Console.Error.WriteLine(fi[0].Name);

            // Variables for looping.
            StreamReader sr;
            String language;
            String tempLine;
            String[] tmp;

            foreach (FileInfo f in fi)
            {
                sr = new StreamReader(new FileStream("Content/Languages/" + f.Name, FileMode.Open));
                if (!f.Name.Equals(""))
                {
                    language = f.Name.Split('.')[0];
                    //language = "English";

                    // Make sure the language exists
                    if (!this.translations.ContainsKey(language))
                    {
                        this.translations.Add(language, new Dictionary<string, string>());
                    }

                    while (!sr.EndOfStream)
                    {
                        tempLine = sr.ReadLine();
                        if (!(tempLine.StartsWith("[") || tempLine.Equals("") || tempLine.StartsWith(";")))
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
        /// Reloads information from language files.
        /// </summary>
        public void ReloadFromFile()
        {
            this.ReadLanguagesFromFile();
        }
        
        /// <summary>
        /// Saves all translated texts.
        /// </summary>
        [Obsolete("Language files should never be changed during runtime.")]
        private void SaveLanguagesToFile(){}
    }
}
