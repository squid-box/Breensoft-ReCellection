/*
 * Instantiates a particle that could be any color (although only red [for blood] and gray [for shrapnel] is used in this game)
 * Project Written by : Fredrik Lindh (Temaran)
 * Last Update : 2010-04-09
 */

namespace Recellection
{
    using Microsoft.Xna.Framework;

    class Light
    {
        #region Fields

        public float TTL;

        public float curPresence; // whether it will swell or shrink during its lifetime, and how much/fast

        public float maxPresence; // whether it will swell or shrink during its lifetime, and how much/fast

        public float maxTTL;

        public float minPresence; // whether it will swell or shrink during its lifetime, and how much/fast

        public Vector2 position;

        public float presenceChangeSpeed; // whether it will swell or shrink during its lifetime, and how much/fast

        public float rotSpeed;

        public float rotation;

        public float xVelocity;

        public float yVelocity;

        #endregion

        #region Constructors and Destructors

        public Light(Vector2 p, float ttl, float yVel, float xVel, float currentPresence, float presenceVariance, float changeSpeed, float rot, float rotSpd)
        {
            this.position = p;
            this.maxTTL = ttl;
            this.TTL = ttl;
            this.xVelocity = xVel;
            this.yVelocity = yVel;

            this.curPresence = currentPresence;
            this.maxPresence = currentPresence + presenceVariance;
            this.minPresence = currentPresence - presenceVariance;
            this.presenceChangeSpeed = changeSpeed;

            this.rotation = rot;
            this.rotSpeed = rotSpd;
        }

        #endregion
    }
}
