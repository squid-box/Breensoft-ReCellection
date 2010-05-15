using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Controllers
{
    class AIPlayer : Player
    {

        /**
         * Variables
         */
        private AIView m_view;
        private List<Player> m_opponents;
        private List<Vector2> m_interrestPoints;
        private List<Vector2> m_enemyPoints;
        private int distanceThreshold;
        private Random randomFactory;



        /// <summary>
        /// Constructor. The AIPlayer requires quite alot of external controllers.
        /// </summary>
        /// <param name="opponents"></param>
        /// <param name="view"></param>
        public AIPlayer(List<Player> opponents, AIView view, Color c):base(c,"AIPLAYER"){
            randomFactory = new Random();
            m_view = view;
            m_opponents = opponents;
            m_interrestPoints = new List<Vector2>();
            m_enemyPoints = new List<Vector2>();
            view.registerPlayer(this);
            distanceThreshold = 3; //Arbitrary number at the moment
        }

        /// <summary>
        /// The main method. When called it causes the AIPlayer to reevaluate 
        /// its situation and make appropriate changes.
        /// </summary>
        public void MakeMove(){

            for (int i = 0; i < m_interrestPoints.Count; i++)
            {
                Vector2 current = m_interrestPoints[i];

                if (m_view.ContainsResourcePoint(current))
                {
                    EvaluateResourcePoint(current);
                }
                else
                {
                    CalculateWeight(m_view.GetBuildingAt(current));
                }
            }
            if (m_enemyPoints.Count == 0)
            {
                Explore();
            }
            else
            {
                for (int i = 0; i < m_enemyPoints.Count; i++)
                {
                    Vector2 current = m_enemyPoints[i];
                    Vector2 nearby = GetClosestPointFromList(current, m_interrestPoints);
                    if (Vector2.Distance(current, nearby) > distanceThreshold)
                    {
                        nearby = CalculatePointNear(current);
                        m_interrestPoints[m_interrestPoints.Count] = nearby;
                    }
                    CalculateWeight(m_view.GetBuildingAt(nearby));
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
        /// Decides what the weight should be at the given fromBuilding
        /// </summary>
        /// <param name="fromBuilding"></param>
        private void CalculateWeight(Building building)
        {
            int friendly = unitCountAt(building.position, this);
            int enemy = unitCountAt(GetClosestPointFromList(building.position, m_enemyPoints), m_opponents[0]);
            int diff = enemy - friendly;
            if (diff > 0) //more enemy units than friendly
            {
                int weight = GraphController.Instance.GetWeight(building);
                GraphController.Instance.SetWeight(building, weight + (diff/2)); //increase the weight by the difference in units / 2
            }
        }

        /// <summary>
        /// Method for sending out some scouts across the map in order to find opponent locations.
        /// </summary>
        private void Explore()
        {
            int scoutSize = 10;

            //Take the units from the base fromBuilding
            Tile source = m_view.getTileAt(m_view.baseBuilding.GetPosition());

            //Move the units to some location at the other end of the map
            Tile dest = m_view.getTileAt(randomPointAtOppositeQuadrant());

            UnitController.MoveUnits(scoutSize, source, dest);
        }


        /// <summary>
        /// Returns a coordinate randomly chosen from the opposite quadrant of the map
        /// relative to the AIs base.
        /// </summary>
        /// <returns></returns>
        private Vector2 randomPointAtOppositeQuadrant()
        {
            Vector2 baseCoords = m_view.baseBuilding.GetPosition();

            //Get the opposite end of the map relative to the base fromBuilding.
            Vector2 quadrantCenter = Vector2.Subtract(new Vector2(m_view.mapWidth, m_view.mapHeight), baseCoords);

            //Create the "inner" border for the opposite quadrant. 
            Vector2 quadrantEdge = Vector2.Subtract(quadrantCenter, baseCoords);
            //Make sure that the coordinates are given positive values only
            quadrantEdge = new Vector2(Math.Abs(quadrantEdge.X), Math.Abs(quadrantEdge.Y));
            //Create the "outer" border for the opposite quadrant.
            Vector2 quadrantEdge2 = Vector2.Add(quadrantCenter, baseCoords);
            

            //Finally pick a set of coordinates within the opposite quadrant.

            float xVal = quadrantEdge.X + (float)randomFactory.NextDouble()*quadrantEdge2.X;
            float yVal = quadrantEdge.Y + (float)randomFactory.NextDouble()*quadrantEdge2.Y;
           
            Vector2 result = new Vector2(xVal, yVal);

            return result;
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

            //How close to the enemy fromBuilding we should build
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
                IssueBuildOrder(point, m_view.baseBuilding , Globals.BuildingTypes.Resource);
            }
            else
            {
                CalculateWeight(m_view.GetBuildingAt(point));
            }
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
            if (unitCountAt(point, this) > (unitCountAt(point, m_opponents[0])+ unitCountAt(GetClosestPointFromList(point, m_enemyPoints), m_opponents[0])))
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



        /// <summary>
        /// Called when a new fromBuilding should be created. Creates a fromBuilding of a given type at the 
        /// given point from the given base fromBuilding.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="baseBuilding"></param>
        /// <param name="buildingType"></param>
        private void IssueBuildOrder(Vector2 point, Building baseBuilding, Globals.BuildingTypes buildingType)
        {
            BuildingController.AddBuilding(buildingType, baseBuilding, point, m_view.world,this);
        }

    }
}
