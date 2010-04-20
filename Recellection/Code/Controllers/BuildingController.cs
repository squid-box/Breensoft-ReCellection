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
            Building sourceBuilding, Vector2 targetCoordinate)
        {
            if (buildingType == Globals.BuildingTypes.Base)
            {
                GraphController.Instance.AddBaseBuilding(new BaseBuilding("Base Buidling",
                (int)targetCoordinate.X, (int)targetCoordinate.Y, sourceBuilding.owner));
            }
            else
            {
                Building newBuilding = null;
                switch (buildingType)
                {
                    case Globals.BuildingTypes.Aggressive:
                        newBuilding = new AggressiveBuilding("Aggresive Building",
                            (int)targetCoordinate.X, (int)targetCoordinate.Y, sourceBuilding.owner,
                            GraphController.Instance.GetGraph(sourceBuilding).baseBuilding);
                        break;
                    case Globals.BuildingTypes.Barrier:
                        newBuilding = new BarrierBuilding("Barrier Building",
                            (int)targetCoordinate.X, (int)targetCoordinate.Y, sourceBuilding.owner,
                            GraphController.Instance.GetGraph(sourceBuilding).baseBuilding);
                        break;
                    case Globals.BuildingTypes.Resource:
                        newBuilding = new ResourceBuilding("Resource Building",
                            (int)targetCoordinate.X, (int)targetCoordinate.Y, sourceBuilding.owner,
                            GraphController.Instance.GetGraph(sourceBuilding).baseBuilding);
                        break;

                }
                GraphController.Instance.AddBuilding(sourceBuilding,newBuilding);

            }
        }

        public static void RemoveBuilding(Building b)
        {
            GraphController.Instance.RemoveBuilding(b);
        }

    }
}
