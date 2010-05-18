using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Recellection.Code.Models;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace Recellection.Code.Models
{
    /// <summary>
    /// The Sounds class is a Model that handles all sounds for the game.
    /// 
    /// author: Mattias Mikkola
    /// </summary>
    public sealed class Sounds : IModel
    {
        private static AudioEngine audioEngine;
        private static SoundBank soundBank;
        private static WaveBank waveBank;
        public bool isMuted { get; set; }

        public static Sounds Instance { get; private set; }

        /// <summary>
        /// Static constructor will initialize instance when referenced.
        /// </summary>
        static Sounds()
        {
            Instance = new Sounds();
        }

        /// <summary>
        /// Private constructor that initializes all members.
        /// </summary>
        private Sounds()
        {
            audioEngine = new AudioEngine("Content/Sounds/RecellectionSounds.xgs");
            soundBank = new SoundBank(audioEngine, "Content/Sounds/Sound Bank.xsb");
            waveBank = new WaveBank(audioEngine, "Content/Sounds/Wave Bank.xwb");
        }

        /// <summary>
        /// Loads a sound from the soundbank and returns the corresponding Cue object.
        /// </summary>
        /// <param name="sound">The identifier for the sound to load.</param>
        /// <returns>The Cue file for the sound.</returns>
        public Cue LoadSound(string sound)
        {
            return soundBank.GetCue(sound);
        }

        /// <summary>
        /// Return a specific audio category from the AudioEngine object.
        /// </summary>
        /// <param name="category">The Category to retrieve</param>
        /// <returns>The AudioCategory object corresponding to that category.</returns>
        public AudioCategory GetCategory(String category)
        {
            return audioEngine.GetCategory(category);
        }
    }
}
