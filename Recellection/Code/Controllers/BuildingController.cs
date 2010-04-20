using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;

namespace Recellection.Code.Controllers
{
    class BuildingController
    {
        public BuildingController(Player player, World world)
        {
            foreach (Graph g in player.GetGraphs())
            {
                foreach (Building b in g.GetBuildings())
                {

                    if (b.type == Globals.BuildingTypes.Aggressive )
                    {
                        AcquireTarget(b);
                    }
                }
            }

        }

        private void AcquireTarget(Building b)
        {
            
        }

    }
}
