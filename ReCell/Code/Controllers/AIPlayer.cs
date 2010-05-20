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

        private int resourceThreshold = 2; //The ideal number of resource hotspots the AI should have secured
        private int resourceCriticalThreshold = 1; //The least number of resource hotspots the AI MUST have secured

        private List<Building> criticalBuildings = new List<Building>();

        private Random randomFactory;
        private Logger log;


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
            log.Info("AI Making a Move.");

            m_view.LookAtScreen(); //Have the AI View update its local variables
            int resourceLocations = m_view.GetResourceLocations().Count;

            //First order of business: secure some income
            //While we have secured less resource points than we should have, get some!
            if (resourceLocations < resourceThreshold)
            {
                log.Info("Not enough resource points. Need " + resourceThreshold + ", have " + resourceLocations);
                SecureNextResourceHotSpot();
            }

            if (resourceLocations < resourceCriticalThreshold)
            {
                log.Info("Not enough resource points so nothing more to do this turn.");
                log.Info("//Ending turn");
                //If we have not secured basic income, dont worry about anything else.
                return;
            }

            //Reset all the building weights.
            SetWeights(m_view.myBuildings, 0);


            //If we are falling behind on the upgrades: catch up.
            if (m_view.opponents[0].PowerLevel > this.PowerLevel)
            {
                log.Info("Attempting to upgrading units.");
                UpgradeUnits();
            }

            List<Building> enemyFront = new List<Building>();
            List<Building> front = GetFrontLine(enemyFront);
            log.Info("Weighing the frontline");
            WeighFrontLine(front, enemyFront);
            SetCriticalWeights();
            log.Info("//Ending turn");
        }


        /// <summary>
        /// Sets the weight of the given builings to the given number.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="p"></param>
        private void SetWeights(List<Building> list, int p)
        {
            foreach (Building b in list) 
            {
                GraphController.Instance.SetWeight(b, p);
            }
        }

        /// <summary>
        /// Upgrade the level of the units by finding a building that can afford
        /// to pay for the upgrade and then ordering the upgrade.
        /// </summary>
        private void UpgradeUnits()
        {
            int cost = unitAcc.GetUpgradeCost();
            
            List<Building> buildings = m_view.myBuildings;
            Building b = Util.FindBuildingWithUnitCount(cost, buildings);
            if (b != null)
            {
                log.Info("Found a suitable building at " + b.GetPosition().X + ";" + b.GetPosition().Y + ", upgrading units.");
                unitAcc.PayAndUpgradePower(b);
                return;
            }
            log.Info("Could not afford to upgrade.");
        }



        /// <summary>
        /// Sets the most critical building's weights. Makes sure that they are not lost.
        /// </summary>
        private void SetCriticalWeights()
        {
            foreach (Building b in criticalBuildings) 
            {
                GraphController.Instance.SetWeight(b, m_view.CRITICAL);
            }
        }

        /// <summary>
        /// Returns a list of all buildings that define the front line, as well
        /// as filling the given list with their corresponding closest enemy.
        /// </summary>
        /// <returns></returns>
        private List<Building> GetFrontLine(List<Building> enemyFront)
        {
            log.Info("Finding the front line.");
            List<Building> buildings = m_view.myBuildings;
            List<Building> result = new List<Building>();
            log.Info("Iterating over " + buildings.Count + " buildings");
            for(int i = 0; i < buildings.Count; i++)
            {
                Building temp = buildings[i];
                Building enemy = m_view.GetBuildingAt(Util.GetClosestPointFromList(temp.GetPosition(), m_view.enemyPoints));
                if (m_view.GetBuildingAt(Util.GetClosestPointFromList(enemy.GetPosition(), m_view.friendlyPoints)) == temp)
                { //There is no friendly point that's more threatened by our closest enemy point
                    log.Info("Adding " + temp.GetPosition().X + ";" +  temp.GetPosition().Y + " to the front list.");
                    result.Add(temp);
                    enemyFront.Add(enemy);
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

            //First, check how many units there are in the enemy front.
            int enemySum = GetUnitCountFrom(enemyFront);
            int mySum = GetUnitCountFrom(front);

            float[] ratios = new float[front.Count];
            //Then see what their internal ratios are.

            if (enemySum == 0)
            { //All enemy buildings have a guaranteed equal distribution
                for (int i = 0; i < ratios.Length; i++)
                    ratios[i] = 1 / enemyFront.Count;
            }
            else
            { //Distribution might vary
                for (int i = 0; i < enemyFront.Count; i++)
                {
                    int currentCount = enemyFront[i].GetUnits().Count;
                    if (currentCount != 0)//The building actually has a ratio
                        ratios[i] = ((float)(currentCount) / (float)enemySum);

                    log.Info("Ratio at " + i + " is " + ratios[i]);
                }
            }
            log.Info("Front line consists of: ");
            //Finally distribute that same ratio across the AI's own border.
            for (int i = 0; i < front.Count; i++)
            {
                int weight = (int)(ratios[i] * m_view.CRITICAL);
                Building temp = front[i];
                log.Info("("+ temp.GetPosition().X + ";" + temp.GetPosition().Y + ") weight set to " + weight);
                GraphController.Instance.SetWeight(temp, weight);
            }
        }

        /// <summary>
        /// Returns the total number of units in the given list of buildings
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private int GetUnitCountFrom(List<Building> b)
        {
            int uSum = 0;
            for (int i = 0; i < b.Count; i++)
            {
                uSum += b[i].GetUnits().Count;
            }
            return uSum;
        }


        /// <summary>
        /// Identifies the closest resource hotspot and expands towards it.
        /// </summary>
        private void SecureNextResourceHotSpot()
        {
            log.Info("Securing next hotspot.");
            Building sourceBuilding = SelectSourceBuilding();
            Vector2 sourcePosition = sourceBuilding.GetPosition();
            Vector2 resource = GetClosestResourceHotspot(sourcePosition);
            log.Info("Next spot found at " + resource.X + ";" + resource.Y);
            List<Vector2> path = GenerateBuildPathBetween(sourcePosition, resource);

            //Skip the parts of the path we have walked before
            for (int i = 0; i < path.Count; i++)
            {
                Building b = m_view.GetBuildingAt(path[i]);
                if (b != null && b.GetOwner() == this)
                {//Already have a building here, take a shortcut
                    criticalBuildings.Remove(b);
                    sourceBuilding = b;
                    sourcePosition = sourceBuilding.GetPosition();
                }
            }
            bool built = false;

            if (WithinBuildRangeOf(sourceBuilding.GetPosition(), resource))
            {//The resource building is within reach of the source
                log.Info("Spot is close enough, expanding from " + sourceBuilding.GetPosition().X + ";" + sourceBuilding.GetPosition().Y);
                built = IssueBuildOrder(resource, sourceBuilding, Globals.BuildingTypes.Resource);
                if (built)
                {
                    log.Info("Building created.");
                    ClearHotspotWeights(path);
                }
                else 
                {
                    log.Info("Could not create the building.");
                }
            }
            else
            {
                Vector2 coords = path[0];
                log.Info("Spot is too far away, relaying building at " + coords.X + ";" + coords.Y);
                //Relay buildings must be placed first
                built = IssueBuildOrder(coords, sourceBuilding, Globals.BuildingTypes.Barrier);
                if (built)
                {
                    log.Info("Building created at " + coords.X + ";" + coords.Y);
                    if (m_view.GetResourceLocations().Count < resourceThreshold)
                    {//If the AI has not secured enough resource spots, set the priority to critical
                        log.Info("Importance of this building is CRITICAL");
                        criticalBuildings.Add(m_view.GetBuildingAt(coords));
                    }
                    else
                    {
                        log.Info("Importance of this building is THREATENED");
                        GraphController.Instance.SetWeight(m_view.GetBuildingAt(coords), m_view.THREATENED);
                    }
                }
                else
                { //Could not create that building
                    log.Info("Could not create the building");
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
            log.Info("Clearing all " + (path.Count-1) + " latent path weights.");
            for (int i = 0; i < path.Count-1; i++)
            {
                Vector2 current = path[i];
                Building b = m_view.GetBuildingAt(current);
                if (b != null)
                {
                    log.Info("(" + current.X + ";" + current.Y + ") set to SAFE.");
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
            Building b = GetGraphs()[0].baseBuilding;
            if (b == null)
            { //Base building destroyed
                b = GetGraphs()[0].GetBuildings().First();
            }
            return b;
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
                source = Util.GetClosestPointFromList(dest, CreateMatrixFromInterval(BuildingController.GetValidBuildingInterval(source, m_view.world)));
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
                
                closestFriendly = Util.GetClosestPointFromList(r, m_view.friendlyPoints);
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
        /// Called when a new fromBuilding should be created. Creates a fromBuilding of a given type at the 
        /// given point from the given sourceBuilding. Returns false if it failed.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="baseBuilding"></param>
        /// <param name="buildingType"></param>
        /// <returns></returns>
        private bool IssueBuildOrder(Vector2 point, Building sourceBuilding, Globals.BuildingTypes buildingType)
        {
            bool created = BuildingController.AddBuilding(buildingType, sourceBuilding, point, m_view.world, this);
            if (created)
                m_view.BuildingAddedAt(point);

            return created;
        }

    }
}