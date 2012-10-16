namespace Recellection.Code.Views
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Models;

    public class HelpView : IView
    {
        #region Public Methods and Operators

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

            this.DrawTexture(sb, Recellection.textureMap.GetTexture(type), new Rectangle(0, 0, Globals.VIEWPORT_WIDTH, Globals.VIEWPORT_HEIGHT));

            this.DrawCenteredString(sb, "Go back to main menu.", new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT - Recellection.screenFont.MeasureString(" ").Y), Color.HotPink);
        }

        public override void Update(GameTime passedTime)
        {

        }

        #endregion
    }
}
