using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Recellection.Code.Controllers;

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


        /// <summary>
        /// Converts a given tile interval to a matrix of tile coordinates.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Vector2> CreateMatrixFromInterval(List<Point> list)
        {
            List<Vector2> result = new List<Vector2>();

            for (int i = list[0].X; i <= list[1].X; i++)
                for (int j = list[0].Y; j <= list[1].Y; j++)
                {
                    result.Add(new Vector2(i, j));
                }
            return result;
        }

        /// <summary>
        /// Returns the total number of units in the given list of buildings
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static int GetUnitCountFrom(List<Building> b)
        {
            int uSum = 0;
            for (int i = 0; i < b.Count; i++)
            {
                uSum += b[i].GetUnits().Count;
            }
            return uSum;
        }

        /// <summary>
        /// Checks if the given source building is within building range of dest.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public static bool WithinBuildRangeOf(Vector2 source, Vector2 dest, World world)
        {
            List<Point> valid = BuildingController.GetValidBuildingInterval(dest, world);

            Point v1 = valid[0];
            Point v2 = valid[1];

            if ((int)source.X < v1.X || (int)source.X > v2.X || (int)source.Y < v1.Y || (int)source.Y > v2.Y)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Generates a list of optimal building placements for connecting the two given points.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="sourcePosition"></param>
        /// <returns></returns>
        public static List<Vector2> GenerateBuildPathBetween(Vector2 p_source, Vector2 p_dest, World world)
        {
            Vector2 source = p_source;
            Vector2 dest = p_dest;

            List<Vector2> path = new List<Vector2>();
            do
            {
                source = Util.GetClosestPointFromList(dest, CreateMatrixFromInterval(BuildingController.GetValidBuildingInterval(source, world)));
                path.Add(source);
            } while (!WithinBuildRangeOf(source, dest, world));
            if (source != dest) //safeguard for double add
                path.Add(p_dest);

            return path;
        }

        /// <summary>
        /// Does exactly as promised.
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Vector2 GetRandomPointFrom(List<Vector2> list)
        {
            Random randomFactory = new Random();
            return list[randomFactory.Next(list.Count)];
        }
    }
}
