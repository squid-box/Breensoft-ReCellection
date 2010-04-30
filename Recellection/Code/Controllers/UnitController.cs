using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Controllers
{
    public class UnitController
    {

        public void Update(IEnumerator<Unit> units, int deltaT)
        {
            while(units.MoveNext())
            {
                units.Current.Update(deltaT);

            }
        }

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
                u.SetTargetX(to.position);
                if (u.IsDispersed())
                {
                    tempUnit.Add(u);
                    u.SetDispersed(false);
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
        public void KillUnits(IEnumerable<Unit> units, int amount)
        {
            foreach (Unit u in units)
            {
                if (amount <= 0)
                {
                    u.Kill();
                    amount--;
                }
            }
        }

        public void Update(IEnumerable<Unit> units, int systemTime)
        {
            foreach (Unit u in units)
            {
                // Check whether or not we just arrived to a building
                bool travelling = u.IsDispersed();
                
                u.Update(systemTime);
                if (travelling && u.IsDispersed())
                {
                    Vector2 pos = u.GetPosition();
                    
                }
                
            }
        }

    }
}
