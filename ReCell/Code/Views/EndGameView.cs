using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
				DrawCenteredString(sb, "Congratulations!\n\n... but I'm afraid the princess is in another castle.", new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT / 2), Color.White);
			}
			else if (gameWasWon == false)
			{
				DrawCenteredString(sb, "You lost.\n\nBetter luck next time!", new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT / 2), Color.White);
			}
			else
			{
				DrawCenteredString(sb, "The game has ended.\nDid you win or lose?\n\n... I do not know.", new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT / 2), Color.White);
			}
			

			DrawCenteredString(sb, "Go back to main menu.", new Vector2(Globals.VIEWPORT_WIDTH / 2, Globals.VIEWPORT_HEIGHT - Recellection.screenFont.MeasureString(" ").Y), Color.HotPink);
		}	
	}
}
