using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Recellection.Code.Models
{
/*
 * Utility class for the AI.
 * Contains "tumor" code that perform simple tasks only requiring their parameters.
 * 
 **/
    public class Util
    {

        /// <summary>
        /// Returns the point in the given list that is closest to the given point.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Vector2 GetClosestPointFromList(Vector2 point, List<Vector2> list)
        {
            Vector2 best = list[0];
            Vector2 temp = best;
            for (int i = 1; i < list.Count; i++)
            {
                temp = list[i];
                if (Vector2.Distance(temp, point) < Vector2.Distance(best, point))
                {
                    best = temp;
                }
            }
            return best;
        }


        /// <summary>
        /// Reuturns the first building from the given list tha has the given amount of
        /// units or more. If none was found null is returned.
        /// </summary>
        /// <param name="units"></param>
        /// <param name="buildings"></param>
        public static Building FindBuildingWithUnitCount(int units, List<Building> buildings)
        {
            for (int i = 0; i < buildings.Count; i++)
            {
                Building b = buildings[i];
                if (b.GetUnits().Count > units)
                {
                    return b;
                }
            }
            return null;
        }

    }
}
