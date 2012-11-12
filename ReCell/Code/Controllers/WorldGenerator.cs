namespace Recellection.Code.Controllers
{
    using System;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Logger;

    /// <summary>
    /// This class will not be instantiated and only be called once to generate
    /// all the world tiles and place the spawn points.
    /// 
    /// Author: John Forsberg
    /// 
    /// </summary>
    public class WorldGenerator
    {
        #region Constants

        public const int MAXIMUM_MAP_SIZE = 30;

        public const int MINIMUM_MAP_SIZE = 20;

        // IGONRE FOR NOW....
        private const int MAXIMUM_SPREAD = 7;

        private const int MINIMUM_SPREAD = 3;

        #endregion

        #region Static Fields

        public static int map_cols = 0;

        public static int map_rows = 0;

        public static Logger myLogger;

        #endregion

        #region Public Methods and Operators

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

            // Constructs a new world using the dimensions.
            var returWorld = new World(tileMatrix, mapSeed);

            return returWorld;
        }

        #endregion

        #region Methods

        /// <summary>
        /// This method initiates all the tiles first and then randomly places patches 
        /// of randomly choosen terrain types.
        /// </summary>
        /// <param name="mapSeed">The seed used for the randomer.</param>
        /// <returns>The tile matrix</returns>
        private static Tile[,] GenerateTileMatrixFromSeed(int mapSeed)
        {
			myLogger.Trace("Generating tile matrix from seed " + mapSeed);
            var randomer = new Random(mapSeed);

            Tile[,] retur = InitTileMatrix(randomer);

            // The ammount of tiles that should be spread.
            int numberOfRandomTiles = randomer.Next((int)(MINIMUM_MAP_SIZE*MINIMUM_MAP_SIZE * 0.5F), (int)(MAXIMUM_MAP_SIZE*MAXIMUM_MAP_SIZE * 0.7F));

            int randomX;
            int randomY;
            int numberOfTilesToRandomize;
			myLogger.Trace("Randomly placing " + numberOfRandomTiles + " tiles");
            while (numberOfRandomTiles > 0)
            {
                // Randomly choose a tile excluding all the edge tiles.
                randomY = randomer.Next(1, map_rows - 2);

                randomX = randomer.Next(1, map_cols - 2);

                numberOfTilesToRandomize = randomer.Next(MINIMUM_SPREAD, 
                    MAXIMUM_SPREAD);

                SpreadTiles(retur, randomX, randomY, numberOfTilesToRandomize, 
                    RandomTerrainType(randomer), randomer);

                numberOfRandomTiles -= numberOfTilesToRandomize;
            }

			myLogger.Trace("Finished randomizing tiles");
            RandomPlaceResources(randomer, retur);

            return retur;
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

        /// <summary>
        /// Initiates the tile matrix with default tiles for every tile
        /// </summary>
        /// <param name="randomer">The random generator used to generate the map.</param>
        /// <returns></returns>
        private static Tile[,] InitTileMatrix(Random randomer)
        {

            map_rows = randomer.Next(MINIMUM_MAP_SIZE, MAXIMUM_MAP_SIZE);

            map_cols = randomer.Next(MINIMUM_MAP_SIZE, MAXIMUM_MAP_SIZE);

            // Construct the matrix, the size is limited by MINIMUM and MAXIMUM
            var retur = new Tile[map_cols, map_rows];


            myLogger.Info("Map consists of " + map_cols + " times "
                + map_rows + " tiles.");

            // Each row needs an array initiated each row needs to have 
            // an array of equal size.
            for (int i = 0; i < map_cols; i++)
            {
                for (int j = 0; j < map_rows ; j++)
                {
                    retur[i, j] = new Tile(i, j);
                }

            }

            return retur;
        }

        private static void RandomPlaceResources(Random randomer, Tile[,] retur)
        {
            myLogger.Trace("Randomly placing resources.");
            int randomX;
            int randomY;

            int numberOfResourceTiles = 5;

            // Place five randomly placed resourceTiles, then 
            for (int i = 0; i < numberOfResourceTiles; i++)
            {
                // Randomly choose a tile excluding all the edge tiles.
                randomY = randomer.Next(map_rows / 3, map_rows * 2 / 3);

                randomX = randomer.Next(map_cols / 3, map_cols * 2 / 3);

                retur[randomX, randomY] = new Tile(randomX, randomY, Globals.TerrainTypes.Mucus);
            }

            myLogger.Trace("Doop");
            for (int x = 0; x < 2; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    int minY = (map_rows / 5) + y * (map_rows * 2 / 3);
                    int minX = (map_cols / 5) + x * (map_cols * 2 / 3);

                    int maxY = (map_rows / 3) + map_rows * y - 2 - (map_rows / 3) * y;
                    int maxX = (map_cols / 3) + map_cols * x - 2 - (map_cols / 3) * x;
                    do
                    {
                        randomY = randomer.Next(minY, maxY);

                        randomX = randomer.Next(minX, maxX);
                    }
// Make sure that the random coordinates are not on the base building spawn point. 
                    while ((randomX == 5 || randomX == map_cols - 5) && (randomY == 5 || randomY == map_rows - 5));
					
                    retur[randomX, randomY] = new Tile(randomX, randomY, Globals.TerrainTypes.Mucus);
                }
            }

            myLogger.Trace("Finished placing resources");
        }

        /// <summary>
        /// Returns a randomly choosen terrain type, though ignoring the default tile type.
        /// </summary>
        /// <param name="randomer">The random generator used</param>
        /// <returns></returns>
        private static Globals.TerrainTypes RandomTerrainType(Random randomer)
        {
            // randomize a number which is 1 to number of terrain types - 1.
            // Ignores the default terrain type Membrane.
            int randomType = randomer.Next(2, GetNumberOfTerrainTypes());

            return (Globals.TerrainTypes)randomType;
        }

        /// <summary>
        /// This is a recurisve method that each call creates a new tile for the 
        /// specific terrain type to spread. It will call itself until the number of
        /// tiles to spread is zero or less. The spread works by randomly choose
        /// a direction as seen futher down.
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

            tileMatrix[xCoord, yCoord] = new Tile(xCoord, yCoord, type);

            // X represents the adjecent tiles, O is the current tile
            // X = 1
            // 2 = X O X = 3
            // 4 = X
            if (numberOfTiles<=0 || !(xCoord > 1 && xCoord < tileMatrix.GetLength(0)-1 && yCoord > 1 && yCoord < tileMatrix.GetLength(1)-1))
            {
                return;
            }

            switch (randomer.Next(4))
            {
                case 0:
                    SpreadTiles(tileMatrix, xCoord, yCoord - 1, 
                        numberOfTiles - 1, type, randomer);
                    break;

                case 1:
                    SpreadTiles(tileMatrix, xCoord - 1, yCoord, 
                        numberOfTiles - 1, type, randomer);
                    break;

                case 2:
                    SpreadTiles(tileMatrix, xCoord + 1, yCoord, 
                        numberOfTiles - 1, type, randomer);
                    break;

                case 3:
                    SpreadTiles(tileMatrix, xCoord, yCoord + 1, 
                        numberOfTiles - 1, type, randomer);
                    break;

            }

        }

        #endregion
    }
}
