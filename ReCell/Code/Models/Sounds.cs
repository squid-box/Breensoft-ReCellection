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
    public sealed class Sounds : IModel
    {
        private static AudioEngine audioEngine;
        private static SoundBank soundBank;        
        private static WaveBank waveBank;

        public static Sounds Instance { get; private set; }

        static Sounds()
        {
            Instance = new Sounds();
        }

        private Sounds()
        {
            audioEngine = new AudioEngine("Content/Sounds/RecellectionSounds.xgs");
            soundBank = new SoundBank(audioEngine, "Content/Sounds/Sound Bank.xsb");
			waveBank = new WaveBank(audioEngine, "Content/Sounds/Wave Bank.xwb");
        }

        public Cue LoadSound(string sound)
        {
            return soundBank.GetCue(sound);
        }
    }
}
