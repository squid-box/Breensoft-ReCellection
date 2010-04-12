using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Recellection.Code.Controllers;
using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
    [TestFixture]
    class WorldGeneratorTest
    {

        Tile[][] tileMatrix;
        Tile defaultTile;
        Random randomer;

        public const int MINIMUM = 100;
        public const int MAXIMUM = 200;


        [SetUp]
        public void Init()
        {
            randomer = new Random(1337);
           /* tileMatrix = new Tile[randomer.Next(MINIMUM, MAXIMUM)][];

            defaultTile = new Tile(

            for (int i = 0; i < tileMatrix.Length; i++)
            {
                tileMatrix[i] = new Tile[randomer.Next(MINIMUM, MAXIMUM)];
            }
            foreach (Tile[] tileRow in tileMatrix)
            {
                for (int i = 0; i < tileRow.Length; i++)
                {
                    tileRow[i] = 
                }

            }*/
        }

        [Test]
        public void RandomTile()
        {

        }
    }
}
