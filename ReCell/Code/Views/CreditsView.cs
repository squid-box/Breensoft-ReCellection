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

		// To decide whether or not we have finished showing the credits
		public bool Finished { get; private set; }

		// The set of credit strings
        private static List<String> credits;
		
		// The top and bottom credit string shown
		private string topString, bottomString;
		// The index of the top and bottom strings;
		private int topStringIndex, bottomStringIndex;

		//The y position of the top and bottom strings
		private float topStringY, bottomStringY;

		// The movement speed of the text
		private static readonly float textMovementSpeed;

		// The time to show each credit string
		private static int timeToShow;
		

        public CreditsView()
        {
            credits = new List<string>(9);
			credits.Add("Produced by:\nBreensoft");
            credits.Add("Project Leader:\nMartin Nycander");
            credits.Add("Lead Designer:\nJohn Forsberg");
            credits.Add("Dungeon Master:\nMattias Mikkola");
            credits.Add("Captain of Test fleet:\nLukas Mattsson");
            credits.Add("GUI Designer:\nCarl-Oscar Erneholm");
            credits.Add("Tracker of Eyes:\nViktor Eklund");
            credits.Add("Master of XNA:\nFredrik Lindh");
            credits.Add("Chief Programmer:\nMarco Ahumada Juntunen");
            credits.Add("Guy that did stuff, sometimes:\n Joel Ahlgren");
			credits.Add("Developers: GOTO 01");

			Finished = false;

        }
		public override void Update(GameTime passedTime)
		{
			// Stop doing shit if we have finished
			if (Finished)
				return;

			int passed = passedTime.ElapsedRealTime.Milliseconds;

			
		}
		
		public override void Draw(SpriteBatch sb)
		{
			sb.GraphicsDevice.Clear(Color.Black);
            //DrawCenteredString(sb, s, new Vector2(Globals.VIEWPORT_WIDTH / 2, 10 + offset*(Recellection.viewPort.Height / credits.Count) + Recellection.screenFont.MeasureString(" ").Y), Color.White);
         }	
	}
}
