using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Controllers;

namespace Recellection.Code.Models
{
    class KamikazeUnit : Unit
    {
        private static readonly float MOVEMENT_SPEED = 0.03f;

        public KamikazeUnit(Player owner, Vector2 position, Entity target)
            : base(owner, position)
        {
            base.MissionEntity = target;
            base.movement_speed = MOVEMENT_SPEED;
        }

        override protected bool stopMovingIfGoalIsReached()
        {

			// If we are reasonably close to baseEntity.
			float distance = float.MaxValue;
			Vector2 here = position;
			Vector2 there = targetPosition;

			Vector2.Distance(ref here, ref there, out distance);

            if (distance == 0.0f)
            {
                if (MissionEntity != null)
                {
                    // If this is an enemy! KILL IT! OMG
                    if (MissionEntity.owner != this.owner)
                    {
                        if (MissionEntity is Unit && !((Unit)MissionEntity).isDead)
						{
							this.Kill();
							((Unit)MissionEntity).Kill();
                            SoundsController.playSound("Celldeath", this.position);
                        }
                    }

                    MissionEntity = null;
                }
            }
            if(MissionEntity == null || MissionEntity is Unit && ((Unit)MissionEntity).isDead)
			{
				this.Kill();
                SoundsController.playSound("Celldeath", this.position);
                return true;
            }

            return false;
        }
/*
        /// <summary>
        /// Internal move logic. Uses targetPosition.
        /// </summary>
        override protected void Move(float deltaTime)
        {
            int beforeX = (int)this.position.X;
            int beforeY = (int)this.position.Y;

            Vector2 direction = Vector2.Subtract(this.targetPosition, this.position);
            direction.Normalize();

            // Move unit towards baseEntity.
            if (this.targetPosition.X != NO_TARGET)
            {
                float distance = this.targetPosition.X - this.position.X;

                if (Math.Abs(distance) < MOVEMENT_SPEED)
                {
                    position = new Vector2(targetPosition.X, position.Y);
                }
                else
                {
                    float newX = position.X + MOVEMENT_SPEED * deltaTime * direction.X * direction.Length();
                    position = new Vector2(newX, position.Y);
                }
            }
            if (this.targetPosition.Y != NO_TARGET)
            {
                float distance = this.targetPosition.Y - this.position.Y;

                if (Math.Abs(distance) < MOVEMENT_SPEED)
                {
                    position = new Vector2(position.X, targetPosition.Y);
                }
                else
                {
                    float newY = position.Y + MOVEMENT_SPEED * deltaTime * direction.Y * direction.Length();
                    position = new Vector2(position.X, newY);
                }
            }

            int afterX = (int)this.position.X;
            int afterY = (int)this.position.Y;

            // Tile management!
            if (afterX != beforeX || afterY != beforeY)
            {
                Unit.world.map.GetTile(beforeX, beforeY).RemoveUnit(this);
                Unit.world.map.GetTile(afterX, afterY).AddUnit(this);
            }
        }
        */
        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.Kamikaze);
        }
    }
}
