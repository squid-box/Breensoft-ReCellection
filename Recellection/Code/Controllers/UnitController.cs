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

        public void MoveUnits(int amount, Tile from, Tile to)
        {
            List<Unit> tempUnit = new List<Unit>();
            foreach (Unit u in from.GetUnits())
            {
                if (amount <= 0)
                    break;
                // TODO
                // Set new target for unit
                tempUnit.Add(u);
                amount--;
            }
            // TODO
            // find out where to remove and add the moved units
            // from.RemoveUnits(tempUnit);
            // to.AddUnits(tempUnit);
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
                u.Update(systemTime);
            }
        }

    }
}
