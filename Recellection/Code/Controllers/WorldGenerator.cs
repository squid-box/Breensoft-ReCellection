using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Recellection.Code.Utility.Logger;
using System.Windows;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// This class will not be instantiated and only be called once to generate
    /// all the world tiles and place the spawn points.
    /// 
    /// Author: John Forsberg
    /// </summary>
    public class WorldGenerator
    {
        public const int MINIMUM_MAP_SIZE = 100;
        public const int MAXIMUM_MAP_SIZE = 200;

        //IGONRE FOR NOW....
        private const int MINIMUM_SPREAD = 3;
        private const int MAXIMUM_SPREAD = 7;

        private const double MIN_DISTANCE_BETWEEN_PLAYERS = 55.55;

        public static Logger myLogger;

        public static int map_rows = 0;

        public static int map_cols = 0;


        /// <summary>
        /// Random generates a world of tiles and places the players
        /// spawnpoints. The same seeds generates the same world.
        /// 
        /// </summary>
        /// <param name="mapSeed">
        /// The seed that the random generater will use.</param>
        /// <returns>The newly initiated world</returns>
        public static World GenerateWorld(int mapSeed) 
        {
            myLogger = LoggerFactory.GetLogger();

            Tile[,] tileMatrix = GenerateTileMatrixFromSeed(mapSeed);


            //Constructs a new world using the dimensions.
            World returWorld = new World(map_rows,map_cols);

            //Sets the map for the world
            returWorld.SetMap(tileMatrix,map_rows,map_cols);
            //TODO Wack'a'Marco
            return returWorld;
        }

        /// <summary>
        /// Fills the tile matrix with random tiles.
        /// </summary>
        /// <param name="mapSeed">The seed the random generator uses.</param>
        /// <returns>A Tile matrix filled with random tiles.</returns>
        private static Tile[,] GenerateTileMatrixFromSeed(int mapSeed)
        {

            myLogger.Info("Seed used to generate the world is:\t" + mapSeed);
            //Initiate the random number generator
            Random randomer = new Random(mapSeed);

            //Init the tile matrix
            Tile[,] retur = InitTileMatrix(randomer);

            for (int i = 0; i < map_rows; i++)
            {
                for (int j = 0; j < map_cols; j++)
                {
                    retur[i,j] = RandomTile(randomer);
                }

            }


            return retur;
        }
        /// <summary>
        /// IGNORE, WORK IN PROGRESS, MIGHT NOT REMAIN!
        /// </summary>
        /// <param name="mapSeed"></param>
        /// <returns></returns>
        private static Tile[,] GenerateTileMatrixFromSeed2(int mapSeed)
        {
            Random randomer = new Random(mapSeed);

            Tile[,] retur = InitTileMatrix2(randomer);

            
            int numberOfRandomTiles = randomer.Next(50, 100);

            int randomX;
            int randomY;
            int numberOfTilesToRandomize;

            while (numberOfRandomTiles > 0)
            {
                randomY = randomer.Next(10,map_rows-10);

                randomX = randomer.Next(10,map_cols-10);


                numberOfTilesToRandomize = randomer.Next(MINIMUM_SPREAD, 
                    MAXIMUM_SPREAD);


                SpreadTiles(retur, randomX, randomY, numberOfTilesToRandomize,
                    RandomTerrainType(randomer), randomer);

                numberOfRandomTiles -= numberOfTilesToRandomize;
            }

            return retur;
        }

        /// <summary>
        /// Initiates the tile matrix, though no Tiles in the matrix
        /// is initiated 
        /// </summary>
        /// <param name="randomer"></param>
        /// <returns>Returns a initiated Tile Matrix</returns>
        private static Tile[,] InitTileMatrix(Random randomer)
        {


            map_rows = randomer.Next(MINIMUM_MAP_SIZE, MAXIMUM_MAP_SIZE);

            map_cols = randomer.Next(MINIMUM_MAP_SIZE, MAXIMUM_MAP_SIZE);

            //Construct the matrix, the size is limited by MINIMUM and MAXIMUM
            Tile[,] retur = new Tile[map_rows, map_cols];

            myLogger.Info("Map consists of " + map_rows + " times "
                + map_cols + " tiles.");

            return retur;
        }

        /// <summary>
        /// Initiates the tile matrix with default tiles for every tile
        /// </summary>
        /// <param name="randomer"></param>
        /// <returns></returns>
        private static Tile[,] InitTileMatrix2(Random randomer)
        {



            map_rows = randomer.Next(MINIMUM_MAP_SIZE, MAXIMUM_MAP_SIZE);

            map_cols = randomer.Next(MINIMUM_MAP_SIZE, MAXIMUM_MAP_SIZE);

            //Construct the matrix, the size is limited by MINIMUM and MAXIMUM
            Tile[,] retur = new Tile[map_rows, map_cols];


            myLogger.Info("Map consists of " + map_rows + " times "
                + map_cols + " tiles.");

            //Each row needs an array initiated each row needs to have 
            //an array of equal size.
            for (int i = 0; i < map_rows; i++)
            {
                for (int j = 0; j < map_cols; j++)
                {
                    retur[i,j] = new Tile();
                }

            }

            return retur;
        }

        /// <summary>
        /// Random generates a spawn point for both players,
        /// it makes sure that the distance between them is 
        /// more then the MIN_DISTANCE_BETWEEN_PLAYERS constant.
        /// </summary>
        /// <param name="players">The players which the spawn point will be 
        /// set for.</param>
        /// <param name="width">The width of the map.</param>
        /// <param name="heigth">The height of the map.</param>
        /// <param name="randomer">The random generator to use.</param>
        private static void SpawnPoints(List<Player> players, 
            int width, int heigth,Random randomer)
        {
            //Make sure the first player can place its spawn point.
            int previousXCoord = Int32.MinValue;
            int previousYCoord = Int32.MinValue;
            
            int randomX = randomer.Next(10, width - 10);
            int randomY = randomer.Next(10, heigth - 10);

            foreach(Player player in players)
            {
                //Calculate the length of the vector between the new spawn
                //point and the last one.
                while ( DistanceBetweenPoints(previousXCoord,previousYCoord,
                    randomX,randomY) < MIN_DISTANCE_BETWEEN_PLAYERS)
                {
                    randomX = randomer.Next(10, width - 10);
                    randomY = randomer.Next(10, heigth - 10);
                }

                SpawnGraph(randomX, randomY, player);

                previousXCoord = randomX;
                previousYCoord = randomY;
            }
        }

        private static double DistanceBetweenPoints(int x1, int y1, int x2, 
            int y2)
        {
          return Math.Sqrt((x1 - x2) ^ 2 + (y1 - y2) ^ 2);
        }

        /// <summary>
        /// Creates a new BaseBuilding at the specified location and
        /// add that building to a new Graph and then add that graph
        /// to the player.
        /// </summary>
        /// <param name="xCoord">The x-coordinate to spawn the BaseBuilding on
        /// </param>
        /// <param name="yCoord">The Y-coordinate to spawn the BaseBuilding on
        /// </param>
        /// <param name="owner">The player which this method will create a new 
        /// graph.</param>
        private static void SpawnGraph(int xCoord, int yCoord, Player owner)
        {
            owner.AddGraph(new Graph(
                new BaseBuilding("base", xCoord, yCoord, 10, owner)));
        }


        /// <summary>
        /// This method constructs a random tile, it can be any of the
        /// terrain types specified in Globals.TerrainType.
        /// </summary>
        /// <param name="randomer">The random generator used</param>
        /// <returns>A newly constructed Tile</returns>
        private static Tile RandomTile(Random randomer)
        {
            //randomize a number which is 0 to number of terrain types - 1.
            int randomTile = randomer.Next(GetNumberOfTerrainTypes());
           

            return new Tile((Globals.TerrainTypes)randomTile);
        }

        /// <summary>
        /// IGNORE, WORK IN PROGRESS; MIGHT NOT REMAIN!
        /// </summary>
        /// <param name="randomer"></param>
        /// <returns></returns>
        private static Globals.TerrainTypes RandomTerrainType(Random randomer)
        {
            //randomize a number which is 1 to number of terrain types - 1.
            //Ignores the default terrain type Membrane.
            int randomType = randomer.Next(1,GetNumberOfTerrainTypes());


            
            return (Globals.TerrainTypes)randomType;
        }

        /// <summary>
        /// IGNORE, WORK IN PROGRESS, MIGHT NOT REMAIN!
        /// </summary>
        /// <param name="tileMatrix"></param>
        /// <param name="xCoord"></param>
        /// <param name="yCoord"></param>
        /// <param name="numberOfTiles"></param>
        /// <param name="type"></param>
        /// <param name="randomer"></param>
        private static void SpreadTiles(Tile[,] tileMatrix, int xCoord, 
            int yCoord, int numberOfTiles, Globals.TerrainTypes type, 
            Random randomer)
        {

            tileMatrix[yCoord,xCoord] = new Tile(type);

            //4 represents the adjecent tiles
            //      X = 1
            //2 = X O X = 3
            //  4 = X
            //
            switch (randomer.Next(4))
            {
                case 1:
                    SpreadTiles(tileMatrix, xCoord, yCoord - 1, 
                        numberOfTiles - 1, type, randomer);
                    break;

                case 2:
                    SpreadTiles(tileMatrix, xCoord - 1, yCoord,
                        numberOfTiles - 1, type, randomer);
                    break;

                case 3:
                    SpreadTiles(tileMatrix, xCoord + 1, yCoord,
                        numberOfTiles - 1, type, randomer);
                    break;

                case 4:
                    SpreadTiles(tileMatrix, xCoord, yCoord + 1,
                        numberOfTiles - 1, type, randomer);
                    break;

            }

        }

        /// <summary>
        /// This method converts one int to the correct enum in 
        /// Globals.TerrainTypes.
        /// </summary>
        /// <param name="type">The int that corresponds to a Terrain Type enum
        /// in Globals.TerrainTypes.</param>
        /// <returns>The Terrain Type enum</returns>
        private static Globals.TerrainTypes 
            GetTerrainTypeEnumFromInt(int type)
        {
            Type enumType = typeof(Globals.TerrainTypes);


            return (Globals.TerrainTypes)
                Enum.ToObject(enumType, type);
        }

        /// <summary>
        /// This method counts how many enums there is in Globals.TerrainTypes
        /// </summary>
        /// <returns>The number of Terrain Type enums in Globals.TerrainTypes
        /// </returns>
        private static int GetNumberOfTerrainTypes()
        {
            return Enum.GetValues(typeof(Globals.TerrainTypes)).Length;
        }
    }
}
