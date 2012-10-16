namespace Recellection.Code.Views
{
    using System.Collections.Generic;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class TestView : IView 
    {
        #region Fields

        int angle;

        int size = 512;

        int x = 250;
        int y = 250;

        #endregion

        #region Public Methods and Operators

        override public void Draw(SpriteBatch spriteBatch)
		{
		}
        
        [System.Obsolete("Use Draw instead!")]
        public List<DrawData> GetDrawData(ContentManager content)
        {
            var tex = content.Load<Texture2D>("Graphics/logo");
            var d = new DrawData(tex, new Rectangle(this.x, this.y, this.size, this.size), this.angle, 0);

            if(Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                this.x += 5;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                this.x -= 5;
            }

            if(Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                this.size += 16;
                if(this.size > 512)
                {
                    this.size = 64;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                this.angle += 1;
                if(this.angle >= 360)
                {
                    this.angle = this.angle - 360;
                }
            }
            
            var ret = new List<DrawData>();
            ret.Add(d);

            return ret;
        }

        override public void Update(GameTime passedTime)
        {
        }

        #endregion
    }
}
