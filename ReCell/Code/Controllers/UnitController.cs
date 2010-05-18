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
		private static List<Unit> toBeKilled = new List<Unit>();
		
        /// <summary>
        /// Move a set of units from one tile to another
        /// </summary>
        /// <param name="amount">The amount of units to be moved</param>
        /// <param name="from">The tile to move units from</param>
        /// <param name="to">Tile tile to move units to</param>
        public static void MoveUnits(int amount, Tile from, Tile to)
        {
			logger.SetThreshold(LogLevel.DEBUG);
			
            bool fromBuilding = (from.GetBuilding() != null);
            bool toBuilding = (to.GetBuilding() != null);
            logger.Debug("Moving "+amount+" units from "+from+" to "+to);

			List<Unit> units;
            // If we are moving units from a fromBuilding, they might not
            // all be on the same tile
            if (fromBuilding)
			{
				units = from.GetBuilding().GetUnits();
				logger.Debug("Moving "+units.Count()+" ("+from.GetBuilding().CountUnits()+") units from a building.");
            }
            else
			{
                units = from.GetUnits();
				logger.Debug("Moving "+units.Count()+" from a tile!");
			}

			List<Unit> toBeRemovedFromBuilding = new List<Unit>();

			// Move some units from the targetted tile
            lock (units)
            {
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

                    if (toBuilding)
                    {
                        logger.Trace("Adding unit to patrol the 'to' building!");
                        u.TargetEntity = to.GetBuilding();
                        u.rallyPoint = to.GetBuilding();
                    }
                }
            }
            
            // We don't want to remove units from a tile, as they do it themselves
            if (fromBuilding)
            {
				logger.Info("Removing " + toBeRemovedFromBuilding.Count + " units from building with "+from.GetBuilding().CountUnits()+" units.");
				from.GetBuilding().RemoveUnits(toBeRemovedFromBuilding);
				logger.Info("There is now "+from.GetBuilding().CountUnits()+" units in the building.");
            }
        }

        /// <summary>
        /// Kill a number of units in a set. Kills the first 'amount' units in the
        /// set.
        /// </summary>
        /// <param name="units"></param>
        /// <param name="amount"></param>
        public static void MarkUnitsAsDead(IEnumerable<Unit> units, int amount)
        {
            logger.Info("Unit Controller has be ordered to assasinate "+amount + " units.");
            foreach (Unit u in units)
            {
                if (amount > 0)
                {
					MarkUnitAsDead(u);
					amount--;
                }
            }
        }
        
        public static void MarkUnitAsDead(Unit u)
        {
			toBeKilled.Add(u);
		}

		public static void RemoveDeadUnits()
		{
			foreach (Unit u in toBeKilled)
			{
				u.RemoveFromWorld();
			}
			toBeKilled.Clear();
		}

        /// <summary>
        /// Call the update method on each unit causing them to move towards their target.
        /// If the unit has arrived to its target, it will recieve new orders. If it was previously
        /// travelling to a fromBuilding, it will be joined with that fromBuilding upon arrival.
        /// </summary>
        /// <param name="units">The set of units to be updated</param>
        /// <param name="systemTime">The time passed since something</param>
        public static void Update(IEnumerable<Unit> units, int systemTime, World.Map worldMap)
		{
            lock (units)
            {
                foreach (Unit u in units)
                {
                    // Try to find enemies for this unit!
                    FindEnemy(u, worldMap);

                    // Update position
                    u.Update(systemTime);
                }

				RemoveDeadUnits();
            }
        }
        
        private static void FindEnemy(Unit u, World.Map worldMap)
		{
			if (u.TargetEntity == null
			 || u.TargetEntity.owner != u.owner)
			{
				Tile t = worldMap.GetTile((int)u.position.X, (int)u.position.Y);
				
				// Search for units
                lock (t.GetUnits())
                {
                    foreach (Unit ou in t.GetUnits(u.owner.Enemy))
                    {
                        // Is this an enemy?
                        if (u.owner != ou.owner && ! ou.isDead)
                        {
                            // FIGHT TO THE DEATH!
                            u.TargetEntity = ou;
                            return;
                        }
                    }
                }
				
				// Is there a building to kill?
				if (t.GetBuilding() != null && t.GetBuilding().owner != u.owner)
				{
					u.TargetEntity = t.GetBuilding();
					return;
				}
			}
        }
    }
}
