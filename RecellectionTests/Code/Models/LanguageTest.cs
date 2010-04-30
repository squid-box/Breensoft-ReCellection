using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Recellection.Code.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-04-30</date>
    [TestFixture]
    class LanguageTest
    {
        Language lang;

        [SetUp]
        public void init()
        {
            
        }

        [Test]
        public void LoadTest()
        {
            try
            {
                lang = Language.Instance;
                Assert.Pass(lang.GetLanguage().ToString() + " loaded.");
                Assert.Pass();
            }
            catch(Exception e)
            {
                if (e.GetType().ToString().Equals("NUnit.Framework.SuccessException"))
                {
                    Assert.Pass();
                }
                else
                {
                    Assert.Fail(e.GetType() + " : " + e.Message);
                }
            }
        }

        [Test]
        public void ChangeLanguageTest()
        {
            lang = Language.Instance;
            lang.SetLanguage("English");
            Assert.AreEqual("Re:Cellection", lang.GetString("title"));

            lang.SetLanguage("Swedish");

            Assert.AreEqual("Åter:Urval", lang.GetString("title"));
        }
    }
}
