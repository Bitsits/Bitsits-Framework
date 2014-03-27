using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuEntry
    {
        #region Fields

        GameScreen screen;

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        public string Text;
        public float TextSize = 30, fontSize;
        SpriteFont font;

        Texture2D texture;

        Vector2 position;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        #endregion

        #region Properties

        public Rectangle GetMenuEntryHitBounds
        {
            get
            {
                if (texture == null)
                {
                    Vector2 textSize = font.MeasureString(Text) * TextSize / fontSize;
                    return new Rectangle((int)position.X, (int)position.Y, (int)textSize.X, (int)textSize.Y);
                }
                else
                    return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
            }
        }

        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text and location.
        /// </summary>
        public MenuEntry(MenuScreen screen, string text, Vector2 position)
        {
            this.screen = screen;
            this.Text = text;
            this.position = position;
            this.font = screen.ScreenManager.GameContent.gameFont;
            this.fontSize = screen.ScreenManager.GameContent.gameFontSize;
        }

        /// <summary>
        /// Constructs a new menu entry with the specified texture and location.
        /// </summary>
        public MenuEntry(MenuScreen screen, Texture2D texture, Vector2 position)
            : this(screen, string.Empty, position)
        {
            this.texture = texture;
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(bool isSelected, GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Gold : Color.Gray;

            // Pulsate the size of the selected menu entry.
            //double time = gameTime.TotalGameTime.TotalSeconds;
            //float pulsate = (float)Math.Sin(time * 6) + 1;
            //float scale = 1 + pulsate * 0.03f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.ScreenManager;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            if (texture == null)
                spriteBatch.DrawString(font, Text, position, color, 0, Vector2.Zero,
                    TextSize / fontSize, SpriteEffects.None, 1);
            else
                spriteBatch.Draw(texture, position, null, color, 0, Vector2.Zero,
                    1, SpriteEffects.None, 1);
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight()
        {
            return font.LineSpacing;
        }


        #endregion
    }
}
