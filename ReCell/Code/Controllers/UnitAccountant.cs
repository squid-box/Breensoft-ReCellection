namespace Recellection.Code.Controllers
{
    using System;
    using System.Collections.Generic;

    using global::Recellection.Code.Models;

    using global::Recellection.Code.Utility.Logger;

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
        #region Constants

        private const int FIRST_POWER_LEVEL_COST = 10;

        private const int MAX_POWER_LEVEL_LEVELS = 4;

        private const uint POP_CAP_PER_PLAYER = 200;

        #endregion

        #region Fields

        private readonly uint[] MAX_OF_EACH_BUILDING_TYPE = { 0, 7, 30, 4, 9 };

        private readonly Logger logger = LoggerFactory.GetLogger();
        private readonly Player owner;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructs an UnitAccountant.
        /// </summary>
        /// <param name="owner">This UnitAccountant will belong to the player 'owner'.</param>
        public UnitAccountant(Player owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Called by fromBuilding, adds units to a fromBuilding.
        /// </summary>
        /// <param name="b">The fromBuilding to add units to.</param>
        /// <param name="units">A list of units.</param>
        public void AddUnits(Building b, List<Unit> units)
        {
            b.AddUnits(units);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="payer">The player this building is built for</param>
        /// <returns>The cost when considering the price inflation</returns>
        public uint CalculateBuildingCostInflation(Globals.BuildingTypes type)
        {
            uint defaultCost = Building.GetBuyPrice(type);
            uint buildingCount = this.owner.CountBuildingsOfType(type);

            // return (uint)(defaultCost + (buildingCount * buildingCount * defaultCost / 2));
            if (buildingCount == 0)
            {
                return defaultCost;
            }

            double exp = 1f / Math.Pow(buildingCount, 1f / (this.MAX_OF_EACH_BUILDING_TYPE[(int)type] - 1f));
            double bas = 1f / (this.MAX_OF_EACH_BUILDING_TYPE[(int)type] - 1f);
            double test = Math.Pow(bas, exp);
            return (uint)(POP_CAP_PER_PLAYER * test);
        }

        public void DestroyUnits(List<Unit> u, int n)
        {
            UnitController.MarkUnitsAsDead(u, n);
            UnitController.RemoveDeadUnits();
        }

        public int GetUpgradeCost()
        {
            if (this.owner.PowerLevel +  this.owner.SpeedLevel >= 0.1f*MAX_POWER_LEVEL_LEVELS)
            {
                return 0x0C00FEE;
            }

            if (this.owner.PowerLevel+this.owner.SpeedLevel == 0.0f)
            {
                return FIRST_POWER_LEVEL_COST;
            }

            float level = this.owner.PowerLevel+this.owner.SpeedLevel * 10f;
            double exp = 1f / Math.Pow(level, 1f / (MAX_POWER_LEVEL_LEVELS - 1f));
            double bas = 1f / (MAX_POWER_LEVEL_LEVELS - 1f);
            double result = Math.Pow(bas, exp);
            return (int)(POP_CAP_PER_PLAYER * result);
        }

        public bool PayAndUpgradePower(Building building)
        {
			if (building.units.Count < this.GetUpgradeCost() || (this.owner.PowerLevel + this.owner.SpeedLevel) >= 0.6f)
            {
                return false;
            }

            this.DestroyUnits(building.units, this.GetUpgradeCost());
            this.owner.PowerLevel += 0.1f;
            return true;

        }

		public bool PayAndUpgradeSpeed(Building building)
		{
			if (building.units.Count < this.GetUpgradeCost() || (this.owner.PowerLevel+this.owner.SpeedLevel) >= 0.6f)
			{
				return false;
			}

			this.DestroyUnits(building.units, this.GetUpgradeCost());
			this.owner.SpeedLevel += 0.1f;
			return true;
		}

        /// <summary>
        /// Quite possibly a horribly slow way of adding units.
        /// </summary>
        public void ProduceUnits()
        {
            var randomer = new Random();
            

            uint totalUnits = this.owner.CountUnits();

            foreach (Graph g in this.owner.GetGraphs())
            {
                if (totalUnits >= POP_CAP_PER_PLAYER)
                    break;

				var res = new List<Unit>();

                // TODO Remove when middle point position is implemented.
                
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

                this.logger.Debug("Producing " + unitsToProduce + " units!");
                if (unitsToProduce > 0)
                {
                    totalUnits += (uint)unitsToProduce;
                }

                for (int i = 0; i < unitsToProduce; i++)
                {
                    // Places them randomly around the fromBuilding. - John
                    // No, it does not. - Martin
                    var temp = new Unit(b.owner, b.position, b);
                    
                    res.Add(temp);
                }

                b.owner.AddUnits(res);
                b.AddUnits(res);

                // This should not be needed but does not hurt i hope.
                res.Clear();
            }
        }

        #endregion
	}
}
