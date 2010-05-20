using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Models;
namespace Recellection.Code.Views
{
	public class EndGameView : IView
	{
		private Nullable<bool> gameWasWon = null;
		
		public EndGameView(Nullable<bool> gameWasWon)
		{
			this.gameWasWon = gameWasWon;
		}
		
		public override void Update(GameTime passedTime)
		{
		}
		
		public override void Draw(SpriteBatch sb)
		{
			sb.GraphicsDevice.Clear(Color.Black);
			
			if (gameWasWon == true)
			{
				DrawCenteredString(sb, Language.Instance.GetString("hasWon"), new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT / 2), Color.White);
			}
			else if (gameWasWon == false)
			{
				DrawCenteredString(sb, Language.Instance.GetString("hasLost"), new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT / 2), Color.White);
			}
			else
			{
				DrawCenteredString(sb, Language.Instance.GetString("isDraw"), new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT / 2), Color.White);
			}
			

			DrawCenteredString(sb, "Go back to main menu.", new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT - Recellection.screenFont.MeasureString(" ").Y), Color.HotPink);
		}	
	}
}
