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
        /// Returns a Cue from the Sounds Model to comply with MVC.
        /// </summary>
        /// <param name="soundIdentifier">The Sound to return.</param>
        /// <returns>A Cue object for that sound</returns>
        public Cue GetCue(String soundIdentifier)
        {
            return Sounds.Instance.LoadSound(soundIdentifier);
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

        /// <summary>
        /// Plays a sound from a specific object.
        /// </summary>
        /// <param name="soundIdentifier">The sound to play.</param>
        /// <param name="point">A Vector2 object describing the point where the object is located.</param>
        public static void playSound(String soundIdentifier, Vector2 point)
		{
			if (theWorld == null)
			{
				throw new FieldAccessException("World object not instantiated.");
			}
			Point lookingAt = theWorld.LookingAt;

			float length = (new Vector2((lookingAt.X + ((Globals.VIEWPORT_WIDTH/Globals.TILE_SIZE) / 2)), (lookingAt.Y + ((Globals.VIEWPORT_HEIGHT/Globals.TILE_SIZE) / 2))) - point).Length();

			float newVolume = GameOptions.Instance.sfxVolume*(80.0f * (float) Math.Log10(-0.05f * length + 1.0f));

            LoggerFactory.GetLogger().Trace("Playing sound at volume: " + newVolume + "db");

			Cue cue = Sounds.Instance.LoadSound(soundIdentifier);
			cue.SetVariable("Volume", newVolume);
			cue.Play();
        }

        /// <summary>
        /// Change the music volume.
        /// </summary>
        /// <param name="percentage">Ranges from 0.0 to 2.0, where 0.0 is -96db, 1.0 is 0db and 2.0 is +6db</param>
        public static void changeMusicVolume(float percentage)
        {
            Sounds.Instance.GetCategory("Music").SetVolume(percentage);
            GameOptions.Instance.musicVolume = percentage;
        }

        /// <summary>
        /// Change the effects volume.
        /// </summary>
        /// <param name="percentage">Ranges from 0.0 to 2.0, where 0.0 is -96db, 1.0 is 0db and 2.0 is +6db</param>
        public static void changeEffectsVolume(float percentage)
        {
            Sounds.Instance.GetCategory("Effects").SetVolume(percentage);
            GameOptions.Instance.sfxVolume = percentage;
        }

        /// <summary>
        /// Mute all sounds from the game.
        /// </summary>
		public static void Mute()
        {
			Sounds.Instance.GetCategory("Music").SetVolume(0.0f);
			Sounds.Instance.GetCategory("Effects").SetVolume(0.0f);
		}
    }
}
