using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

/* Handles sound effects and music in the game
 * 
 * Author: Fredrik Lindh
 * Date: 30/3/2010
 * 
 */

namespace Recellection
{
    class AudioPlayer
    {
        private AudioEngine engine;
        private WaveBank waves;
        private SoundBank sounds;
        private AudioCategory soundCategory;

        private float soundVolume;
        private Song[] songs;

		[System.Obsolete("NOOB!")]
        public AudioPlayer(ContentManager content)
        {
            engine = new AudioEngine("Content/Sounds/RecellectionSounds.xgs");
            waves = new WaveBank(engine, "Content/Sounds/Wave Bank.xwb");   
            sounds = new SoundBank(engine, "Content/Sounds/Sound Bank.xsb");
            soundCategory = engine.GetCategory("Default");

            soundVolume = 1.0f;
            songs = new Song[1];
            songs[0] = content.Load<Song>("Sounds/Songs/getdown");

            MediaPlayer.IsMuted = true;
        }

        public void Update()
        {
            engine.Update();
        }



        #region Sound methods

        public void PlaySound(String sound)
        {
            sounds.PlayCue(sound);
        }

        public void SetSoundVolume(float volume)
        {
            soundVolume = volume;
            soundCategory.SetVolume(soundVolume);
        }

        public float GetSoundVolume()
        {
            return soundVolume;
        }

        #endregion

        #region Music methods

        public void PlaySong(Globals.Songs song)
        {
            MediaPlayer.Play(songs[(int)song]);
        }

        public void ToggleMusicMute()
        {
            if (MediaPlayer.IsMuted)
                MediaPlayer.IsMuted = false;
            else
                MediaPlayer.IsMuted = true;
        }

        public void SetMusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }

        public float GetMusicVolume()
        {
            return MediaPlayer.Volume;
        }

        #endregion
    }
}
