using System;
using Microsoft.Xna.Framework;

/* Contains all global variables and enumerations in the game.
 *
 */

namespace Recellection
{
    public sealed class Globals
    {
        public enum Songs
        {
            Theme
        }
        public enum RegionCategories
        {
            MainMenu = 0, HelpMenu = 1
        }
        public enum TerrainTypes
        {
            Membrane, Mucus, Water, Slow, Infected
        }

        public enum BuildingTypes
        {
            NoType, Aggressive, Barrier, Base, Resource
        }

        public enum Languages
        {
            English, Swedish
        }

        public enum TextureTypes
        {
            Membrane, Mucus, Water, Slow, Infected, BaseBuilding, 
            BarrierBuilding, AggressiveBuilding, ResourceBuilding
        }
    }
}
