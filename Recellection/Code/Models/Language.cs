using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public Language(Globals.Languages language)
        {
            this.currentLanguage = language;
            this.translations = new Dictionary<Globals.Languages, Dictionary<string, string>>();
            foreach (Globals.Languages lang in Enum.GetValues(typeof(Globals.Languages)))
            {
                // TODO: CREATE STUFFS
                this.translations.Add(lang, new Dictionary<string, string>());
            }
        }

        #endregion


        public static string GetString(string label)
        {
            return null;
        }
        
        // Adding new strings to the model
        public void SetString(string label, Type type, string translation)
        {

        }
        
        // Changing the language
        public void SetLanguage(Language newLanguage)
        {

        }

        public Language GetLanguage()
        {
            return null;
        }
    }
}
