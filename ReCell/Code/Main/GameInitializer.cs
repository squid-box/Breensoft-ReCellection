using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recellection.Code.Models;
using Recellection.Code.Controllers;
using Recellection.Code.Views;
using Recellection.Code.Utility.Logger;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Main
{
    public class GameInitializer
    {
        public World theWorld { get; private set; }
        public Logger myLogger;
        //public Dictionary<Player,UnitAccountant> suitGuys { get; private set; }

        public GameInitializer()
        {
            myLogger = LoggerFactory.GetLogger();
            myLogger.Info("Beginning Game Initialization.");
            CreateGameObjects(606);
        }

        private bool CreateGameObjects(int seed)
        {
            myLogger.Info("Generating world.");
            theWorld = WorldGenerator.GenerateWorld(seed);
            myLogger.Info("Done.");
            
            // Let all units belong to the world!
            Unit.SetWorld(theWorld);
            new SoundsController(theWorld);

            Random randomer = new Random(seed);

            myLogger.Info("Adding players.");
            Player human = new Player(Color.Blue, "John");
            theWorld.AddPlayer(human);

            List<Player> temp2 = new List<Player>();
            temp2.Add(human);
            AIPlayer ai = new AIPlayer(new AIView(theWorld),Color.Red);
            theWorld.AddPlayer(ai);
            
            human.Enemy = ai;
            ai.Enemy = human;

            myLogger.Info("Creating spawnpoints.");
            SpawnPoints(theWorld.players, theWorld.map.width, theWorld.map.height, randomer);

            myLogger.Info("Spawning units.");
            //suitGuys = new Dictionary<Player, UnitAccountant>(2);
            foreach(Player p in theWorld.players)
            {
                //suitGuys[p] = new UnitAccountant(p);
                p.unitAcc.ProduceUnits();
            }
            
            int xOffset = (Recellection.viewPort.Width/Globals.TILE_SIZE)/2;
            int yOffset = (Recellection.viewPort.Height/Globals.TILE_SIZE)/2;

            theWorld.LookingAt = new Point(
                (int)(theWorld.players[0].GetGraphs()[0].baseBuilding.position.X-xOffset),
                (int)(theWorld.players[0].GetGraphs()[0].baseBuilding.position.Y-yOffset));

            myLogger.Info("Setting lookingAt to X: " + theWorld.LookingAt.X + "  y: " + theWorld.LookingAt.Y);

            return true;
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
                //Calculate the length of the vector between the new spawn
                //point and the last one.

                myLogger.Info("Create random spawn point?");
                do
                {
                    randomPlaceX = randomer.Next(0, 2);
                    randomPlaceY = randomer.Next(0, 2);
                }
                while (previousPlaceX == randomPlaceX && previousPlaceY == randomPlaceY);

                int xCoordinate = randomPlaceX == 0 ? 5 : width - 5;
                int yCoordinate = randomPlaceY == 0 ? 5 : heigth - 5;

                SpawnGraph(xCoordinate, yCoordinate, player);

                previousPlaceX = randomPlaceX;
                previousPlaceY = randomPlaceY;
            }

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
            myLogger.Info("Creating graph for player: "+ owner +" at: "+xCoord+","+yCoord);
            //owner.AddGraph(new Graph(baseBuilding));
            BuildingController.AddBuilding(Globals.BuildingTypes.Base, null, new Vector2(xCoord, yCoord), theWorld,owner);
        }
    }

}
