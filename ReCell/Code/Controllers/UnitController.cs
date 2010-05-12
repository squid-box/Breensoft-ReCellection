using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        /// <summary>
        /// Move a set of units from one tile to another
        /// </summary>
        /// <param name="amount">The amount of units to be moved</param>
        /// <param name="from">The tile to move units from</param>
        /// <param name="to">Tile tile to move units to</param>
        public static void MoveUnits(int amount, Tile from, Tile to)
        {
            List<Unit> tempUnit = new List<Unit>();

            IEnumerable<Unit> units;
            bool building = false;
            // If we are moving units from a building, they might not
            // all be on the same tile
            if (from.GetBuilding() != null)
            {
                units = from.GetBuilding().GetUnits();
                building = true;
            }
            else
            {
                units = from.GetUnits();
            }
           
            // Move some units from the target tile
            foreach (Unit u in units)
            {
                if (amount <= 0)
                    break;
                u.targetPosition = to.position;
                if (u.isDispersed)
                {
                    tempUnit.Add(u);
                    u.isDispersed = true;
                    amount--;
                }
            }
            // We don't want to remove units from a tile, as they do it themselves
            if (building)
            {
                from.GetBuilding().RemoveUnits(tempUnit);

            }

            // If we are moving them to a building, add the units to that building
            if (to.GetBuilding() != null)
            {
                units = to.GetBuilding().GetUnits();
            }
            else
            {
                units = to.GetUnits();
            }
        }

        /// <summary>
        /// Kill a number of units in a set. Kills the first 'amount' units in the
        /// set.
        /// </summary>
        /// <param name="units"></param>
        /// <param name="amount"></param>
        public static void KillUnits(IEnumerable<Unit> units, int amount)
        {
            foreach (Unit u in units)
            {
                if (amount >= 0)
                {
                    u.Kill();
                    amount--;
                }
            }
        }

        /// <summary>
        /// Call the update method on each unit causing them to move towards their target.
        /// If the unit has arrived to its target, it will recieve new orders. If it was previously
        /// travelling to a building, it will be joined with that building upon arrival.
        /// </summary>
        /// <param name="units">The set of units to be updated</param>
        /// <param name="systemTime">The time passed since something</param>
        public static void Update(IEnumerable<Unit> units, int systemTime)
        {
            foreach (Unit u in units)
            {
                
                u.Update(systemTime);
                // We we arrive to our target
                if (u.hasArrived)
                {
                    Vector2 tilePos = u.targetPosition;
                    Vector2 min = new Vector2((float)Math.Floor(tilePos.X), (float)Math.Floor(tilePos.Y));
                    
                    Random r = new Random();
                    float rX = (float)r.NextDouble() + min.X;
                    float rY = (float)r.NextDouble() + min.Y;

                    u.targetPosition = new Vector2(rX, rY);
					u.hasArrived = false;
                }
            }
        }
    }
}
