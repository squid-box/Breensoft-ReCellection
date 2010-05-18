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
        private int distanceThreshold;
        private Random randomFactory;
        private Logger log;



        /// <summary>
        /// Constructor. The AIPlayer requires quite alot of external controllers.
        /// </summary>
        /// <param name="opponents"></param>
        /// <param name="view"></param>
        [Obsolete("parameter opponents no longer needed. Overloaded constructor exists.")]
        public AIPlayer(List<Player> opponents, AIView view, Color c)
            : base(c, "AIPLAYER")
        {
            log = Utility.Logger.LoggerFactory.GetLogger();
            randomFactory = new Random();
            m_view = view;

            view.RegisterPlayer(this);
            distanceThreshold = 3; //Arbitrary number at the moment
        }

        /// <summary>
        /// Constructor. The AIPlayer requires only an AIView in addition to the parameters needed for a regular Player.
        /// </summary>
        /// <param name="view"></param>
        [Obsolete("parameter opponents no longer needed. Overloaded constructor exists.")]
        public AIPlayer(AIView view, Color c)
            : base(c, "AIPLAYER")
        {
            log = Utility.Logger.LoggerFactory.GetLogger();
            randomFactory = new Random();
            m_view = view;

            view.RegisterPlayer(this);
            distanceThreshold = 3; //Arbitrary number at the moment
        }

        /// <summary>
        /// The main method. When called it causes the AIPlayer to reevaluate 
        /// its situation and make appropriate changes.
        /// </summary>
        public void MakeMove()
        {
            log.Fatal("AI Making a Move.");

            m_view.LookAtScreen(); //Have the AI View update the local variables

            //Relevant if fog of war is implemented, currently replaced by LookAtScreen
            //if (m_view.enemyPoints.Count == 0 || m_view.interrestPoints.Count == 0)
            //{
                //Explore();
            //}

            
            IterateInterrestPoints();

            for (int i = 0; i < m_view.enemyPoints.Count; i++)
            {
                Vector2 current = m_view.enemyPoints[i];
                EvaluateEnemyPoint(current);
            }
        }

        /// <summary>
        /// Takes a look at the newest revision of the interresPoints and iterates over them,
        /// then making appropriate calls to game controllers.
        /// </summary>
        private void IterateInterrestPoints()
        {
            List<Vector2> interrestPoints = m_view.GetInterrestPoints();

            for (int i = 0; i < interrestPoints.Count; i++)
            {
                Vector2 current = interrestPoints[i];

                //HACK! Invalid coords should not be added to the list in the first place.
                //Suspect that LookAtScreen is incorrect.
                if (!m_view.Valid(current))
                {
                    log.Fatal("erroneous point " + current.X + "," + current.Y);
                    continue;
                }

                if (m_view.ContainsResourcePoint(current))
                {
                    EvaluateResourcePoint(current);
                }
                else
                {
                    CalculateWeight(m_view.GetBuildingAt(current));
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
        /// Decides what the weight should be at the given building
        /// </summary>
        /// <param name="fromBuilding"></param>
        private void CalculateWeight(Building building)
        {
            if (building == null)
            {
                throw new NullReferenceException();
            }
            int friendly = unitCountAt(building.position, this);
            int enemy = unitCountAt(GetClosestPointFromList(building.position, m_view.enemyPoints), m_view.opponents[0]);
            int diff = enemy - friendly;
            if (diff > 0) //more enemy units than friendly
            {
                int weight = GraphController.Instance.GetWeight(building);
                GraphController.Instance.SetWeight(building, weight + (diff / 2)); //increase the weight by the difference in units / 2
            }
            else
            {
                int weight = GraphController.Instance.GetWeight(building);
                GraphController.Instance.SetWeight(building, weight + diff); //decrease the weight by the difference in units
            }
        }

        /// <summary>
        /// Method for sending out some scouts across the map in order to find opponent locations.
        /// </summary>
        private void Explore()
        {
            log.Fatal("AI Exploring.");
            int scoutSize = 10;

            //Take the units from the base fromBuilding
            Building bb = GetGraphs()[0].baseBuilding;

            Tile source = m_view.GetTileAt(bb.GetPosition());
            if (source == null)
            {
                return;
                
            }

            //Move the units to some location at the other end of the map
            Tile dest = m_view.GetTileAt(randomPointAtOppositeQuadrant());
            if (dest == null)
            {
                return;
            }
            UnitController.MoveUnits(scoutSize, source, dest);
            log.Fatal("AI moved units to " + dest.GetPosition().X + "," + dest.GetPosition().Y);
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
        /// Decides what to do with the given enemy point
        /// </summary>
        /// <param name="current"></param>
        private void EvaluateEnemyPoint(Vector2 current)
        {
            Vector2 nearby = GetClosestPointFromList(current, m_view.GetFriendlyBuildings());

            if (Vector2.Distance(current, nearby) > distanceThreshold)
            {
                nearby = CalculatePointNear(current); //Find a good spot to go to
                m_view.interrestPoints[m_view.interrestPoints.Count - 1] = nearby; //And add it to the todo-list
            }
            else //"nearby" is close enough.
            {
                if (m_view.ContainsFriendlyBuilding(nearby))
                {
                    CalculateWeight(m_view.GetBuildingAt(nearby));
                }
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
            if (unitCountAt(point, this) > (unitCountAt(point, m_view.opponents[0]) + unitCountAt(GetClosestPointFromList(point, m_view.enemyPoints), m_view.opponents[0])))
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
            return m_view.GetTileAt(point).GetUnits(player).ToArray().Length;
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
