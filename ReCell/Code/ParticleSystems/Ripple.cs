using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

/*
 * Instantiates a particle that could be any color (although only red [for blood] and gray [for shrapnel] is used in this game)
 * Project Written by : Fredrik Lindh (Temaran)
 * Last Update : 2010-04-09
 */

namespace Recellection
{
    class Ripple
    {
        public Vector2 position;
        public float maxDistortion, distortion;
        public float waveInitDivider, waveDivider;
        public float disappearRate;
        public float movementRate;

        public Ripple(Vector2 p, float dist, float waveDiv, float dRate, float mRate)
        {
            position = p;

            maxDistortion = dist;
            distortion = dist;

            waveInitDivider = waveDiv;
            waveDivider = waveDiv;

            disappearRate = dRate;
            movementRate = mRate;
        }
    }
}
