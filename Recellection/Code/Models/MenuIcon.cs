using System.Linq;
using System.Text;
using Tobii.TecSDK.Client.Interaction.RegionImplementations;
using System;
using Recellection.Code.Models;
using Microsoft.Xna.Framework.Graphics;

namespace Recellection
{


	public class MenuIcon
	{
		
		private GUIRegion region;
		private String label;
		private Texture2D iconPic;
		
		public MenuIcon (String label, Texture2D iconPic)
		{
			region = null;
			this.label = label;
			this.iconPic = iconPic;
		}
		
		public String getLabel()
		{
			return label;
		}
		
		public Texture2D getIconPic()
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
