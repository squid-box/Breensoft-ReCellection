using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recellection.Code.Models;
using Recellection.Code.Controllers;
using Recellection.Code.Views;

namespace Recellection.Code.Main
{
    public class GameInitializer
    {
        public World theWorld { get; private set; }
        public Dictionary<Player,UnitAccountant> suitGuys { get; private set; }

        public GameInitializer()
        {
            CreateGameObjects(4711);
        }

        private bool CreateGameObjects(int seed)
        {
            try
            {
                theWorld = WorldGenerator.GenerateWorld(seed);

                Random randomer = new Random(seed);

                Player temp = new Player(PlayerColour.BLUE, "John");
                theWorld.AddPlayer(temp);

                List<Player> temp2 = new List<Player>();
                temp2.Add(temp);
                theWorld.AddPlayer(new AIPlayer(temp2, new AIView(theWorld)));

                SpawnPoints(theWorld.players, theWorld.map.Cols, theWorld.map.Rows, randomer);

                foreach(Player p in theWorld.players)
                {
                    suitGuys[p] = new UnitAccountant(p);
                    suitGuys[p].ProduceUnits();
                }
                return true;
            }
            catch (Exception e)
            {
                Console.Beep(75, 1500);
                Console.Error.WriteLine(e.GetType() + " : " + e.Message);
                return false;
            }
            
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

                do
                {
                    randomPlaceX = randomer.Next(0, 1);
                    randomPlaceY = randomer.Next(0, 1);
                }
                while (previousPlaceX == randomPlaceX && previousPlaceY == randomPlaceY);

                int xCoordinate = randomPlaceX == 0 ? 10 : width - 10;
                int yCoordinate = randomPlaceY == 0 ? 10 : heigth - 10;

                SpawnGraph(xCoordinate, yCoordinate, player);

                previousPlaceX = randomPlaceX;
                previousPlaceY = randomPlaceY;
            }

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
        private void SpawnGraph(int xCoord, int yCoord, Player owner)
        {
            owner.AddGraph(new Graph(
                new BaseBuilding("base", xCoord, yCoord, owner)));
        }
    }

}
