using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
    class WorldGenerator
    {
        public const int MINIMUM = 100;
        public const int MAXIMUM = 200;


        public static Tile[][] GenerateWorld(int mapSeed) 
        {
            Tile[][] tileMatrix = GenerateTileMatrixFromSeed(mapSeed);
            //TODO make a world from the tiles.


            //TODO Set spawnpoint, easiest if it is, 10 tiles from the corner
            //For a 100,100 map that means it is at 10,10 and 90,90

            return tileMatrix;
        }
        private static Tile[][] GenerateTileMatrixFromSeed(int mapSeed)
        {
            //Initiate the random number generator
            Random randomer = new Random(mapSeed);

            //Construct the matrix, the size is limited by MINIMUM and MAXIMUM
            Tile[][] retur = new Tile
                [randomer.Next(MINIMUM, MAXIMUM)][];
            //Each row needs an array initiated.
            for(int i = 0; i < retur.Length; i++)
            {
                retur[i] = new Tile[randomer.Next(MINIMUM, MAXIMUM)];

            }

            foreach (Tile[] tileRow in retur)
            {
                for (int i = 0; i < tileRow.Length; i++ )
                {
                    tileRow[i] = RandomTile(randomer);
                }

            }


            return retur;
        }


        private static Tile RandomTile(Random randomer)
        {

            Array terrainTypes = Enum.GetValues(typeof(Globals.TerrainTypes));
            
            //TODO Use random to create diffrent types.
            int randomTile = randomer.Next(
                Enum.GetValues(typeof(Globals.TerrainTypes)).Length);

            return new Tile(new TerrainType());//new TerrainType(terrainTypes.GetValue[randomTile]));
        }
            
    }
}
