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

		Logger logger;

		// To decide whether or not we have finished showing the credits
		public bool Finished { get; private set; }

		// The set of credit strings
        private static List<CreditItem> credits;
		
		// The movement speed of the text
		private static readonly float textMovementSpeed = 0.5f;	

        public CreditsView()
        {
			logger = LoggerFactory.GetLogger();
			logger.Active = true;

            credits = new List<CreditItem>();
			credits.Add(new CreditItem("Produced by:\nBreensoft"));
            credits.Add(new CreditItem("Project Leader:\nMartin Nycander"));
            credits.Add(new CreditItem("Lead Designer:\nJohn Forsberg"));
            credits.Add(new CreditItem("Dungeon Master:\nMattias Mikkola"));
            credits.Add(new CreditItem("Captain of Test fleet:\nLukas Mattsson"));
            credits.Add(new CreditItem("GUI Designer:\nCarl-Oscar Erneholm"));
            credits.Add(new CreditItem("Tracker of Eyes:\nViktor Eklund"));
            credits.Add(new CreditItem("Master of XNA:\nFredrik Lindh"));
            credits.Add(new CreditItem("Chief Programmer:\nMarco Ahumada Juntunen"));
            credits.Add(new CreditItem("Guy that did stuff, sometimes:\n Joel Ahlgren"));
			credits.Add(new CreditItem("Developers: GOTO 01"));

			credits.First().Visible = true;

			Finished = false;
        }
		public override void Update(GameTime passedTime)
		{
			// Stop doing shit if we have finished
			if (Finished)
				return;
			float passed = (float)passedTime.ElapsedRealTime.Milliseconds;
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
						
						c.YPosition = c.YPosition - passed * textMovementSpeed;
						Vector2 stringSize = Recellection.screenFont.MeasureString(c.Text);
						// If there is space for another 
						if (Globals.VIEWPORT_HEIGHT - c.YPosition >= stringSize.Y && c.YPosition >= 0)
						{
							setNextToVisible = true;
						}
						if (c.YPosition >= 0)
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
			float fontY = 300.0f;
			sb.DrawString(Recellection.worldFont, "Tjohej", new Vector2(fontX, fontY), Color.HotPink, 0, new Vector2(0f), 1.0f, SpriteEffects.None, Layer);

			lock (credits)
			{
				foreach (CreditItem c in credits)
				{
					if (c.Visible)
					{
						Vector2 stringSize = Recellection.screenFont.MeasureString(c.Text);
						fontX = (float)(Globals.VIEWPORT_WIDTH / 2) - stringSize.X / 2;

						sb.DrawString(Recellection.worldFont, c.Text, new Vector2(fontX, c.YPosition), Color.Red, 0, new Vector2(0f), 1.0f, SpriteEffects.None, Layer);
					}
				}
			}
			
         }	
	}
}
