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
        public KamikazeUnit(Player owner, Vector2 position, Entity target)
            : base(owner, position, target)
        {
            base.TargetEntity = target;
        }

        override protected bool stopMovingIfGoalIsReached()
        {

            // If we are reasonably close to target.
			float dx = this.position.X - this.targetPosition.X;
			float dy = this.position.Y - this.targetPosition.Y;
			double distance = Math.Sqrt(dx*dx + dy*dy);

            if (distance == 0)
            {
                if (TargetEntity != null)
                {
                    // If this is an enemy! KILL IT! OMG
                    if (TargetEntity.owner != this.owner)
                    {
                        if (TargetEntity is Unit && !((Unit)TargetEntity).isDead)
                        {
							UnitController.KillUnit(this);
							UnitController.KillUnit( ((Unit)TargetEntity));
                            SoundsController.playSound("Celldeath", this.position);
                        }
                    }

                    TargetEntity = null;
                }
            }
            if(TargetEntity == null || TargetEntity is Unit && ((Unit)TargetEntity).isDead)
			{
				UnitController.KillUnit(this);
                SoundsController.playSound("Celldeath", this.position);
                return true;
            }

            return false;
        }

        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.Kamikaze);
        }
    }
}
