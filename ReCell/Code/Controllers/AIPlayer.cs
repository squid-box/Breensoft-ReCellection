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
        private AIView m_view;
        private World m_world;
        private GraphController m_graph;
        private List<Player> m_opponents;
        private List<Vector2> interrestPoints;
        private List<Vector2> enemyPoints;

        private int distanceThreshold;



        /// <summary>
        /// Constructor. The AIPlayer requires quite alot of external controllers.
        /// </summary>
        /// <param name="opponents"></param>
        /// <param name="view"></param>
        /// <param name="world"></param>
        /// <param name="graph"></param>
        public AIPlayer(List<Player> opponents, AIView view, World world, GraphController graph){
            m_view = view;
            m_world = world;
            m_graph = graph;
            m_opponents = opponents;
            interrestPoints = new List<Vector2>();
            enemyPoints = new List<Vector2>();
            view.registerPlayer(this);
            distanceThreshold = 3; //Arbitrary number at the moment
        }

        /// <summary>
        /// The main method. When called it causes the AIPlayer to reevaluate 
        /// its situation and make appropriate changes.
        /// </summary>
        public void MakeMove(){

            for (int i = 0; i < interrestPoints.Count; i++)
            {
                Vector2 current = interrestPoints[i];

                if (m_view.ContainsResourcePoint(current))
                {
                    EvaluateResourcePoint(current);
                }
                else
                {
                    CalculateWeight(m_view.GetBuildingAt(current));
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
                    if (Vector2.Distance(current, nearby) > distanceThreshold)
                    {
                        nearby = CalculatePointNear(current);
                        interrestPoints[interrestPoints.Count] = nearby;
                    }
                    SendUnits(nearby); //TODO: How many to send?
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
        /// Decides what the weight should be at the given building
        /// </summary>
        /// <param name="building"></param>
        private void CalculateWeight(Building building)
        {
            int friendly = unitCountAt(building.coordinates, this);
            int enemy = unitCountAt(GetClosestPointFromList(building.coordinates, enemyPoints), m_opponents[0]);
            int diff = enemy - friendly;
            if (diff > 0) //more enemy units than friendly
            {
                int weight = m_graph.GetWeight(building);
                m_graph.SetWeight(building, weight + (diff/2)); //increase the weight by the difference in units / 2
            }
        }

        /// <summary>
        /// Method for sending out some scouts across the map in order to find opponent locations.
        /// </summary>
        private void Explore()
        {

            throw new NotImplementedException();
        }


        /// <summary>
        /// Figure out where to add a new point of interrest that is close enough to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private Vector2 CalculatePointNear(Vector2 point)
        {
            Vector2 closestFriendly = GetClosestPointFromList(point, m_view.GetFriendlyBuildings());

            //The distance between the two points, counting from the parameter point as origin.
            int diffX = (int)(point.X - closestFriendly.X);
            int diffY = (int)(point.Y - closestFriendly.Y);

            //How close to the enemy building we should build
            //These values may be calculated using more advanced logic.
            int offsetX = diffX / 2;
            int offsetY = diffY / 2;

            int newX = (int)(point.X + diffX - offsetX);
            int newY = (int)(point.Y + diffY - offsetY);

            return new Vector2(newX, newY);
        }

        /// <summary>
        /// Decides what to do with the given resource point
        /// </summary>
        /// <param name="point"></param>
        private void EvaluateResourcePoint(Vector2 point)
        {
            if (Harvesting(point))
            {
                if (CanHoldPoint(point))
                    return;

                CalculateWeight(m_view.GetBuildingAt(point));
            }
            if (CanHoldPoint(point))
            {
                IssueBuildOrder(point, m_view.getBaseBuilding() , Globals.BuildingTypes.Resource);
            }
            else
            {
                SendUnits(point);
            }
        }

        private void IncreaseWeight(Building building)
        {
            throw new NotImplementedException();
        }


        
        /// <summary>
        /// Returns true if the AIPlayer is already harvesting at the given coordinates.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool Harvesting(Vector2 point)
        {
            Building tempBuilding = m_view.GetBuildingAt(point);

            if (tempBuilding == null)
                return false;

            if (tempBuilding.owner != this)
            {
                //TODO: Enemy harvesting at this location, very interresting.
                return false;
            }

            if (m_view.GetBuildingAt(point).type == Globals.BuildingTypes.Resource)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evaluates whether or not the given point can be defended against a potential/ongoing attack
        /// from nearby enemy buildings.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool CanHoldPoint(Vector2 point)
        {
            if (unitCountAt(point, this) > (unitCountAt(point, m_opponents[0])+ unitCountAt(GetClosestPointFromList(point, enemyPoints), m_opponents[0])))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the number of units located at the given coordinates belonging to the given player.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private int unitCountAt(Vector2 point, Player player)
        {
            return m_view.getTileAt(point).GetUnits(player).ToArray().Length;
        }

        private void SendUnits(Vector2 point)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Called when a new building should be created
        /// </summary>
        /// <param name="point"></param>
        /// <param name="baseBuilding"></param>
        /// <param name="buildingType"></param>
        private void IssueBuildOrder(Vector2 point, Building baseBuilding, Globals.BuildingTypes buildingType)
        {
            BuildingController.AddBuilding(buildingType, baseBuilding, point, m_world);
        }

    }
}
