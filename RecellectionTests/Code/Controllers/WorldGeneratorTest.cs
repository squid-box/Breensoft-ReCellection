using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using Recellection.Code.Controllers;
using Recellection.Code.Models;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Controllers
{
    [TestFixture]
    class WorldGeneratorTest
    {

        Tile[][] tileMatrix;

        Random randomer;
        Logger myLogger;

        public const int SEED = 0xC00FEE;

        int width;


        [SetUp]
        public void Init()
        {
            myLogger = LoggerFactory.GetLogger();

            randomer = new Random(SEED);
            tileMatrix = new Tile[randomer.Next(WorldGenerator.MINIMUM,
                WorldGenerator.MAXIMUM)][];

            width = randomer.Next(WorldGenerator.MINIMUM,
                    WorldGenerator.MAXIMUM);

            myLogger.Trace("Test map consists of " + tileMatrix.Length +
                " times " + width + " tiles.");

            for (int i = 0; i < tileMatrix.Length; i++)
            {
                tileMatrix[i] = new Tile[width];
            }

            for (int i = 0; i < tileMatrix.Length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tileMatrix[i][j] = RandomTile();
                }

            }
        }

        [Test]
        public void GenerateWorld()
        {
            Tile[][] toTest = WorldGenerator.GenerateWorld(SEED);
            for (int i = 0; i < tileMatrix.Length; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Assert.IsNotNull(toTest[i][j]);

                }
            }

        }
        public Tile RandomTile()
        {

            int randomTile = randomer.Next(
                Enum.GetValues(typeof(Globals.TerrainTypes)).Length - 1);

            Type enumType = typeof(Globals.TerrainTypes);


            return new Tile(new TerrainType((Globals.TerrainTypes)
                Enum.ToObject(enumType, randomTile)));

        }
    }
}
