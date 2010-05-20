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
            : base(owner, position)
        {
            base.MissionEntity = target;
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

        public override Texture2D GetSprite()
        {
            return Recellection.textureMap.GetTexture(Globals.TextureTypes.Kamikaze);
        }
    }
}
