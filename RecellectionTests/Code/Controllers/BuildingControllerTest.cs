using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Controllers
{
    [TestFixture]
    class BuildingControllerTest
    {
        World testWorld;


        [SetUp]
        public void Init()
        {
            testWorld = WorldGenerator.GenerateWorld(0);
        }

        [Test]
        public void CreateControlZone()
        {

        }
    }
}
