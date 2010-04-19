using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Controllers
{
    class AIPlayer : Player
    {

        /**
         * Variables
         */
        private AIView view;
        private List<Vector2> interrestPoints;
        private List<Vector2> enemyPoints;
        private int threshold;



        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="p_view"></param>
        public AIPlayer(AIView p_view){
            view = p_view;
            interrestPoints = new List<Vector2>();
            enemyPoints = new List<Vector2>();

        }

        /// <summary>
        /// The main method. When called it causes the AIPlayer to reevaluate 
        /// its situation and make appropriate changes.
        /// </summary>
        public void MakeMove(){

            for (int i = 0; i < interrestPoints.Count; i++)
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

            if (enemyPoints.Count == 0)
            {
                Explore();
            }
            else
            {
                for (int i = 0; i < enemyPoints.Count; i++)
                {
                    Vector2 current = enemyPoints[i];
                    Vector2 nearby = GetClosestPointFromList(current, interrestPoints);
                    if (Vector2.Distance(current, nearby) > threshold)
                    {
                        nearby = CalculatePointNear(current);
                        interrestPoints[interrestPoints.Count] = nearby;
                    }
                    SendUnits(nearby);
                }
            }
        }

        /// <summary>
        /// Returns the point in the given list that is closest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private Vector2 GetClosestPointFromList(Vector2 point, List<Vector2> list)
        {
            Vector2 best = list[0];
            Vector2 temp = best;
            for(int i = 1; i < list.Count; i++){
                temp = list[i];
                if (Vector2.Distance(temp, point) < Vector2.Distance(temp, best))
                {
                    best = temp;
                }
            }
            return best;
        }


        /// <summary>
        /// Decides what the weight should be at the given coordinates.
        /// </summary>
        /// <param name="Current"></param>
        private void CalculateWeight(Vector2 Current)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Method for sending out some scouts across the map in order to find opponent locations.
        /// </summary>
        private void Explore()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decides what direction/location to scout.
        /// </summary>
        /// <returns></returns>
        private object CalculateScoutDirection()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Figure out where to add a new point of interrest that is close enough to the given point.
        /// </summary>
        /// <param name="Current"></param>
        /// <returns></returns>
        private Vector2 CalculatePointNear(Vector2 current)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decides what to do with the given resource point
        /// </summary>
        /// <param name="point"></param>
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

        
        /// <summary>
        /// Returns true if the AIPlayer is already harvesting at the given coordinates.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool Harvesting(Vector2 point)
        {
            Building tempBuilding = view.GetBuildingAt(point);

            if (tempBuilding == null)
                return false;

            if (tempBuilding.GetPlayer() != this)
            {
                //TODO: Enemy harvesting at this location, very interresting.
                return false;
            }

            if (view.GetBuildingTypeOf(view.GetBuildingAt(point)) == Globals.BuildingTypes.Resource)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evaluates whether or not the given point can be defended against a potential attack
        /// from nearby enemy buildings.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool CanHoldPoint(Vector2 point)
        {
            if (unitCountAt(point) > unitCountAt(GetClosestPointFromList(point, enemyPoints)))
            {
                return true;
            }
            return false;
        }

        private int unitCountAt(Vector2 point)
        {
            return view.getTileAt(point).GetUnits().ToArray().Length;
        }

        private void SendUnits(Vector2 point)
        {
            throw new NotImplementedException();
        }

        private void IssueBuildOrder(Vector2 point, Globals.BuildingTypes buildingType)
        {
            throw new NotImplementedException();
        }

    }
}
