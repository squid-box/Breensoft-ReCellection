namespace Recellection.Code.Models
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public abstract class Entity
    {
        #region Constructors and Destructors

        public Entity(Vector2 position, Player owner)
        {
            this.position = position;
            this.owner = owner;
            this.angle = 0;
        }

        #endregion

        #region Public Properties

        public int angle { get; protected set; }

        public Player owner { get; protected set; }

        public Vector2 position { get; protected set; }

        #endregion

        #region Public Methods and Operators

        public Player GetOwner()
        {
            return this.owner;
        }

        /// <summary>
        /// Get current position of this Unit.
        /// </summary>
        /// <returns>X and Y coordinates for tile.</returns>
        public Vector2 GetPosition()
        {
            return this.position;
        }

        public abstract Texture2D GetSprite();

        public void SetOwner(Player owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Magically teleport this Unit somewhere.
        /// </summary>
        /// <param name="newPos">X and Y coordinate of destination tile.</param>
        public void SetPosition(Vector2 newPos)
        {
            this.position = newPos;
        }

        public int getAngle()
        {
            return this.angle;
        }

        public void setAngle(int a)
        {
            this.angle = a;
        }

        #endregion
    }
}
