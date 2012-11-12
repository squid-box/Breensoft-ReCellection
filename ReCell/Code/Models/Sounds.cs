namespace Recellection.Code.Models
{
    using System;

    using Microsoft.Xna.Framework.Audio;

    /// <summary>
    /// The Sounds class is a Model that handles all sounds for the game.
    /// 
    /// author: Mattias Mikkola
    /// </summary>
    public sealed class Sounds : IModel
    {
        #region Static Fields

        private static AudioEngine audioEngine;
        private static SoundBank soundBank;
        private static WaveBank waveBank;

        #endregion

        #region Constructors and Destructors

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

        #endregion

        #region Public Properties

        public static Sounds Instance { get; private set; }

        public bool isMuted { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Return a specific audio category from the AudioEngine object.
        /// </summary>
        /// <param name="category">The Category to retrieve</param>
        /// <returns>The AudioCategory object corresponding to that category.</returns>
        public AudioCategory GetCategory(string category)
        {
            return audioEngine.GetCategory(category);
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

        #endregion
    }
}
