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
    public class Language : IModel
    {
        private Globals.Languages currentLanguage;
        private Dictionary<Globals.Languages, Dictionary<String, String>> translations;

        #region Constructors

        /// <summary>
        /// Create a Language object. Preferably only once...
        /// </summary>
        /// <param name="language">Current language to use.</param>
        public Language(Globals.Languages language)
        {
            this.currentLanguage = language;
            this.translations = new Dictionary<Globals.Languages, Dictionary<string, string>>();
            foreach (Globals.Languages lang in Enum.GetValues(typeof(Globals.Languages)))
            {
                this.translations.Add(lang, new Dictionary<string, string>());
            }
        }

        #endregion


        public string GetString(string label)
        {
            return translations[currentLanguage][label];
        }
        
        // Adding new strings to the model
        public void SetString(string label, Globals.Languages type, string translation)
        {
            this.translations[type].Add(label, translation);
        }
        
        // Changing the language
        public void SetLanguage(Globals.Languages newLanguage)
        {
            this.currentLanguage = newLanguage;
        }

        public Globals.Languages GetLanguage()
        {
            return this.currentLanguage;
        }


        private void ReadLanguagesFromFile()
        {
            // TODO: Fill translations with text.
        }
    }
}
