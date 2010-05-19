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

        private int resourceThreshold = 3; //The ideal number of resource hotspots the AI should have secured
        private int resourceCriticalThreshold = 1; //The least number of resource hotspots the AI MUST have secured
        
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
        }

        /// <summary>
        /// The main method. When called it causes the AIPlayer to reevaluate 
        /// its situation and make appropriate changes.
        /// </summary>
        public void MakeMove()
        {
            log.Fatal("AI Making a Move.");

            m_view.LookAtScreen(); //Have the AI View update its local variables
            int resourceLocations = m_view.GetResourceLocations().Count;

            //First order of business: secure some income
            //While we have secured less resource points than we should have, get some!
            if(resourceLocations < resourceThreshold)
                SecureNextResourceHotSpot();

            if (resourceLocations < resourceCriticalThreshold)
                //If we have not secured basic income, dont worry about anything else.
                return;

            List<Building> enemyFront = new List<Building>();
            List<Building> front = GetFrontLine(enemyFront);
            WeighFrontLine(front, enemyFront);
        }

        /// <summary>
        /// Returns a list of all buildings that define the front line, as well
        /// as filling the given list with their corresponding closest enemy.
        /// </summary>
        /// <returns></returns>
        private List<Building> GetFrontLine(List<Building> enemyFront)
        {
            List<Building> buildings = m_view.myBuildings;
            List<Building> result = new List<Building>();
            for(int i = 0; i < buildings.Count; i++)
            {
                Building temp = buildings[i];
                Building enemy = m_view.GetBuildingAt(GetClosestPointFromList(temp.GetPosition(), m_view.enemyPoints));
                if (m_view.GetBuildingAt(GetClosestPointFromList(enemy.GetPosition(), m_view.friendlyPoints)) == temp)
                { //There is no friendly point that's more threatened by our closest enemy point
                    if (!enemyFront.Contains(enemy))
                    {
                        result.Add(temp);
                        enemyFront.Add(enemy);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Iterates over the giving front, setting their weights according to their respective enemies.
        /// </summary>
        /// <param name="front"></param>
        /// <param name="enemyFront"></param>
        private void WeighFrontLine(List<Building> front, List<Building> enemyFront)
        {
            double enemySum = 0;
            double[] ratios = new double[front.Count];

            //First, check how many units are in the enemy front.
            for (int i = 0; i < enemyFront.Count; i++)
            {
                enemySum += enemyFront[i].GetUnits().Count;
            }
            //Then see what their internal ratios are.
            for (int i = 0; i < enemyFront.Count; i++)
            {
                if (enemySum != 0)
                {
                    ratios[i] = (double)(enemyFront[i].GetUnits().Count) / enemySum;
                }
            }
            //Finally distribute that same ratio across the AI's own border.
            for (int i = 0; i < front.Count; i++)
            {
                GraphController.Instance.SetWeight(front[i], (int)(ratios[i] * m_view.CRITICAL));
            }
        }


        /// <summary>
        /// Identifies the closest resource hotspot and expands towards it.
        /// </summary>
        private void SecureNextResourceHotSpot()
        {
            Building sourceBuilding = SelectSourceBuilding();
            Vector2 sourcePosition = sourceBuilding.GetPosition();
            Vector2 resource = GetClosestResourceHotspot(sourcePosition);
            List<Vector2> path = GenerateBuildPathBetween(sourcePosition, resource);

            //Skip the parts of the path we have walked before
            for (int i = 0; i < path.Count; i++)
            {
                Building b = m_view.GetBuildingAt(path[i]);
                if (b != null && b.GetOwner() == this)
                {//Already have a building here, take a shortcut
                    sourceBuilding = b;
                    sourcePosition = sourceBuilding.GetPosition();
                }
            }
            bool built = false;

            if (WithinBuildRangeOf(sourceBuilding.GetPosition(), resource))
            {//The resource building is within reach of the source
                built = IssueBuildOrder(resource, sourceBuilding, Globals.BuildingTypes.Resource);
                if(built)
                    ClearHotspotWeights(path);
            }
            else
            {
                //Relay buildings must be placed first
                built = IssueBuildOrder(path[0], sourceBuilding, Globals.BuildingTypes.Barrier);
                if (built)
                {
                    if (m_view.GetResourceLocations().Count < resourceThreshold)
                    {//If the AI has not secured enough resource spots, set the priority to critical
                        GraphController.Instance.SetWeight(m_view.GetBuildingAt(path[0]), m_view.CRITICAL);
                    }
                    else
                    {
                        GraphController.Instance.SetWeight(m_view.GetBuildingAt(path[0]), m_view.THREATENED);
                    }
                }
            }
        }

        /// <summary>
        /// Iterates over a given list of coordinates representing building coordinates
        /// and sets the weights of those buildings to "SAFE".
        /// </summary>
        /// <param name="path"></param>
        private void ClearHotspotWeights(List<Vector2> path)
        {
            for (int i = 0; i < path.Count-1; i++)
            {
                Building b = m_view.GetBuildingAt(path[i]);
                if (b != null)
                {
                    GraphController.Instance.SetWeight(b, m_view.SAFE);
                }
            }
        }


        /// <summary>
        /// Method for deciding where to build from. 
        /// right now it's basically a placeholder for more advanced code later.
        /// </summary>
        /// <returns></returns>
        private Building SelectSourceBuilding()
        {
            return this.GetGraphs()[0].baseBuilding;
        }


        /// <summary>
        /// Generates a list of optimal building placements for connecting the two given points.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="sourcePosition"></param>
        /// <returns></returns>
        private List<Vector2> GenerateBuildPathBetween(Vector2 p_source, Vector2 p_dest)
        {
            Vector2 source = p_source;
            Vector2 dest = p_dest;

            List<Vector2> path  = new List<Vector2>();
            do
            {
                source = GetClosestPointFromList(dest, CreateMatrixFromInterval(BuildingController.GetValidBuildingInterval(source, m_view.world)));
                path.Add(source);
            } while (!WithinBuildRangeOf(source, dest));
            if(source != dest) //safeguard for double add
                path.Add(p_dest);

            return path;
        }


        /// <summary>
        /// Converts a given tile interval to a matrix of tile coordinates.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<Vector2> CreateMatrixFromInterval(List<Point> list)
        {
            List<Vector2> result = new List<Vector2>();

            for (int i = list[0].X; i <= list[1].X; i++ )
                for (int j = list[0].Y; j <= list[1].Y; j++)
                {
                    result.Add(new Vector2(i,j));
                }
            return result;
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
        private Vector2 GetClosestResourceHotspot(Vector2 friendly)
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
                Vector2 r = resources[i];
                Building b = m_view.GetBuildingAt(r);
                if (b != null)
                    //There is already a building here
                    continue;

                closestFriendly = GetClosestPointFromList(r, m_view.friendlyPoints);
                currentDist = (int)Vector2.Distance(r, closestFriendly);
                if(currentDist < bestDist)
                {//We have a new leader

                    bestDist = currentDist; //Closest distance
                    currentBest = r; //Closest resource point
                    friendly = closestFriendly;//Closest friendly building location
                }
            }
            return currentBest;
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
                if (Vector2.Distance(temp, point) < Vector2.Distance(best, point))
                {
                    best = temp;
                }
            }
            return best;
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