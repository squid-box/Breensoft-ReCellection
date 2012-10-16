namespace Recellection.Code.Views
{
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    using global::Recellection.Code.Utility.Logger;

    public abstract class IView
	{
        #region Static Fields

        private static readonly Texture2D pixel = Recellection.textureMap.GetTexture(Globals.TextureTypes.Pixel);

        #endregion

        #region Public Properties

        public float Layer { get; set; }

        #endregion

        #region Public Methods and Operators

        public abstract void Draw(SpriteBatch spriteBatch);

        public void DrawCenteredString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            position.Y -= Recellection.screenFont.MeasureString(text).Y / 2;

            // Split up multiliners
            char[] splits = { '\n' };
            foreach (string line in text.Split(splits))
            {
                // Measure text
                var origin = new Vector2(Recellection.screenFont.MeasureString(line).X / 2, 0);

                spriteBatch.DrawString(
                    Recellection.screenFont, line, position, color, 0, origin, 1.0f, SpriteEffects.None, this.Layer);

                position.Y += Recellection.screenFont.MeasureString(line == string.Empty ? " " : line).Y;
            }
        }

        // Draws a line with specified thickness 
		public void DrawLine(SpriteBatch batch, Vector2 start, Vector2 end, Color color, int thickness)
		{
		    // Calculate distance between both points 
		    var distance = (int)Vector2.Distance(start, end);

		    // Calculate angle between both points 
		    var angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

		    LoggerFactory.GetLogger("Recellection.Code.Views.WorldView").Info(
		        "Drawing line between " + start + " and " + end + ".");

		    var modifier = new Vector2(0, thickness / 2);

		    // Draw line 
		    batch.Draw(
		        pixel, 
		        new Rectangle((int)start.X, (int)start.Y, distance, thickness), 
		        null, 
		        color, 
		        angle, 
		        new Vector2(0, 1), 
		        SpriteEffects.None, 
		        this.Layer);
		}

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(Recellection.screenFont, text, position, color, 0, new Vector2(), 1.0f, SpriteEffects.None, this.Layer);
        }

        public void DrawTexture(SpriteBatch spriteBatch, Texture2D t, Rectangle targetArea)
        {
            spriteBatch.Draw(t, targetArea, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, this.Layer);
        }

        public void DrawTexture(SpriteBatch spriteBatch, Texture2D t, Rectangle targetArea, Color color)
        {
            spriteBatch.Draw(t, targetArea, null, color, 0, new Vector2(0, 0), SpriteEffects.None, this.Layer);
        }

        public abstract void Update(GameTime passedTime);

        #endregion
	}
}
