namespace Recellection.Code.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Logger;

    class AIPlayer : Player
    {

        /**
         * Variables
         */
        #region Fields

        private readonly List<Building> criticalBuildings = new List<Building>();

        private readonly AIView m_view;

        private Logger log;

        private Random randomFactory;

        private int resourceCriticalThreshold = 1; // The least number of resource hotspots the AI MUST have secured

        private int resourceThreshold = 2; // The ideal number of resource hotspots the AI should have secured

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructor. The AIPlayer requires only an AIView in addition to the parameters needed for a regular Player.
        /// </summary>
        /// <param name="view"></param>
        public AIPlayer(AIView view, Color c)
            : base(c, "AIPLAYER")
        {
            this.InitiateUtils();

            this.m_view = view;
            view.RegisterPlayer(this);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The main method. When called it causes the AIPlayer to reevaluate 
        /// its situation and make appropriate changes.
        /// </summary>
        public void MakeMove()
        {
            this.log.Info("AI Making a Move.");

            this.m_view.LookAtScreen(); // Have the AI View update its local variables
            if (this.m_view.myBuildings.Count == 0)
            {
                return;
            }

            this.resourceThreshold = this.GetResourceThreshold();

            int resourceLocations = this.m_view.GetResourceLocations().Count;
            if (resourceLocations < this.resourceThreshold)
            {// While we have secured less resource points than we should have, get some!
                this.log.Info("Not enough resource points. Need " + this.resourceThreshold + ", have " + resourceLocations);
                this.SecureNextResourceHotSpot();
            }

            if (resourceLocations < this.resourceCriticalThreshold)
            {// If we have not secured basic income, dont worry about anything else.
                this.log.Info("Not enough resource points so nothing more to do this turn.");
                this.log.Info("//Ending turn");
                return;
            }


            if (this.GetGraphs()[0].baseBuilding == null)
            { // Our base building has been destroyed! Create a new one from where we can afford it.
                Building relay = Util.FindBuildingWithUnitCount((int)this.unitAcc.CalculateBuildingCostInflation(Globals.BuildingTypes.Base), this.m_view.myBuildings);
                if (relay == null)
                    return;
                this.IssueBuildOrder(Util.GetRandomBuildPointFrom(Util.CreateMatrixFromInterval(BuildingController.GetValidBuildingInterval(relay.GetPosition(), this.m_view.world)), this.m_view.world), relay, Globals.BuildingTypes.Base);
            }

            // Reset all the building weights.
            this.SetWeights(this.m_view.myBuildings, 0);


            // If we are falling behind on the upgrades: catch up.
            if (this.m_view.opponents[0].PowerLevel > this.PowerLevel)
            {
                this.log.Info("Attempting to upgrading units.");
                this.UpgradeUnits();
            }

            if (this.ShouldAttack())
            {
                this.AttackWeakestFrontPoint();
            }


            var enemyFront = new List<Building>();
            List<Building> front = this.GetFrontLine(enemyFront);
            this.log.Info("Weighing the frontline");
            this.WeighFrontLine(front, enemyFront);
            this.SetCriticalWeights();
            this.log.Info("//Ending turn");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Identify the weakest enemy point at the front and send enough units there
        /// to make a nice dent.
        /// </summary>
        private void AttackWeakestFrontPoint()
        {
            // First, find the weakest enemy point
            Building target = this.GetWeakestPoint();

            // Then find the strongest friendly point
            Building source = this.GetStrongestFriendlyBuilding();
            UnitController.MoveUnits(
                this, 
                Util.GetTileAt(source.GetPosition(), this.m_view.world), 
                Util.GetTileAt(target.GetPosition(), this.m_view.world), 
                source.GetUnits().Count);
        }

        /// <summary>
        /// Iterates over a given list of coordinates representing building coordinates
        /// and sets the weights of those buildings to "SAFE".
        /// </summary>
        /// <param name="path"></param>
        private void ClearHotspotWeights(List<Vector2> path)
        {
            this.log.Info("Clearing all " + (path.Count-1) + " latent path weights.");
            for (int i = 0; i < path.Count-1; i++)
            {
                Vector2 current = path[i];
                Building b = Util.GetBuildingAt(current, this.m_view.world);
                if (b != null)
                {
                    this.log.Info("(" + current.X + ";" + current.Y + ") set to SAFE.");
                    GraphController.Instance.SetWeight(b, this.m_view.SAFE);
                }
            }
        }

        /// <summary>
        /// Method for figuring out the best resource hotspot to take.
        /// The parameter is the closest friendly building.
        /// </summary>
        /// <returns></returns>
        private Vector2 GetClosestResourceHotspot(Vector2 friendly)
        {
            // Get a list of all resource hotspots
            List<Vector2> resources = this.m_view.resourcePoints;
            Vector2 currentBest = resources[0];
            Vector2 closestFriendly = currentBest;

            int currentDist = int.MaxValue;
            int bestDist = int.MaxValue;

            // For every resource hotspot check the distance to the closest friendly.
            for (int i = 0; i < resources.Count; i++)
            {
                Vector2 r = resources[i];
                Building b = Util.GetBuildingAt(r, this.m_view.world);
                if (b != null)
                {
                    // There is already a building here
                    continue;
                }

                closestFriendly = Util.GetClosestPointFromList(r, this.m_view.friendlyPoints);
                currentDist = (int)Vector2.Distance(r, closestFriendly);
                if(currentDist < bestDist)
                {// We have a new leader

                    bestDist = currentDist; // Closest distance
                    currentBest = r; // Closest resource point
                    friendly = closestFriendly;// Closest friendly building location
                }
            }

            return currentBest;
        }

        /// <summary>
        /// Returns a list of all buildings that define the front line, as well
        /// as filling the given list with their corresponding closest enemy.
        /// Overloaded. +1 decides on a distance tolerance.
        /// </summary>
        /// <returns></returns>
        private List<Building> GetFrontLine(List<Building> enemyFront)
        {
            this.log.Info("Finding the front line.");
            List<Building> buildings = this.m_view.myBuildings;
            var result = new List<Building>();
            this.log.Info("Iterating over " + buildings.Count + " buildings");
            for(int i = 0; i < buildings.Count; i++)
            {
                Building temp = buildings[i];
                Building enemy = Util.GetBuildingAt(Util.GetClosestPointFromList(temp.GetPosition(), this.m_view.enemyPoints), this.m_view.world);
                if (Util.GetBuildingAt(Util.GetClosestPointFromList(enemy.GetPosition(), this.m_view.friendlyPoints), this.m_view.world) == temp)
                { // The enemy of my enemy is me, and therefore I am closest to it and it is closest to me.
                    this.log.Info("Adding " + temp.GetPosition().X + ";" +  temp.GetPosition().Y + " to the front list.");
                    result.Add(temp);
                    enemyFront.Add(enemy);
                }
            }

            return result;
        }


        /// <summary>
        /// Returns a list of all the enemy buildings situated close to
        /// our own.
        /// </summary>
        /// <param name="enemyFront"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private List<Building> GetFrontLine(List<Building> enemyFront, int tolerance)
        {
            this.log.Info("Finding the front line.");
            var positions = new List<Vector2>();
            foreach (Vector2 vec in this.m_view.friendlyPoints)
            {
                positions.Add(vec);
            }

            for (int i = 0; i < this.m_view.friendlyPoints.Count; i++)
            {
                var interval = new List<Point>();
                interval.Add(new Point((int)this.m_view.friendlyPoints[i].X - (tolerance/2), (int)this.m_view.friendlyPoints[i].Y - (tolerance/2)));
                interval.Add(new Point((int)this.m_view.friendlyPoints[i].X + (tolerance/2), (int)this.m_view.friendlyPoints[i].Y + (tolerance/2)));
                List<Vector2> vecs = Util.CreateMatrixFromInterval(interval);
                foreach (Vector2 vec in vecs)
                    positions.Add(vec);
            }

            var result = new List<Building>();
            this.log.Info("Iterating over " + positions.Count + " buildings");
            for (int i = 0; i < positions.Count; i++)
            {
                Vector2 temp = positions[i];
                Building enemy = Util.GetBuildingAt(Util.GetClosestPointFromList(temp, this.m_view.enemyPoints), this.m_view.world);
                if (Util.GetBuildingAt(Util.GetClosestPointFromList(enemy.GetPosition(), this.m_view.friendlyPoints), this.m_view.world) == Util.GetBuildingAt(temp, this.m_view.world))
                { // The enemy of my enemy is me, and therefore I am closest to it and it is closest to me.
                    this.log.Info("Adding " + temp.X + ";" + temp.Y + " to the front list.");
                    result.Add(Util.GetBuildingAt(temp, this.m_view.world));
                    enemyFront.Add(enemy);
                }
            }

            return result;
        }

        /// <summary>
        /// Decides how many resource buildings the AI should have
        /// </summary>
        /// <returns></returns>
        private int GetResourceThreshold()
        {
            return (int)this.m_view.opponents[0].CountBuildingsOfType(Globals.BuildingTypes.Resource);
        }

        /// <summary>
        /// Returns the friendly building with the largest amount of units in it
        /// </summary>
        /// <returns></returns>
        private Building GetStrongestFriendlyBuilding()
        {
            List<Building> buildings = this.m_view.myBuildings;
            Building strongest = buildings[0];
            for (int i = 0; i < buildings.Count; i++)
            {
                if (buildings[i].GetUnits().Count > strongest.GetUnits().Count)
                    strongest = buildings[i];
            }

            return strongest;
        }

        /// <summary>
        /// Find the weakest building in the enemies defenses.
        /// </summary>
        /// <returns></returns>
        private Building GetWeakestPoint()
        {
            var enemies = new List<Building>();
            this.GetFrontLine(enemies, 2);
            Building target = enemies[0];

            // Then find the weakest one.
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemies[i].GetUnits().Count < target.GetUnits().Count)
                {
                    target = enemies[i];
                }
            }

            return target;
        }

        /// <summary>
        /// Initiate utilities
        /// </summary>
        private void InitiateUtils()
        {
            this.log = Utility.Logger.LoggerFactory.GetLogger();
            this.randomFactory = new Random();
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
            bool created = BuildingController.AddBuilding(buildingType, sourceBuilding, point, this.m_view.world, this);
            if (created)
                this.m_view.BuildingAddedAt(point);

            return created;
        }

        /// <summary>
        /// Identifies the closest resource hotspot and expands towards it.
        /// </summary>
        private void SecureNextResourceHotSpot()
        {
            this.log.Info("Securing next hotspot.");
            Building sourceBuilding = this.SelectSourceBuilding();
            Vector2 sourcePosition = sourceBuilding.GetPosition();
            Vector2 resource = this.GetClosestResourceHotspot(sourcePosition);
            this.log.Info("Next spot found at " + resource.X + ";" + resource.Y);
            List<Vector2> path = Util.GenerateBuildPathBetween(sourcePosition, resource, this.m_view.world);

            // Skip the parts of the path we have walked before
            for (int i = 0; i < path.Count; i++)
            {
                Building b = Util.GetBuildingAt(path[i], this.m_view.world);
                if (b != null && b.GetOwner() == this)
                {// Already have a building here, take a shortcut
                    this.criticalBuildings.Remove(b);
                    sourceBuilding = b;
                    sourcePosition = sourceBuilding.GetPosition();
                }
            }

            bool built = false;

            if (Util.WithinBuildRangeOf(sourceBuilding.GetPosition(), resource, this.m_view.world))
            {// The resource building is within reach of the source
                this.log.Info("Spot is close enough, expanding from " + sourceBuilding.GetPosition().X + ";" + sourceBuilding.GetPosition().Y);
                built = this.IssueBuildOrder(resource, sourceBuilding, Globals.BuildingTypes.Resource);
                if (built)
                {
                    this.log.Info("Building created.");
                    this.ClearHotspotWeights(path);
                }
                else 
                {
                    this.log.Info("Could not create the building.");
                }
            }
            else
            {
                Vector2 coords = path[0];
                this.log.Info("Spot is too far away, relaying building at " + coords.X + ";" + coords.Y);

                // Relay buildings must be placed first
                built = this.IssueBuildOrder(coords, sourceBuilding, Globals.BuildingTypes.Barrier);
                if (built)
                {
                    this.log.Info("Building created at " + coords.X + ";" + coords.Y);
                    if (this.m_view.GetResourceLocations().Count < this.resourceThreshold)
                    {
                        // If the AI has not secured enough resource spots, set the priority to critical
                        this.log.Info("Importance of this building is CRITICAL");
                        this.criticalBuildings.Add(Util.GetBuildingAt(coords, this.m_view.world));
                    }
                    else
                    {
                        this.log.Info("Importance of this building is THREATENED");
                        GraphController.Instance.SetWeight(
                            Util.GetBuildingAt(coords, this.m_view.world), this.m_view.THREATENED);
                    }
                }
                else
                {
                    // Could not create that building
                    this.log.Info("Could not create the building");
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
            Building b = this.GetGraphs()[0].baseBuilding;
            if (b == null)
            { // Base building destroyed
                b = this.GetGraphs()[0].GetBuildings().First();
            }

            return b;
        }

        /// <summary>
        /// Sets the most critical building's weights. Makes sure that they are not lost.
        /// </summary>
        private void SetCriticalWeights()
        {
            foreach (Building b in this.criticalBuildings) 
            {
                if(b != null)
                    GraphController.Instance.SetWeight(b, this.m_view.CRITICAL);
            }
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
                if(b != null)
                    GraphController.Instance.SetWeight(b, p);
            }
        }

        /// <summary>
        /// Method for deciding whether or not we should attack.
        /// </summary>
        /// <returns></returns>
        private bool ShouldAttack()
        {
            int maby = this.randomFactory.Next(10);
            if (maby > 8)
                return true;

            return false;
        }

        /// <summary>
        /// Upgrade the level of the units by finding a building that can afford
        /// to pay for the upgrade and then ordering the upgrade.
        /// </summary>
        private void UpgradeUnits()
        {
            int cost = this.unitAcc.GetUpgradeCost();
            
            List<Building> buildings = this.m_view.myBuildings;
            Building b = Util.FindBuildingWithUnitCount(cost, buildings);
            if (b != null)
            {
                this.log.Info("Found a suitable building at " + b.GetPosition().X + ";" + b.GetPosition().Y + ", upgrading units.");
                this.unitAcc.PayAndUpgradePower(b);
                return;
            }

            this.log.Info("Could not afford to upgrade.");
        }

        /// <summary>
        /// Iterates over the giving front, setting their weights according to their respective enemies.
        /// </summary>
        /// <param name="front"></param>
        /// <param name="enemyFront"></param>
        private void WeighFrontLine(List<Building> front, List<Building> enemyFront)
        {
            // First, check how many units there are in the enemy front.
            int enemySum = Util.GetUnitCountFrom(enemyFront);
            int mySum = Util.GetUnitCountFrom(front);

            var ratios = new float[front.Count];

            // Then see what their internal ratios are.
            if (enemySum == 0)
            {
                // All enemy buildings have a guaranteed equal distribution
                for (int i = 0; i < ratios.Length; i++)
                {
                    ratios[i] = 1 / enemyFront.Count;
                }
            }
            else
            {
                // Distribution might vary
                for (int i = 0; i < enemyFront.Count; i++)
                {
                    int currentCount = enemyFront[i].GetUnits().Count;
                    if (currentCount != 0) // The building actually has a ratio
                    {
                        ratios[i] = (currentCount) / (float)enemySum;
                    }

                    this.log.Info("Ratio at " + i + " is " + ratios[i]);
                }
            }

            this.log.Info("Front line consists of: ");

            // Finally distribute that same ratio across the AI's own border.
            for (int i = 0; i < front.Count; i++)
            {
                var weight = (int)(ratios[i] * this.m_view.CRITICAL);
                Building temp = front[i];
                this.log.Info("(" + temp.GetPosition().X + ";" + temp.GetPosition().Y + ") weight set to " + weight);
                GraphController.Instance.SetWeight(temp, weight);
            }
        }

        /// <summary>
        /// Returns a coordinate randomly chosen from the opposite quadrant of the map
        /// relative to the AIs base.
        /// </summary>
        /// <returns></returns>
        private Vector2 randomPointAtOppositeQuadrant()
        {
            Vector2 baseCoords = this.GetGraphs()[0].baseBuilding.GetPosition();
            var mapSize = new Vector2(this.m_view.mapWidth, this.m_view.mapHeight);

            var middle = new Vector2(mapSize.X / 2, mapSize.Y / 2);

            Vector2 inner;
            Vector2 outer;

            // The map is divided into four parts as follows:
            // |q1 | q2|
            // |q3 | q4|

            // base in quadrant 1, opponent in q4
            if (baseCoords.X <= middle.X && baseCoords.Y <= middle.Y)
            {
                inner = middle;
                outer = mapSize;
            }
            
                // base in quadrant 2, opponent in q3
            else if (baseCoords.X > middle.X && baseCoords.Y <= middle.Y)
            {
                inner = new Vector2(0, mapSize.Y);
                outer = middle;
            }
                
                // base in quadrant 3, opponent in q2
            else if (baseCoords.X <= middle.X && baseCoords.Y > middle.Y)
            {
                inner = middle;
                outer = new Vector2(mapSize.X, 0);
            }
                
                // base in quadrant 4, opponent in q1
            else
            {
                inner = new Vector2(0, 0);
                outer = middle;
            }

            // Finally pick a set of coordinates within the opposite quadrant.
            float xVal = inner.X + (float)this.randomFactory.NextDouble() * (outer.X-inner.X);
            float yVal = inner.Y + (float)this.randomFactory.NextDouble() * (outer.Y-inner.Y);
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

            var result = new Vector2(xVal, yVal);

            return result;
        }

        #endregion
    }
}