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
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-16</date>
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

        /// <summary>
        /// Create a Language object. Preferably only once...
        /// </summary>
        /// <param name="language">Current language to use.</param>
        public Language(String language)
        {
            this.currentLanguage = language;
            this.translations = new Dictionary<String, Dictionary<String, String>>();
            this.ReadLanguagesFromFile();
        }

        #endregion


        public string GetString(string label)
        {
            return translations[currentLanguage][label];
        }
        
        // Adding new strings to the model
        public void SetString(String language, string label, string translation)
        {
            this.translations[language].Add(label, translation);
        }
        
        // Changing the language
        public void SetLanguage(String newLanguage)
        {
            this.currentLanguage = newLanguage;
        }

        public String GetLanguage()
        {
            return this.currentLanguage;
        }


        private void ReadLanguagesFromFile()
        {
            // Get list of language-files in Content directory.
            DirectoryInfo di = new DirectoryInfo("Content/Languages");
            FileInfo[] fi = di.GetFiles("*."+EXTENSION);

            // Variables for looping.
            StreamReader sr;
            String language;
            String[] tmp;

            foreach (FileInfo f in fi)
            {
                sr = new StreamReader(new FileStream("Content/Languages/" + f.Name, FileMode.Open));
                language = f.Name.Split('.')[0];

                // Make sure the language exists
                if(this.translations[language] == null)
                {
                    this.translations.Add(language, new Dictionary<string,string>());
                }
                
                while (!sr.EndOfStream)
                {
                    tmp = sr.ReadLine().Split('=');
                    translations[language].Add(tmp[0], tmp[1]);
                }
            }
        }
        
        /// <summary>
        /// Saves all translated texts.
        /// </summary>
        [Obsolete("Language files should never be changed during runtime.")]
        private void SaveLanguagesToFile()
        {
            
        }
    }
}
