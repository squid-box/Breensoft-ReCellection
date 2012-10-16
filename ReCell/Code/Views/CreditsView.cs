namespace Recellection.Code.Views
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    class CreditsView : IView
    {
        #region Static Fields

        private static readonly float textMovementSpeed = 1.25f;

        private static List<CreditItem> credits;

        #endregion

        #region Fields

        private readonly Texture2D back;

        private readonly Texture2D logo;

        private float fadeInTime = 1.5f;

        private byte opacity;

        #endregion

        // The set of credit strings
        #region Constructors and Destructors

        public CreditsView()
        {
            this.logo = Recellection.textureMap.GetTexture(Globals.TextureTypes.logo);
            this.back = Recellection.textureMap.GetTexture(Globals.TextureTypes.white);

            credits = new List<CreditItem>();
            credits.Add(new CreditItem("Produced by:\nBreensoft"));
            credits.Add(new CreditItem("\n"));
            credits.Add(new CreditItem("Bringer of Impalement:\nMartin Nycander"));
            credits.Add(new CreditItem("Lead Designer:\nJohn Forsberg"));
            credits.Add(new CreditItem("Dungeon Master:\nMattias Mikkola"));
            credits.Add(new CreditItem("Captain of Test fleet:\nLukas Mattsson"));
            credits.Add(new CreditItem("GUI Wizard:\nCarl-Oscar Erneholm"));
            credits.Add(new CreditItem("Tracker of Eyes:\nViktor Eklund"));
            credits.Add(new CreditItem("Wielder of XNA:\nFredrik Lindh"));
            credits.Add(new CreditItem("Overseer of button-pressing:\nMarco Ahumada Juntunen"));
            credits.Add(new CreditItem("Jack of all trades :\n Joel Ahlgren"));
            credits.Add(new CreditItem("\n"));
            credits.Add(
                new CreditItem("Music:\n Can't get hold of the guy but it's Sten something.\nOh, and he's Estonian"));
            credits.Add(new CreditItem("Alpha testers:\n Christoffer Hirsimaa\n Fredrik's girlfriend?"));
            credits.Add(new CreditItem("Thanks:\n Sten, wherever you are, for some nice music"));
            credits.Add(new CreditItem("\n"));
            credits.Add(
                new CreditItem(
                    "Special thanks:\n Tobii, for a great opportunity to have fun and learning and the same time"));

            credits.First().Visible = true;
            this.opacity = 0;

            this.Finished = false;
        }

        #endregion

        #region Public Properties

        public bool Finished { get; set; }

        // If we have finished scrolling text
        public bool FinishedScrolling { get; set; }

        #endregion

        #region Public Methods and Operators

        public override void Draw(SpriteBatch sb)
        {
            sb.GraphicsDevice.Clear(Color.Black);
            float fontX = Globals.VIEWPORT_WIDTH / 2;
            if (!this.FinishedScrolling)
            {
                lock (credits)
                {
                    foreach (CreditItem c in credits)
                    {
                        if (c.Visible)
                        {
                            Vector2 stringSize = Recellection.screenFont.MeasureString(c.Text);
                            fontX = (Globals.VIEWPORT_WIDTH / 2) - stringSize.X / 4;

                            sb.DrawString(
                                Recellection.worldFont, 
                                c.Text, 
                                new Vector2(fontX, c.YPosition), 
                                Color.White, 
                                0, 
                                new Vector2(0f), 
                                1.0f, 
                                SpriteEffects.None, 
                                this.Layer);
                        }
                    }
                }
            }
            else
            {
                this.Layer = 1.0f;
                this.DrawTexture(
                    sb, 
                    this.back, 
                    new Rectangle(0, 0, Recellection.viewPort.Width, Recellection.viewPort.Height), 
                    new Color(255, 255, 255, this.opacity));

                int x = Recellection.viewPort.Width / 2 - this.logo.Width / 2;
                int y = Recellection.viewPort.Height / 2 - this.logo.Height / 2;

                this.Layer = 0.0f;
                this.DrawTexture(
                    sb, 
                    this.logo, 
                    new Rectangle(x, y, this.logo.Width, this.logo.Height), 
                    new Color(255, 255, 255, this.opacity));
            }
        }

        public override void Update(GameTime passedTime)
        {
            // Stop doing shit if we have finished
            if (this.Finished)
            {
                return;
            }

            if (this.FinishedScrolling)
            {
                this.ShowLogo(passedTime);
                return;
            }

            this.ScrollCredits(passedTime);
        }

        #endregion

        #region Methods

        private void ScrollCredits(GameTime passedTime)
        {
            if (credits.Count == 0)
            {
                this.FinishedScrolling = true;
                return;
            }

            float passed = passedTime.ElapsedGameTime.Milliseconds;
            bool setNextToVisible = false;

            var trash = new List<CreditItem>();
            lock (credits)
            {
                foreach (CreditItem c in credits)
                {
                    if (setNextToVisible)
                    {
                        c.Visible = true;
                        setNextToVisible = false;
                    }

                    if (!c.Visible)
                    {
                        break;
                    }
                    else
                    {
                        c.YPosition = c.YPosition - (passed / 10 * textMovementSpeed);
                        Vector2 stringSize = Recellection.screenFont.MeasureString(c.Text);

                        // If there is space for another 
                        if (Globals.VIEWPORT_HEIGHT - c.YPosition >= stringSize.Y && c.YPosition >= 0)
                        {
                            setNextToVisible = true;
                        }

                        if (c.YPosition <= -stringSize.Y)
                        {
                            trash.Add(c);
                        }
                    }
                }

                foreach (CreditItem c in trash)
                {
                    credits.Remove(c);
                }
            }
        }

        private void ShowLogo(GameTime passedTime)
        {
            if (this.opacity < 255)
            {
                this.opacity += (byte)((float)passedTime.ElapsedGameTime.TotalSeconds * (255f / this.fadeInTime));
            }
            else
            {
                this.Finished = true;
            }
        }

        #endregion

        private class CreditItem
        {
            #region Constructors and Destructors

            internal CreditItem(string text)
            {
                this.Text = text;
                this.YPosition = Globals.VIEWPORT_HEIGHT;
                this.Visible = false;
            }

            #endregion

            #region Properties

            internal string Text { get; set; }

            internal bool Visible { get; set; }

            internal float YPosition { get; set; }

            #endregion
        }
    }
}
