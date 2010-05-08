using System.Linq;
using System.Text;
using Tobii.TecSDK.Client.Interaction.RegionImplementations;
using System;
using Recellection.Code.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Recellection
{


	public class MenuIcon
	{
		
		private GUIRegion region;
		private String label;
		private DrawData iconPic;
        public Texture2D texture { get; private set; }

        public MenuIcon(String label, Texture2D texture)
		{
			region = null;
			this.label = label;
			this.texture = texture;
		}
		
		public String getLabel()
		{
			return label;
		}
		
		public DrawData getIconPic()
		{
			return iconPic;
		}
		
		public GUIRegion getRegion()
		{
			return region;
		}
		
		public void setRegion(GUIRegion region)
		{
			this.region = region;
		}

        public void setRectangle(Rectangle rectangle)
        {
            iconPic = new DrawData(texture, rectangle);
        }
		
//		public void setPosition(int x1, int y1, int x2, int y2)
//		{
//			
//		}
		
//		public Rect getPosition()
//		{
//			return region.BoundingGeometry()
//		}
	}
}
