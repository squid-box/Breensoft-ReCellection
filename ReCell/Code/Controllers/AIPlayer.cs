using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Utility.Logger;

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
        private Logger log;



        /// <summary>
        /// Constructor. The AIPlayer requires quite alot of external controllers.
        /// </summary>
        /// <param name="opponents"></param>
        /// <param name="view"></param>
        public AIPlayer(List<Player> opponents, AIView view, Color c)
            : base(c, "AIPLAYER")
        {
            log = Utility.Logger.LoggerFactory.GetLogger();
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
        public void MakeMove()
        {
            log.Info("AI Making a Move.");

            if (m_enemyPoints.Count == 0 || m_interrestPoints.Count == 0)
            {
                Explore();
                //CHEAT!
                LookAtScreen();
            }
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

            for (int i = 0; i < m_enemyPoints.Count; i++)
            {
                Vector2 current = m_enemyPoints[i];
                Vector2 nearby = GetClosestPointFromList(current, m_view.GetFriendlyBuildings());

                if (Vector2.Distance(current, nearby) > distanceThreshold)
                {
                    nearby = CalculatePointNear(current); //Find a good spot to go to
                    m_interrestPoints[m_interrestPoints.Count-1] = nearby; //And add it to the todo-list
                }
                else //"nearby" is close enough.
                {
                    if (m_view.ContainsFriendlyBuilding(nearby))
                    {
                        CalculateWeight(m_view.GetBuildingAt(nearby));
                    }
                }
            }
        }


        /// <summary>
        /// The AI takes a look at all the tiles it can see (2010-05-16 everything) and
        /// looks for enemy buildings and resource points.
        /// </summary>
        private void LookAtScreen()
        {
            Tile[,] wMap = m_view.world.GetMap().map;
            Tile temp;
            for (int i = 0; i < m_view.mapWidth; i++)
            {
                for (int j = 0; j < m_view.mapHeight; j++)
                {
                    temp = wMap[i, j];
                    if(m_view.ContainsResourcePoint(temp)){
                        m_interrestPoints.Add(temp.GetPosition());
                    }
                    if (m_view.ContainsEnemyBuilding(temp))
                    {
                        m_enemyPoints.Add(temp.GetPosition());
                    }
                    if (m_view.ContainsFriendlyBuilding(temp.GetPosition()))
                    {
                        m_view.myBuildings.Add(temp.GetBuilding());
                    }
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
            for (int i = 1; i < list.Count; i++)
            {
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
            if (building == null)
            {
                throw new NullReferenceException();
            }
            int friendly = unitCountAt(building.position, this);
            int enemy = unitCountAt(GetClosestPointFromList(building.position, m_enemyPoints), m_opponents[0]);
            int diff = enemy - friendly;
            if (diff > 0) //more enemy units than friendly
            {
                int weight = GraphController.Instance.GetWeight(building);
                GraphController.Instance.SetWeight(building, weight + (diff / 2)); //increase the weight by the difference in units / 2
            }
        }

        /// <summary>
        /// Method for sending out some scouts across the map in order to find opponent locations.
        /// </summary>
        private void Explore()
        {
            log.Info("AI Exploring.");
            int scoutSize = 10;

            //Take the units from the base fromBuilding
            Building bb = GetGraphs()[0].baseBuilding;

            Tile source = m_view.getTileAt(bb.GetPosition());
            if (source == null)
            {
                return;
                
            }

            //Move the units to some location at the other end of the map
            Tile dest = m_view.getTileAt(randomPointAtOppositeQuadrant());
            if (dest == null)
            {
                return;
            }
            UnitController.MoveUnits(scoutSize, source, dest);
            log.Info("AI moved units to " + dest.GetPosition().X + "," + dest.GetPosition().Y);
        }


        /// <summary>
        /// Returns a coordinate randomly chosen from the opposite quadrant of the map
        /// relative to the AIs base.
        /// </summary>
        /// <returns></returns>
        private Vector2 randomPointAtOppositeQuadrant()
        {
            Vector2 baseCoords = GetGraphs()[0].baseBuilding.GetPosition();
            Vector2 mapSize = new Vector2(m_view.mapWidth, m_view.mapHeight);

            Vector2 middle = new Vector2(mapSize.X / 2, mapSize.Y / 2);

            Vector2 inner;
            Vector2 outer;

            //The map is divided into four parts as follows:
            //|q1 | q2|
            //|q3 | q4|

            //base in quadrant 1, opponent in q4
            if (baseCoords.X <= middle.X && baseCoords.Y <= middle.Y)
            {
                inner = middle;
                outer = mapSize;
            }
            //base in quadrant 2, opponent in q3
            else if (baseCoords.X > middle.X && baseCoords.Y <= middle.Y)
            {
                inner = new Vector2(0, mapSize.Y);
                outer = middle;
            }
            //base in quadrant 3, opponent in q2
            else if (baseCoords.X <= middle.X && baseCoords.Y > middle.Y)
            {
                inner = middle;
                outer = new Vector2(mapSize.X, 0);
            }
            //base in quadrant 4, opponent in q1
            else
            {
                inner = new Vector2(0, 0);
                outer = middle;
            }


            //Finally pick a set of coordinates within the opposite quadrant.

            float xVal = inner.X + (float)randomFactory.NextDouble() * (outer.X-inner.X);
            float yVal = inner.Y + (float)randomFactory.NextDouble() * (outer.Y-inner.Y);
            if (xVal == 0)
            {
                xVal += 1;
            }
            if (xVal >= mapSize.X)
            {
                xVal = mapSize.X-1;
            }
            if (yVal == 0)
            {
                yVal += 1;
            }
            if (yVal >= mapSize.Y)
            {
                yVal = mapSize.Y-1;
            }

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
            if (m_view.Harvesting(point) == this)
            {
                if (CanHoldPoint(point))
                    return;

                CalculateWeight(m_view.GetBuildingAt(point));
            }
            Vector2 closestFriendly = GetClosestPointFromList(point, m_view.GetFriendlyBuildings());
            if (CanHoldPoint(closestFriendly))
            {
                IssueBuildOrder(point, m_view.GetBuildingAt(closestFriendly), Globals.BuildingTypes.Resource);
            }
            else
            {
                CalculateWeight(m_view.GetBuildingAt(closestFriendly));
            }
        }




        /// <summary>
        /// Evaluates whether or not the given point can be defended against a potential/ongoing attack
        /// from nearby enemy buildings.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool CanHoldPoint(Vector2 point)
        {
            if (unitCountAt(point, this) > (unitCountAt(point, m_opponents[0]) + unitCountAt(GetClosestPointFromList(point, m_enemyPoints), m_opponents[0])))
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
        /// given point from the given sourceBuilding.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="baseBuilding"></param>
        /// <param name="buildingType"></param>
        private void IssueBuildOrder(Vector2 point, Building sourceBuilding, Globals.BuildingTypes buildingType)
        {
            BuildingController.AddBuilding(buildingType, sourceBuilding, point, m_view.world, this);
        }

    }
}
