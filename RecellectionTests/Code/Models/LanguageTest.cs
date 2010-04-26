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
    /// <date>2010-04-13</date>
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
            }
            catch(Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
    }
}
