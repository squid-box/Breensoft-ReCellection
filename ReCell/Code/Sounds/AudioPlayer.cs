/* Handles sound effects and music in the game
 * 
 * Author: Fredrik Lindh
 * Date: 30/3/2010
 * 
 */

namespace Recellection
{
    using System;

    using Microsoft.Xna.Framework.Audio;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Media;

    class AudioPlayer
    {
        #region Fields

        private readonly AudioEngine engine;

        private readonly Song[] songs;

        private readonly SoundBank sounds;
        private AudioCategory soundCategory;

        private float soundVolume;

        private WaveBank waves;

        #endregion

        #region Constructors and Destructors

        [System.Obsolete("NOOB!")]
        public AudioPlayer(ContentManager content)
        {
            this.engine = new AudioEngine("Content/Sounds/RecellectionSounds.xgs");
            this.waves = new WaveBank(this.engine, "Content/Sounds/Wave Bank.xwb");   
            this.sounds = new SoundBank(this.engine, "Content/Sounds/Sound Bank.xsb");
            this.soundCategory = this.engine.GetCategory("Default");

            this.soundVolume = 1.0f;
            this.songs = new Song[1];
            this.songs[0] = content.Load<Song>("Sounds/Songs/Castlevania");

            MediaPlayer.IsMuted = true;
        }

        #endregion

        #region Public Methods and Operators

        public float GetMusicVolume()
        {
            return MediaPlayer.Volume;
        }

        public float GetSoundVolume()
        {
            return this.soundVolume;
        }

        public void PlaySong(Globals.Songs song)
        {
            MediaPlayer.Play(this.songs[(int)song]);
        }

        public void PlaySound(string sound)
        {
            this.sounds.PlayCue(sound);
        }

        public void SetMusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }

        public void SetSoundVolume(float volume)
        {
            this.soundVolume = volume;
            this.soundCategory.SetVolume(this.soundVolume);
        }

        public void ToggleMusicMute()
        {
            if (MediaPlayer.IsMuted)
                MediaPlayer.IsMuted = false;
            else
                MediaPlayer.IsMuted = true;
        }

        public void Update()
        {
            this.engine.Update();
        }

        #endregion
    }
}
