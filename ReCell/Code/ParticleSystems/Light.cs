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
    class Light
    {
        public Vector2 position;
        public float maxTTL, TTL;
        public float yVelocity;
        public float xVelocity;
        public float curPresence, maxPresence, minPresence, presenceChangeSpeed; //whether it will swell or shrink during its lifetime, and how much/fast
        public float rotation;
        public float rotSpeed;

        public Light(Vector2 p, float ttl, float yVel, float xVel, float currentPresence, float presenceVariance, float changeSpeed, float rot, float rotSpd)
        {
            position = p;
            maxTTL = ttl;
            TTL = ttl;
            xVelocity = xVel;
            yVelocity = yVel;

            curPresence = currentPresence;
            maxPresence = currentPresence + presenceVariance;
            minPresence = currentPresence - presenceVariance;
            presenceChangeSpeed = changeSpeed;

            rotation = rot;
            rotSpeed = rotSpd;
        }
    }
}
