
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Recellection.Code.Models;
using Recellection.Code.Utility.Events;
using Recellection.Code.Views;
using Microsoft.Xna.Framework.Content;

namespace Recellection
{

	public sealed class MenuView : IRenderable
	{
		/// <summary>
		/// author: co
		/// </summary>
	
		private SpriteBatch textDrawer = new SpriteBatch(Recellection.graphics.GraphicsDevice);
        private RenderTarget2D textRenderTex = new RenderTarget2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width, Recellection.viewPort.Height, 0, Recellection.graphics.GraphicsDevice.DisplayMode.Format);
        private float fontSzInPx = 14;
		List<DrawData> graphics;
        static readonly object padlock = new object();
        static MenuView instance = null;
        
        private Menu currentMenu = null;

        public static MenuView Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new MenuView();
                    }
                    return instance;
                }
            }
        }


		private MenuView()
		{
			MenuModel.Instance.MenuEvent += menuEventFunction;
			graphics = new List<DrawData>();
		}
		
		public void menuEventFunction(Object publisher, Event<Menu> ev)
		{
			currentMenu = ev.subject;
		/*
			Menu m = ev.subject;
			graphics.Clear();
			graphics.Add(new DrawData(new Vector2(0,0), m.getMenuPic(), 0, 0, Recellection.viewPort.Height, Recellection.viewPort.Width));
			//graphics.Add();//TODO skriv ut text SEN inte nu, laga menu
			Vector2 position = new Vector2(20, 20);
			Vector2 scale = new Vector2(1.0f, 1.0f);
			textDrawer.Begin();
			textDrawer.DrawString(Recellection.screenFont, "LOL", 
				position, Color.Black, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
			textDrawer.End();
			if(ev.type == EventType.ADD)
			{
				
			}
			else if (ev.type == EventType.REMOVE)
			{
				
			}*/
		}
		
		public List<DrawData> GetDrawData(ContentManager content)
		{
			graphics.Clear();
			graphics.Add(new DrawData(currentMenu.getMenuPic(), new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height)));
			return graphics;
		}
	}
}