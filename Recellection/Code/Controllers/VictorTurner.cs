using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
    class VictorTurner
    {
        private List<Player> players;

        //private World World;

        Boolean finished = false;
        /// <summary>
        /// The constructor used to initiate the Victor Turner
        /// </summary>
        /// <param name="players">The players in the game</param>
        /// <param name="world">The world the game takes place in</param>
        /*public VictorTurner(List<Player> players,World world)
        {
            this.players = players;
            this.world = world;
        }*/

        public void Run()
        {

            while (!finished)
            {
                foreach (Player player in players)
                {
                    if(HasLost(player))
                    {
                        //world.RemovePlayer(player);
                    }
                    if(HasWon(player))
                    {

                        finished = true;
                    }
                }

            }

        }

        private Boolean HasLost(Player player)
        {

            return false;
        }

        private Boolean HasWon(Player player)
        {


            return false;
        }

        private void PlayerAct(Player player)
        {

        }

        private void WorldAct()
        {

        }
    }
}
