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

        Tile[][] world;
        Tile[][] visibleWorld;
        Dictionary<Building, Globals.BuildingTypes> buildingTypes;



        public AIView()
        {
        }

        /*
         * Returns the Tile located in the given coordinates provided that it is visible.
         */
        internal Tile getTileAt(Vector2 coords)
        {
            Tile newTile = new Tile();
       
            return visibleWorld[(int)coords.X][(int)coords.Y];
        }


        /*
         * Checks whether or not there is a Resource Point at the given coordinates.
         */
        internal bool ContainsResourcePoint(Vector2 current)
        {
            Tile tempTile = getTileAt(current);
            if (tempTile.GetTerrainType().getResourceModifier() > 0)
            {
                return true;
            }
            return false;
        }

        internal Building GetBuildingAt(Vector2 point)
        {
            return getTileAt(point).GetBuilding();
        }

        internal Globals.BuildingTypes GetBuildingTypeOf(Building building)
        {
            return buildingTypes[building];
        }
    }
}
