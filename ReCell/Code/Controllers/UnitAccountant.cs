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

        public int getUpgradeCost()
        {
            if (owner.powerLevel >= 0.6f)
            {
                return 4711;
            }
            return (int) ((20 * owner.powerLevel) * (20 * owner.powerLevel)) +4;//TODO change to a more sane formula.
        }

        public bool payAndUpgrade(Building building)
        {
            if (building.units.Count < getUpgradeCost() || owner.powerLevel >= 0.6f)
            {
                return false;
            }
            destroyUnits(building.units, getUpgradeCost());
            owner.powerLevel += 0.1f;
            return true;

        }
        public void destroyUnits(List<Unit> u, int n)
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

                    res.Add(new Unit(b.owner, b.position, b));
                }
                b.AddUnits(res);
            }
        }
    }
}
