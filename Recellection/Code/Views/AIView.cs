using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Recellection.Code.Models;

/*
 * The AI View keeps track of things relevant for the AI Player.
 * 
 * Author: Lukas Mattsson
 */

namespace Recellection.Code
{
    class AIView
    {
        private Player ai;
        private World world;
        public List<Building> myBuildings { get; internal set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="p_world"></param>
        public AIView(World p_world)
        {
            world = p_world;
        }

        /// <summary>
        /// Internal function. Allows the AI Player to register itself so that this view can keep track
        /// of who it is making calls for.
        /// </summary>
        /// <param name="p"></param>
        internal void registerPlayer(Player p)
        {
            ai = p;
        }

        /// <summary>
        /// Returns the Tile located in the given coordinates provided that it is visible.
        /// If it is not visible, null is returned.
        /// </summary>
        /// <param name="coords"></param>
        /// <returns></returns>
        internal Tile getTileAt(Vector2 coords)
        {
            Tile tempTile = world.GetMap().GetTile((int)coords.X, (int)coords.Y);
            if (tempTile.IsVisible(ai))
            {
                return tempTile;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Checks whether or not there is a Resource Point at the given coordinates.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        internal bool ContainsResourcePoint(Vector2 current)
        {
            Tile tempTile = getTileAt(current);
            if (tempTile.GetTerrainType().getResourceModifier() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the building at the given coordinates provided that it is visible.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal Building GetBuildingAt(Vector2 point)
        {
            return getTileAt(point).GetBuilding();
        }

        /// <summary>
        /// Returns the coordinates of all the friendly buildings
        /// </summary>
        /// <returns></returns>
        internal List<Vector2> GetFriendlyBuildings()
        {
            List<Vector2> coordinates = new List<Vector2>();
            for (int i = 0; i < coordinates.Count; i++)
            {
                coordinates.Add(myBuildings[i].coordinates);
            }
            return coordinates;
        }
    }
}
