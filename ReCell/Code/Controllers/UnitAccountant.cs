using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// The purpose of the Unit Accountant is to insert new units into its
    /// graph as they are created by the buildings.
    /// 
    /// </summary>
    /// <author>Joel Ahlgren</author>
    /// <date>2010-05-07</date>
    /// 
    /// Signature: John Doe (yyyy-mm-dd)
    /// Signature: Jane Doe (yyyy-mm-dd)
    public sealed class UnitAccountant
	{
        private const uint POP_CAP_PER_PLAYER = 200;
        private readonly uint[] MAX_OF_EACH_BUILDING_TYPE = { 0, 7, 11, 4, 9 };
        private const int MAX_POWER_LEVEL_LEVELS = 4;
        private const int FIRST_POWER_LEVEL_COST = 10;

		private Logger logger = LoggerFactory.GetLogger();
        private Player owner;

        /// <summary>
        /// Constructs an UnitAccountant.
        /// </summary>
        /// <param name="owner">This UnitAccountant will belong to the player 'owner'.</param>
        public UnitAccountant(Player owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Called by fromBuilding, adds units to a fromBuilding.
        /// </summary>
        /// <param name="b">The fromBuilding to add units to.</param>
        /// <param name="units">A list of units.</param>
        public void AddUnits(Building b, List<Unit> units)
        {
            b.AddUnits(units);
        }

        public int GetUpgradeCost()
        {
            if (owner.powerLevel >= 0.1f*MAX_POWER_LEVEL_LEVELS)
            {
                return (int)0x0C00FEE;
            }
            if (owner.powerLevel == 0.0f)
            {
                return FIRST_POWER_LEVEL_COST;
            }
            float level = (owner.powerLevel * 10f);
            double exp = 1f / Math.Pow((level), 1f / ((float)MAX_POWER_LEVEL_LEVELS - 1f));
            double bas = 1f / ((float)MAX_POWER_LEVEL_LEVELS - 1f);
            double result = Math.Pow(bas, exp);
            return (int)(POP_CAP_PER_PLAYER * result);
        }

        public bool PayAndUpgrade(Building building)
        {
            if (building.units.Count < GetUpgradeCost() || owner.powerLevel >= 0.6f)
            {
                return false;
            }
            DestroyUnits(building.units, GetUpgradeCost());
            owner.powerLevel += 0.1f;
            return true;

        }
        public void DestroyUnits(List<Unit> u, int n)
        {
            UnitController.MarkUnitsAsDead(u, n);
            UnitController.RemoveDeadUnits();
        }

        /// <summary>
        /// Quite possibly a horribly slow way of adding units.
        /// </summary>
        public void ProduceUnits()
        {
            Random randomer = new Random();
            

            uint totalUnits = owner.CountUnits();

            foreach (Graph g in owner.GetGraphs())
            {
				List<Unit> res = new List<Unit>();

                //TODO Remove when middle point position is implemented.
                

                BaseBuilding b = g.baseBuilding;
                if (b == null)
                {
                    continue;
                }
                int unitsToProduce = b.RateOfProduction;
                if (b.RateOfProduction + totalUnits > POP_CAP_PER_PLAYER)
                {
                    unitsToProduce = (int) (POP_CAP_PER_PLAYER - totalUnits);
                }
                logger.Debug("Producing " + unitsToProduce + " units!");

                for (int i = 0; i < unitsToProduce; i++)
                {
                    // Places them randomly around the fromBuilding. - John
                    // No, it does not. - Martin

                    Unit temp = new Unit(b.owner, b.position, b);
                    
                    res.Add(temp);
                }
                b.owner.AddUnits(res);
                b.AddUnits(res);
            }
        }

        /// <summary>
        /// The increese in cost is 50% extra for each building of that type built.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="payer">The player this building is built for</param>
        /// <returns>The cost when considering the price inflation</returns>
        public uint CalculateBuildingCostInflation(Globals.BuildingTypes type)
        {
            uint defaultCost = Building.GetBuyPrice(type);
            uint buildingCount = owner.CountBuildingsOfType(type);
            //return (uint)(defaultCost + (buildingCount * buildingCount * defaultCost / 2));
            if(buildingCount == 0)
            {
                return defaultCost;
            }
            double exp = 1f / Math.Pow((buildingCount), 1f / ((float)MAX_OF_EACH_BUILDING_TYPE[(int)type] - 1f));
            double bas = 1f /((float)MAX_OF_EACH_BUILDING_TYPE[(int)type] - 1f);
            double test = Math.Pow(bas, exp);
            return (uint)(POP_CAP_PER_PLAYER * test);
        }
    }
}
