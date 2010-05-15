using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// This class delegates the update-cycle to each unit allowing them to move in the world.
    /// The Unit controller also gives orders to units to move to another position.
    /// 
    /// Author: Marco
    /// </summary>
    public class UnitController
    {
		private static Logger logger = LoggerFactory.GetLogger();
        /// <summary>
        /// Move a set of units from one tile to another
        /// </summary>
        /// <param name="amount">The amount of units to be moved</param>
        /// <param name="from">The tile to move units from</param>
        /// <param name="to">Tile tile to move units to</param>
        public static void MoveUnits(int amount, Tile from, Tile to)
        {
            bool fromBuilding = (from.GetBuilding() != null);
            bool toBuilding = (to.GetBuilding() != null);
            logger.Debug("Moving "+amount+" units from "+from+" to "+to);

			HashSet<Unit> units;
            // If we are moving units from a fromBuilding, they might not
            // all be on the same tile
            if (fromBuilding)
			{
				units = from.GetBuilding().GetUnits();
				logger.Debug("Moving "+units.Count()+" units from a building!");
            }
            else
			{
                units = from.GetUnits();
				logger.Debug("Moving "+units.Count()+" from a tile!");
			}

			List<Unit> toBeRemovedFromBuilding = new List<Unit>();

			// Move some units from the targetted tile
            foreach (Unit u in units)
            {
                if (amount <= 0)
                {
                    break;
                }

				u.targetPosition = to.position;
				amount--;
                
                if (u.isPatrolling())
                {
					logger.Trace("Unit is patrolling! Removing from rotation.");
                    toBeRemovedFromBuilding.Add(u);
                }
                
                if (toBuilding && to.GetBuilding().owner == u.owner)
                {
					logger.Trace("Adding unit to patrol the 'to' building!");
					to.GetBuilding().AddUnit(u);
                }
            }
            
            // We don't want to remove units from a tile, as they do it themselves
            if (fromBuilding)
            {
                from.GetBuilding().RemoveUnits(toBeRemovedFromBuilding);
            }
			logger.Debug("");
        }

        /// <summary>
        /// Kill a number of units in a set. Kills the first 'amount' units in the
        /// set.
        /// </summary>
        /// <param name="units"></param>
        /// <param name="amount"></param>
        public static void KillUnits(IEnumerable<Unit> units, int amount)
        {
			List<Unit> toBeKilled = new List<Unit>();
            foreach (Unit u in units)
            {
                if (amount >= 0)
                {
					toBeKilled.Add(u);
					amount--;
                }
            }
            
            foreach(Unit u in toBeKilled)
            {
				u.GetPosition();
                u.Kill();
			}
        }

        /// <summary>
        /// Call the update method on each unit causing them to move towards their target.
        /// If the unit has arrived to its target, it will recieve new orders. If it was previously
        /// travelling to a fromBuilding, it will be joined with that fromBuilding upon arrival.
        /// </summary>
        /// <param name="units">The set of units to be updated</param>
        /// <param name="systemTime">The time passed since something</param>
        public static void Update(IEnumerable<Unit> units, int systemTime)
        {
            foreach (Unit u in units)
            {
                u.Update(systemTime);
            }
        }
    }
}
