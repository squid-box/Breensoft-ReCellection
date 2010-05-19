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
        private static List<String> credits;

        public CreditsView()
        {
            credits = new List<string>(9);
            credits.Add("Project Leader:\nMartin Nycander");
            credits.Add("Head Designer:\nJohn Forsberg");
            credits.Add("Dungeon Master:\nMattias Mikkola");
            credits.Add("Captain of Test fleet:\nLukas Mattsson");
            credits.Add("GUI Designer:\nCarl-Oscar Erneholm");
            credits.Add("Tracker of Eyes:\nViktor Eklund");
            credits.Add("Master of XNA:\nFredrik Lindh");
            credits.Add("Chief Programmer:\nMarco Ahumada Juntunen");
            credits.Add("Guy that did stuff, sometimes:\n Joel Ahlgren");

        }
		public override void Update(GameTime passedTime)
		{
		}
		
		public override void Draw(SpriteBatch sb)
		{
			sb.GraphicsDevice.Clear(Color.Black);
            int offset = 0;
            foreach (String s in credits)
            {
                DrawCenteredString(sb, s, new Vector2(Globals.VIEWPORT_WIDTH / 2, 10 + offset*(Recellection.viewPort.Height / credits.Count) + Recellection.screenFont.MeasureString(" ").Y), Color.White);
                offset++;
            }
			

		}	
	}
}
