using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Recellection.Code.Main.Utility;

namespace Recellection.Code.Controllers
{
    public class WorldGenerator
    {
        public const int MINIMUM = 100;
        public const int MAXIMUM = 200;
        public static Logger myLogger;


        /// <summary>
        /// Random generates a world of tiles and places the players
        /// spawnpoints. The same seeds generates the same world.
        /// 
        /// </summary>
        /// <param name="mapSeed">
        /// The seed that the random generater will use.</param>
        /// <returns>TODO Make it return a World</returns>
        public static Tile[][] GenerateWorld(int mapSeed) 
        {
            Tile[][] tileMatrix = GenerateTileMatrixFromSeed(mapSeed);
            //TODO make a world from the tiles.

            myLogger = LoggerFactory.GetLogger();

            //TODO Set spawnpoint, easiest if it is, 10 tiles from the corner
            //For a 100,100 map that means it is at 10,10 and 90,90

            return tileMatrix;
        }

        private static Tile[][] InitTileMatrix(Random randomer)
        {
            

            
            //Construct the matrix, the size is limited by MINIMUM and MAXIMUM
            Tile[][] retur = new Tile
                [randomer.Next(MINIMUM, MAXIMUM)][];

            int width = randomer.Next(MINIMUM, MAXIMUM);

            myLogger.Trace("Map consists of " + retur.Length + " times "
                + width + " tiles.");

            //Each row needs an array initiated each row needs to have 
            //an array of equal size.
            for (int i = 0; i < retur.Length; i++)
            {
                retur[i] = new Tile[width];

            }

            return retur;
        }
        private static Tile[][] GenerateTileMatrixFromSeed(int mapSeed)
        {

            myLogger.Trace("Seed used to generate the world is:\t" + mapSeed);
            //Initiate the random number generator
            Random randomer = new Random(mapSeed);


            Tile[][] retur = InitTileMatrix(randomer);

            for (int i = 0; i < retur.Length; i++)
            {
                for (int j = 0; j < retur[i].Length; j++)
                {
                    retur[i][j] = RandomTile(randomer);
                }

            }


            return retur;
        }


        private static Tile RandomTile(Random randomer)
        {
            //randomize a number which is 0 to number of terrain types - 1.
            int randomTile = randomer.Next(GetNumberOfTerrainTypes()  - 1);
           
            //This is aperantly the best way to determine how many 
            //different enums there is

            return new Tile(new TerrainType(
                GetTerrainTypeEnumFromInt(randomTile)));
        }

        /**
         * This method converts one int to the correct enum in 
         * Globals.TerrainTypes.
         * 
         * TODO Decide if this should be in the Globals class.
         */
        private static Globals.TerrainTypes 
            GetTerrainTypeEnumFromInt(int type)
        {
            Type enumType = typeof(Globals.TerrainTypes);


            return (Globals.TerrainTypes)
                Enum.ToObject(enumType, type);
        }

        private static int GetNumberOfTerrainTypes()
        {
            return Enum.GetValues(typeof(Globals.TerrainTypes)).Length;
        }
    }
}
