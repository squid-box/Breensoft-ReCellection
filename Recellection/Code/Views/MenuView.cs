
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection
{

	public class MenuView : IDrawable
	{
		/// <summary>
		/// author: co
		/// </summary>
	
		private SpriteBatch textDrawer = new SpriteBatch(Recellection.graphics.GraphicsDevice);
        private RenderTarget2D textRenderTex = new RenderTarget2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width, Recellection.viewPort.Height, 0, Recellection.graphics.GraphicsDevice.DisplayMode.Format);
        private float fontSzInPx = 14;
		List<DrawData> graphics;
		
		public MenuView()
		{
			MenuModel.Instance.MenuEvent += menuEventFunction;
			graphics = new List<DrawData>();
		}
		
		public void menuEventFunction(Object publisher, Event ev)
		{
			Menu m = ev.subject;
			graphics.Clear();
			graphics.Add(new DrawData(new Vector2D(0,0), m.getMenuPic(), 0, 0, Recellection.viewPort.Width));
			//graphics.Add();//TODO skriv ut text SEN inte nu, laga menu
			textDrawer.DrawString(Recellection.screenFont, Language.Instance.GetString("MainMenu1"), offset, Color.Black, 0, Vector2.Zero, textScale, SpriteEffects.None, 0);

			if(ev.type == EventType.ADD)
			{
				Menu m = ev.subject;
			}else if (ev.type == EventType.REMOVE)
			{
				
			}
		}
		
		
		List<DrawData> GetDrawData()
		{
			return graphics;
		}
	}
}