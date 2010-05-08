
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

	public sealed class MenuView : IView
	{
		/// <summary>
		/// author: co
		/// </summary>
	
		private SpriteBatch textDrawer = new SpriteBatch(Recellection.graphics.GraphicsDevice);
        //private RenderTarget2D textRenderTex = new RenderTarget2D(Recellection.graphics.GraphicsDevice, Recellection.viewPort.Width, Recellection.viewPort.Height, 0, Recellection.graphics.GraphicsDevice.DisplayMode.Format);
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
			
            //Recellection.graphics.GraphicsDevice.SetRenderTarget(0, textRenderTex);
            //Recellection.graphics.GraphicsDevice.Clear(Color.White);
			/*
            foreach (MenuIcon mi in currentMenu.GetIcons())
            {
                if (mi.texture != null)
                {
                    graphics.Add(mi.getIconPic());
                }
                else if(mi.getLabel() != null)
                {
                    //TODO Do this!
                }
            }*/
			/*//graphics.Add();//TODO skriv ut text SEN inte nu, laga menu
			Vector2 position = new Vector2(20, 20);
			Vector2 scale = new Vector2(1.0f, 1.0f);

			if(ev.type == EventType.ADD)
			{
				
			}
			else if (ev.type == EventType.REMOVE)
			{
				
			}*/
		}

		override public void Update(GameTime passedTime)
		{
		}
		
		override public void Draw(SpriteBatch spriteBatch)
		{
			this.drawTexture(spriteBatch, currentMenu.getMenuPic(), new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height));
			
			foreach (MenuIcon mi in currentMenu.GetIcons())
			{
				if (mi.texture != null)
				{
					this.drawTexture(spriteBatch, mi.texture, mi.getIconPic().TargetRectangle);
				}
				else if (mi.getLabel() != null)
				{
					//TODO Do this!
				}
			}
		}
	}
}