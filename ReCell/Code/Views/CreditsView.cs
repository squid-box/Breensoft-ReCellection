using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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
		public bool Finished { get; private set; }

		// The set of credit strings
        private static List<CreditItem> credits;

		private CreditItem lastItem;
		
		// The movement speed of the text
		private static readonly float textMovementSpeed;	

        public CreditsView()
        {
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

			Finished = false;
			lastItem = null;

        }
		public override void Update(GameTime passedTime)
		{
			// Stop doing shit if we have finished
			if (Finished)
				return;
			float passed = (float)passedTime.ElapsedRealTime.Milliseconds;
			CreditItem temp;
			foreach(CreditItem c in credits)
			{
				
				if (lastItem == null)
					lastItem = c;
				if (c.Visible)
				{
					c.YPosition = c.YPosition - passed * textMovementSpeed;

				}
				temp = c;
			}
		}
		
		public override void Draw(SpriteBatch sb)
		{
			sb.GraphicsDevice.Clear(Color.Black);
            //DrawCenteredString(sb, s, new Vector2(Globals.VIEWPORT_WIDTH / 2, 10 + offset*(Recellection.viewPort.Height / credits.Count) + Recellection.screenFont.MeasureString(" ").Y), Color.White);
         }	
	}
}
