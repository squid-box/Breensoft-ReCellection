namespace Recellection.Code.Views
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Models;

    public class EndGameView : IView
	{
        #region Fields

        private readonly bool? gameWasWon;

        #endregion

        #region Constructors and Destructors

        public EndGameView(bool? gameWasWon)
		{
			this.gameWasWon = gameWasWon;
		}

        #endregion

        #region Public Methods and Operators

        public override void Draw(SpriteBatch sb)
		{
			sb.GraphicsDevice.Clear(Color.Black);
			
			if (this.gameWasWon == true)
			{
				this.DrawCenteredString(sb, Language.Instance.GetString("hasWon"), new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT / 2), Color.White);
			}
			else if (this.gameWasWon == false)
			{
				this.DrawCenteredString(sb, Language.Instance.GetString("hasLost"), new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT / 2), Color.White);
			}
			else
			{
				this.DrawCenteredString(sb, Language.Instance.GetString("isDraw"), new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT / 2), Color.White);
			}
			

			this.DrawCenteredString(sb, Language.Instance.GetString("BackToMain"), new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT - Recellection.screenFont.MeasureString(" ").Y), Color.HotPink);
		}

        public override void Update(GameTime passedTime)
        {
        }

        #endregion
	}
}
