namespace Recellection.Code.Main
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;

    using global::Recellection.Code.Controllers;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Logger;

    public class GameInitializer
    {
        #region Fields

        public Logger myLogger;

        #endregion

        // public Dictionary<Player,UnitAccountant> suitGuys { get; private set; }
        #region Constructors and Destructors

        public GameInitializer()
        {
            this.myLogger = LoggerFactory.GetLogger();
            this.myLogger.Info("Beginning Game Initialization.");
            this.CreateGameObjects(606);
        }

        #endregion

        #region Public Properties

        public World theWorld { get; private set; }

        #endregion

        #region Methods

        private bool CreateGameObjects(int seed)
        {
            this.myLogger.Info("Generating world.");
            this.theWorld = WorldGenerator.GenerateWorld(seed);
            this.myLogger.Info("Done.");

            // Let all units belong to the world!
            Unit.SetWorld(this.theWorld);
            new SoundsController(this.theWorld);

            var randomer = new Random(seed);

            this.myLogger.Info("Adding players.");
            var human = new Player(Color.Blue, "John");
            this.theWorld.AddPlayer(human);

            var temp2 = new List<Player>();
            temp2.Add(human);
            var ai = new AIPlayer(new AIView(this.theWorld), Color.Red);
            this.theWorld.AddPlayer(ai);

            human.Enemy = ai;
            ai.Enemy = human;

            this.myLogger.Info("Creating spawnpoints.");
            this.SpawnPoints(this.theWorld.players, this.theWorld.map.width, this.theWorld.map.height, randomer);

            this.myLogger.Info("Spawning units.");
            foreach (Player p in this.theWorld.players)
            {
                // We want 50 units!
                for (int i = 0; i < 10; i++)
                {
                    p.UnitAcc.ProduceUnits();
                }
            }

            int xOffset = (Recellection.viewPort.Width / Globals.TILE_SIZE) / 2;
            int yOffset = (Recellection.viewPort.Height / Globals.TILE_SIZE) / 2;

            this.theWorld.LookingAt =
                new Point(
                    (int)(this.theWorld.players[0].GetGraphs()[0].baseBuilding.position.X - xOffset), 
                    (int)(this.theWorld.players[0].GetGraphs()[0].baseBuilding.position.Y - yOffset));

            this.myLogger.Info(
                "Setting lookingAt to X: " + this.theWorld.LookingAt.X + "  y: " + this.theWorld.LookingAt.Y);
            return true;
        }

        /// <summary>
        /// Creates a new BaseBuilding at the specified location and
        /// add that fromBuilding to a new Graph and then add that graph
        /// to the player.
        /// </summary>
        /// <param name="xCoord">The x-coordinate to spawn the BaseBuilding on
        /// </param>
        /// <param name="yCoord">The Y-coordinate to spawn the BaseBuilding on
        /// </param>
        /// <param name="owner">The player which this method will create a new 
        /// graph.</param>
        private void SpawnGraph(int xCoord, int yCoord, Player owner)
        {
            /*BaseBuilding baseBuilding = new BaseBuilding("base", xCoord, yCoord, owner);

            theWorld.map.GetTile(xCoord, yCoord).SetBuilding(baseBuilding);*/
            this.myLogger.Info("Creating graph for player: " + owner + " at: " + xCoord + "," + yCoord);

            // owner.AddGraph(new Graph(baseBuilding));
            BuildingController.AddBuilding(
                Globals.BuildingTypes.Base, null, new Vector2(xCoord, yCoord), this.theWorld, owner);
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
        private void SpawnPoints(List<Player> players, 
                                 int width, int heigth, Random randomer)
        {
            int previousPlaceX = -1;
            int previousPlaceY = -1;

            int randomPlaceX;
            int randomPlaceY;

            foreach (Player player in players)
            {
                // Calculate the length of the vector between the new spawn
                // point and the last one.
                this.myLogger.Info("Create random spawn point?");
                do
                {
                    randomPlaceX = randomer.Next(0, 2);
                    randomPlaceY = randomer.Next(0, 2);
                }
                while (previousPlaceX == randomPlaceX && previousPlaceY == randomPlaceY);

                int xCoordinate = randomPlaceX == 0 ? 5 : width - 5;
                int yCoordinate = randomPlaceY == 0 ? 5 : heigth - 5;

                this.SpawnGraph(xCoordinate, yCoordinate, player);

                previousPlaceX = randomPlaceX;
                previousPlaceY = randomPlaceY;
            }

        }

        #endregion
    }

}
