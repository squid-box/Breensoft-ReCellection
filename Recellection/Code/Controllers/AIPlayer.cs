using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Controllers
{
    class AIPlayer
    {

        /**
         * Variables
         */
        private AIView view;
        private Vector2[] interrestPoints;
        private Vector2[] enemyPoints;
        private int threshold;




        /**
         * Methods
         */
        public void MakeMove(){

            for (int i = 0; i < interrestPoints.Length; i++)
            {
                Vector2 current = interrestPoints[i];

                if (view.ContainsResourcePoint(current))
                {
                    EvaluateResourcePoint(current);
                }
                else
                {
                    CalculateWeight(current);
                }
            }

            if (enemyPoints.Length == 0)
            {
                Explore(CalculateScoutDirection());
            }
            else
            {
                for (int i = 0; i < enemyPoints.Length; i++)
                {
                    Vector2 current = enemyPoints[i];
                    Vector2 nearby = GetClosestInterrestPoint(current);
                    if (DistanceBetween(current, nearby) > threshold)
                    {
                        nearby = CalculatePointNear(current);
                        interrestPoints[interrestPoints.Length] = nearby;
                    }
                    SendUnits(nearby);
                }
            }
        }


        private void CalculateWeight(Vector2 Current)
        {
            throw new NotImplementedException();
        }

        private void Explore(object p)
        {
            throw new NotImplementedException();
        }

        private object CalculateScoutDirection()
        {
            throw new NotImplementedException();
        }

        private Vector2 GetClosestInterrestPoint(Vector2 Current)
        {
            throw new NotImplementedException();
        }

        private int DistanceBetween(Vector2 Current, Vector2 Nearby)
        {
            throw new NotImplementedException();
        }

        private Vector2 CalculatePointNear(Vector2 Current)
        {
            throw new NotImplementedException();
        }

        private void EvaluateResourcePoint(Vector2 point)
        {
            //Resource point but we already have it.
            if (Harvesting(point))
                return;

            if (CanHoldPoint(point))
            {
                IssueBuildOrder(point, Globals.BuildingTypes.Resource);
            }
            else
            {
                SendUnits(point);
            }
        }

        
        /*
         * Returns true if the AIPlayer is already harvesting at the given coordinates.
         */
        private bool Harvesting(Vector2 point)
        {
            Building tempBuilding = view.GetBuildingAt(point);

            if (tempBuilding == null)
                return false;

            if (tempBuilding.GetPlayer != this)
            {
                //TODO: Enemy harvesting at this location, very interresting.
                return false;
            }

            if (view.GetBuildingTypeOf(view.GetBuildingAt(point)) == Globals.BuildingTypes)
            {
                return true;
            }
        }

        private bool CanHoldPoint(Vector2 Point)
        {
            throw new NotImplementedException();
        }

        private void SendUnits(Vector2 Point)
        {
            throw new NotImplementedException();
        }

        private void IssueBuildOrder(Vector2 point, Globals.BuildingTypes buildingType)
        {
            throw new NotImplementedException();
        }

    }
}
