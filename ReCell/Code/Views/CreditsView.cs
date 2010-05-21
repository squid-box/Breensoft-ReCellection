using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Recellection.Code.Utility.Logger;

namespace Recellection.Code.Views
{
    class CreditsView : IView
    {
		private class CreditItem
		{

			internal string Text { get; set; }
			internal float YPosition { get; set; }
			internal bool Visible { get; set; }
			

			internal CreditItem(string text)
			{
				this.Text = text;
				this.YPosition = Globals.VIEWPORT_HEIGHT;
				this.Visible = false;
			}
		}

		// To decide whether or not we have finished showing the credits
		public bool Finished { get; set; }
		// If we have finished scrolling text
		public bool FinishedScrolling { get; set; }
		private Texture2D logo;
		private Texture2D back;
		private float fadeInTime = 1.5f;
		private byte opacity;

		// The set of credit strings
        private static List<CreditItem> credits;
		
		// The movement speed of the text
		private static readonly float textMovementSpeed = 1.25f;	

        public CreditsView()
        {

			logo = Recellection.textureMap.GetTexture(Globals.TextureTypes.logo);
			back = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);

            credits = new List<CreditItem>();
			credits.Add(new CreditItem("Produced by:\nBreensoft"));
			credits.Add(new CreditItem("\n"));
            credits.Add(new CreditItem("Asserter of Impalement:\nMartin Nycander"));
            credits.Add(new CreditItem("Lead Designer:\nJohn Forsberg"));
            credits.Add(new CreditItem("Dungeon Master:\nMattias Mikkola"));
            credits.Add(new CreditItem("Captain of Test fleet:\nLukas Mattsson"));
            credits.Add(new CreditItem("GUI Wizard:\nCarl-Oscar Erneholm"));
            credits.Add(new CreditItem("Tracker of Eyes:\nViktor Eklund"));
            credits.Add(new CreditItem("Wielder of XNA:\nFredrik Lindh"));
            credits.Add(new CreditItem("Overseer of button-pressing:\nMarco Ahumada Juntunen"));
            credits.Add(new CreditItem("Jack of all trades :\n Joel Ahlgren"));
			credits.Add(new CreditItem("\n"));
			credits.Add(new CreditItem("Music:\n Can't get hold of the guy but it's Sten something.\nOh, and he's Estonian"));
			credits.Add(new CreditItem("Alpha testers:\n Christoffer Hirsimaa\n Fredrik's girlfriend?"));
			credits.Add(new CreditItem("Thanks:\n Sten, wherever you are, for some nice music"));
			credits.Add(new CreditItem("\n"));
			credits.Add(new CreditItem("Special thanks:\n Tobii, for a great opportunity to have fun and learning and the same time"));



			credits.First().Visible = true;
			opacity = 0;

			Finished = false;
        }
		public override void Update(GameTime passedTime)
		{
			// Stop doing shit if we have finished
			
			if (Finished)
				return;
			if (FinishedScrolling)
			{
				ShowLogo(passedTime);
				return;
			}
			ScrollCredits(passedTime);
		}

		private void ShowLogo(GameTime passedTime)
		{
			if (opacity < 255)
			{
				opacity += (byte)((float)passedTime.ElapsedGameTime.TotalSeconds * (255f / fadeInTime));
			}
			else 
			{
				Finished = true;
			}
		}

		private void ScrollCredits(GameTime passedTime)
		{
			if (credits.Count == 0)
			{
				FinishedScrolling = true;
				return;
			}
			float passed = (float)passedTime.ElapsedGameTime.Milliseconds;
			bool setNextToVisible = false;

			List<CreditItem> trash = new List<CreditItem>();
			lock (credits)
			{
				foreach (CreditItem c in credits)
				{
					if (setNextToVisible)
					{
						c.Visible = true;
						setNextToVisible = false;
					}
					if (!c.Visible)
					{
						break;
					}
					else
					{
						c.YPosition = c.YPosition - (passed / 10 * textMovementSpeed);
						Vector2 stringSize = Recellection.screenFont.MeasureString(c.Text);
						// If there is space for another 
						if (Globals.VIEWPORT_HEIGHT - c.YPosition >= stringSize.Y && c.YPosition >= 0)
						{
							setNextToVisible = true;
						}
						if (c.YPosition <= -stringSize.Y)
						{
							trash.Add(c);
						}
					}
				}
				foreach (CreditItem c in trash)
				{
					credits.Remove(c);
				}
			}
		}
		
		public override void Draw(SpriteBatch sb)
		{
			sb.GraphicsDevice.Clear(Color.Black);
			float fontX = (float)(Globals.VIEWPORT_WIDTH / 2);
			if (!FinishedScrolling)
			{
				lock (credits)
				{
					foreach (CreditItem c in credits)
					{
						if (c.Visible)
						{
							Vector2 stringSize = Recellection.screenFont.MeasureString(c.Text);
							fontX = (float)(Globals.VIEWPORT_WIDTH / 2) - stringSize.X / 4;

							sb.DrawString(Recellection.worldFont, c.Text, new Vector2(fontX, c.YPosition), Color.White, 0, new Vector2(0f), 1.0f, SpriteEffects.None, Layer);
						}
					}
				}
			}
			else
			{
				this.Layer = 1.0f;
				DrawTexture(sb, back, new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height), new Color(255, 255, 255, opacity));

				int x = Recellection.viewPort.Width / 2 - logo.Width / 2;
				int y = Recellection.viewPort.Height / 2 - logo.Height / 2;

				this.Layer = 0.0f;
				DrawTexture(sb, logo, new Rectangle(x, y, logo.Width, logo.Height), new Color(255, 255, 255, opacity));
			}
			
         }	
	}
}
