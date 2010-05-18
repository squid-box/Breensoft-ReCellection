using System;
using Microsoft.Xna.Framework;

/* Contains all global variables and enumerations in the game.
 *
 */

namespace Recellection
{
    public sealed class Globals
    {
        public const int TILE_SIZE = 128;
        public static int VIEWPORT_WIDTH = 1024;
        public static int VIEWPORT_HEIGHT = 768;

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

        public enum TextureTypes
        {
            Membrane, Mucus, Water, Slow, Infected, BaseBuilding,
            BarrierBuilding, AggressiveBuilding, ResourceBuilding, Unit, Kamikaze, white, logo,

            MainMenu, OptionsMenu, Help, PromptMenu, GetIntMenu,
            CommandMenu, SpecialCommandMenu, ThreeByThree, TwoByTwo, NoTexture, Yes, No, Light, Pixel,

			Priority1, Priority2, Priority3, Priority4, Priority5, Priority6, Priority7, Priority8,

			ActiveTile, 
			
			ScrollUp, ScrollDown, ScrollLeft, ScrollRight,
			ScrollUpLeft, ScrollUpRight, ScrollDownLeft, ScrollDownRight
        }

        public enum GameStates
        {
            StartUp,
            Menu,
            Game,
            Help
        };

        public enum MenuLayout
        {
            Prompt,
            NineMatrix,
            FourMatrix,
		    FreeStyle
        };
    }
}
