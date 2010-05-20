using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Models;
namespace Recellection.Code.Views
{
    public class HelpView : IView
    {
        public HelpView()
        {
            
        }

        public override void Update(GameTime passedTime)
        {

        }

        public override void Draw(SpriteBatch sb)
        {
            sb.GraphicsDevice.Clear(Recellection.breen);

            Globals.TextureTypes type;

            if (Language.Instance.GetLanguage().Equals("English"))
            {
                type = Globals.TextureTypes.HelpViewEN;
            }
            else
            {
                type = Globals.TextureTypes.HelpViewSV;
            }

            DrawTexture(sb, Recellection.textureMap.GetTexture(type), new Rectangle(0,0,Globals.VIEWPORT_WIDTH,Globals.VIEWPORT_HEIGHT));

            DrawCenteredString(sb, "Go back to main menu.", new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT - Recellection.screenFont.MeasureString(" ").Y), Color.HotPink);
        }
    }
}
