using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

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

			float length = (new Vector2((lookingAt.X + (Globals.VIEWPORT_WIDTH / 2)), (lookingAt.Y + (Globals.VIEWPORT_HEIGHT / 2))) - point).Length();

			float volumeModifier = -0.05f * length + 1.0f;

			Cue cue = Sounds.Instance.LoadSound(soundIdentifier);
			cue.SetVariable("Volume", volumeModifier);
			cue.Play();
        }
        #region TODO
        public void changeMusicVolume(float percentage)
        {
            /// TODO: IMPLEMENT
        }

        public void changeEffectsVolume(float percentage)
        {
            /// TODO: IMPLEMENT
        }
        #endregion
    }
}
