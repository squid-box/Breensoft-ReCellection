using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection.Code.Models
{
    public abstract class Entity
    {
        public Vector2 position { get; protected set; }
        public Player owner { get; protected set; }
        public int angle { get; protected set; }

        public Entity(Vector2 position, Player owner)
        {
            this.position = position;
            this.owner = owner;
            this.angle = 0;
        }

        /// <summary>
        /// Get current position of this Unit.
        /// </summary>
        /// <returns>X and Y coordinates for tile.</returns>
        public Vector2 GetPosition()
        {
            return this.position;
        }
        /// <summary>
        /// Magically teleport this Unit somewhere.
        /// </summary>
        /// <param name="newPos">X and Y coordinate of destination tile.</param>
        public void SetPosition(Vector2 newPos)
        {
            this.position = newPos;
        }

        public abstract Texture2D GetSprite();

        public int getAngle()
        {
            return this.angle;
        }

        public void setAngle(int a)
        {
            this.angle = a;
        }

        public Player GetOwner()
        {
            return this.owner;
        }

        public void SetOwner(Player owner)
        {
            this.owner = owner;
        }

    }
}
