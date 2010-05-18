using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Controllers
{
    /// <summary>
    /// Handles logic needed for the Sounds model.
    /// Author: Mattias Mikkola
    /// </summary>
    class SoundsController
    {
        private static World theWorld;
 
        /// <param name="worldInstance">A World instance used to calculate distance to objects</param>
        public SoundsController(World worldInstance)
        {
            theWorld = worldInstance;
        }

        /// <summary>
        /// Plays a sound at the normal volume.
        /// </summary>
        /// <param name="soundIdentifier">The sound to play.</param>
        public static void playSound(String soundIdentifier)
        {
            Sounds.Instance.LoadSound(soundIdentifier).Play();
        }

        /// <summary>
        /// Plays a sound coming from a specific object.
        /// </summary>
        /// <param name="soundIdentifier">The sound to play.</param>
        /// <param name="point">The point where the object is located.</param>
        public static void playSound(String soundIdentifier, Point point)
        {
			playSound(soundIdentifier, new Vector2(point.X, point.Y));
        }

        public static void playSound(String soundIdentifier, Vector2 point)
		{
			if (theWorld == null)
			{
				throw new FieldAccessException("World object not instantiated.");
			}
			Point lookingAt = theWorld.LookingAt;

            
			float length = (new Vector2((lookingAt.X + ((Globals.VIEWPORT_WIDTH/Globals.TILE_SIZE) / 2)), (lookingAt.Y + ((Globals.VIEWPORT_HEIGHT/Globals.TILE_SIZE) / 2))) - point).Length();

			float newVolume = (GameOptions.Instance.musicVolume/-96.0f)*(50.0f * (float) Math.Log10(-0.05f * length + 1.0f));

            LoggerFactory.GetLogger().Trace("Playing sound at volume: " + newVolume + "db");

			Cue cue = Sounds.Instance.LoadSound(soundIdentifier);
			cue.SetVariable("Volume", newVolume);
			cue.Play();
        }

        public static void changeMusicVolume(float percentage)
        {
            Sounds.Instance.GetCategory("Music").SetVolume(percentage);
        }

        public static void changeEffectsVolume(float percentage)
        {
            Sounds.Instance.GetCategory("Effects").SetVolume(percentage);
        }
    }
}
