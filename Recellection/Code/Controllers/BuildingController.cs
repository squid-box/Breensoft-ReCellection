using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Controllers
{
    class BuildingController
    {
        public static void AggressiveBuildingAct(Player player)
        {
            foreach (Graph g in player.GetGraphs())
            {
                foreach (Building b in g.GetBuildings())
                {

                    if (b.type == Globals.BuildingTypes.Aggressive)
                    {
                        AcquireTarget((AggressiveBuilding)b);
                    }
                }
            }

        }

        private static void AcquireTarget(AggressiveBuilding b)
        {
            foreach (Tile t in b.controlZone)
            {
                foreach (Unit u in t.GetUnits())
                {
                    if (u.GetOwner().Equals(b.owner))
                    {
                        b.SetTarget(u);
                        break;
                    }
                }
            }
        }

        public static void AddBuilding(Globals.BuildingTypes buildingType,
            Building buildFrom, Vector2 targetCoordinate)
        {
            if (buildingType == Globals.BuildingTypes.Base)
            {
                //GLOBAL ACCESS THE SINGLETON GRAPH_CONTROLL HAX
                //theGraphController.AddBaseBuilding(new BaseBuilding("Base Buidling",
                //targetCoordinate.X,targetCoordinate.Y,buildFrom.owner));
            }
            else
            {
                /*Building b;
                switch (buildingType)
                {
                    case Globals.BuildingTypes.Aggressive:
                        b = new AggressiveBuilding("Aggresive Building", targetCoordinate.X, targetCoordinate.Y, buildFrom.owner,
                            theGraphController.GetGraph(buildFrom).baseBuilding);
                        break;
                    case Globals.BuildingTypes.Barrier:
                        b = new BarrierBuilding("Barrier Building", targetCoordinate.X, targetCoordinate.Y, buildFrom.owner,
                            theGraphController.GetGraph(buildFrom).baseBuilding);
                        break;
                    case Globals.BuildingTypes.Resource:
                        b = new ResourceBuilding("Resource Building", targetCoordinate.X, targetCoordinate.Y, buildFrom.owner,
                            theGraphController.GetGraph(buildFrom).baseBuilding);
                        break;

                }*/
                //GLOBAL ACCESS THE SINGLETON GRAPH_CONTROLL HAX
                //theGraphController.AddBuilding(b);

            }
        }

        public static void RemoveBuilding(Building b)
        {
            //theGraphController.Remove(b);
        }

    }
}
