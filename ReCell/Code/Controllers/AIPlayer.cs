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
        private int resourceThreshold = 3; //The least number of resource hotspots the AI should have secured

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
            InitiateUtils();
            m_view = view;

            view.RegisterPlayer(this);
            distanceThreshold = BuildingController.MAX_BUILDING_RANGE;
        }

        /// <summary>
        /// Initiate utilities
        /// </summary>
        private void InitiateUtils()
        {
            log = Utility.Logger.LoggerFactory.GetLogger();
            randomFactory = new Random();
        }

        /// <summary>
        /// Constructor. The AIPlayer requires only an AIView in addition to the parameters needed for a regular Player.
        /// </summary>
        /// <param name="view"></param>
        public AIPlayer(AIView view, Color c)
            : base(c, "AIPLAYER")
        {
            InitiateUtils();

            m_view = view;
            view.RegisterPlayer(this);
            distanceThreshold = BuildingController.MAX_BUILDING_RANGE;
        }

        /// <summary>
        /// The main method. When called it causes the AIPlayer to reevaluate 
        /// its situation and make appropriate changes.
        /// </summary>
        public void MakeMove()
        {
            log.Fatal("AI Making a Move.");

            m_view.LookAtScreen(); //Have the AI View update its local variables

            //First order of business: secure some income
            //While we have secured less resource points than we should have, get some!
            Building sourceBuilding = m_view.myBuildings[0];
            Vector2 sourcePosition = sourceBuilding.GetPosition();
            while (m_view.GetResourceLocations().Count < resourceThreshold && sourceBuilding.units.Count > this.unitAcc.CalculateBuildingCostInflation(Globals.BuildingTypes.Resource))
            {
                Vector2 resource = getClosestResourceHotspot(sourcePosition);

                if (!WithinBuildRangeOf(resource, sourcePosition))
                { //Invalid coordinates, the resource spot is too far away.
                    log.Fatal("Resource spot " + resource.X+","+resource.Y + " too far away.");
                    //Generate a point closer to the resource point than the source building

                    Vector2 halfway = Vector2.Add(Vector2.Divide(Vector2.Subtract(resource, sourcePosition), 2), resource);
                    if (WithinBuildRangeOf(sourcePosition, halfway))
                    {//We can relay with one building
                        IssueBuildOrder(halfway, sourceBuilding, Globals.BuildingTypes.Barrier);
                    }
                    else
                    {//The closest resource hotspot is further away than one relay building.
                        IssueBuildOrder(halfway, sourceBuilding, Globals.BuildingTypes.Base);
                    }
                }
                else
                {
                    IssueBuildOrder(resource, sourceBuilding, Globals.BuildingTypes.Resource);
                }
            }


            //
            //IterateInterrestPoints();

            //for (int i = 0; i < m_view.enemyPoints.Count; i++)
            //{
            //    Vector2 current = m_view.enemyPoints[i];
            //    EvaluateEnemyPoint(current);
            //}
        }

        /// <summary>
        /// Syntactic sugar. Checks if the given source building is within building range of dest.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        private bool WithinBuildRangeOf(Vector2 source, Vector2 dest)
        {
            List<Point> valid = BuildingController.GetValidBuildingInterval(dest, m_view.world);

            Point v1 = valid[0];
            Point v2 = valid[1];

            if ((int)source.X < v1.X || (int)source.X > v2.X || (int)source.Y < v1.Y || (int)source.Y > v2.Y)
            {
                return false;
            }
            return true;
        }
        

        /// <summary>
        /// Method for figuring out the best resource hotspot to take.
        /// The parameter is the closest friendly building.
        /// </summary>
        /// <returns></returns>
        private Vector2 getClosestResourceHotspot(Vector2 friendly)
        {
            //Get a list of all resource hotspots
            List<Vector2> resources = m_view.resourcePoints;
            Vector2 currentBest = resources[0];
            Vector2 closestFriendly = currentBest;

            int currentDist = int.MaxValue;
            int bestDist = int.MaxValue;
            //For every resource hotspot check the distance to the closest friendly.
            for (int i = 0; i < resources.Count; i++)
            {
                //newBest is the friendly building closest to resources[i]. Might be useful
                closestFriendly = GetClosestPointFromList(resources[i], m_view.friendlyPoints, currentDist);
                if(currentDist < bestDist)
                {//We have a new leader

                    bestDist = currentDist; //Closest distance
                    currentBest = resources[i]; //Closest resource point
                    friendly = closestFriendly;//Closest friendly building location
                }
            }
            return currentBest;
        }

        /// <summary>
        /// Takes a look at the newest revision of the interresPoints and iterates over them,
        /// then making appropriate calls to game controllers.
        /// </summary>
        private void IterateInterrestPoints()
        {
            List<Vector2> interrestPoints = m_view.interrestPoints;

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
        /// Overloaded. +1 to get the distance
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
                if (Vector2.Distance(temp, point) < Vector2.Distance(best, point))
                {
                    best = temp;
                }
            }
            return best;
        }


        /// <summary>
        /// Returns the point in the given list that is closest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private Vector2 GetClosestPointFromList(Vector2 point, List<Vector2> list, int dist)
        {
            Vector2 best = list[0];
            Vector2 temp = best;
            for (int i = 1; i < list.Count; i++)
            {
                temp = list[i];
                float dist1 = Vector2.Distance(temp, point);
                float dist2 = Vector2.Distance(best, point);
                if (dist1 < dist2)
                {
                    best = temp;
                    dist = (int)dist1;
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
                m_view.AddInterrestPoint(nearby); //And add it to the todo-list
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
        /// Creates a resource building at the given location from the closest  friendly building
        /// available. Returns if it failed.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        private bool BuildResourceBuilding(Vector2 location)
        {
            Vector2 closestFriendly = GetClosestPointFromList(location, m_view.friendlyPoints);
            return IssueBuildOrder(location, m_view.GetTileAt(closestFriendly).GetBuilding(), Globals.BuildingTypes.Resource);
        }

        /// <summary>
        /// Called when a new fromBuilding should be created. Creates a fromBuilding of a given type at the 
        /// given point from the given sourceBuilding. Returns false if it failed.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="baseBuilding"></param>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        private bool IssueBuildOrder(Vector2 point, Building sourceBuilding, Globals.BuildingTypes buildingType)
        {
            return BuildingController.AddBuilding(buildingType, sourceBuilding, point, m_view.world, this);
        }

    }
}
