using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    class MessageBoxScreen : GameScreen
    {
        #region Fields


        const int hPad = 32;
        const int vPad = 16;

        string text;
        SpriteFont font;

        Texture2D texture;

        Vector2 origin;

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;


        #endregion

        #region Initialization


        public MessageBoxScreen(string message)
            : this(null, true)
        {
            this.text = message;
        }


        public MessageBoxScreen(Texture2D texture, bool isPopup)
        {
            this.texture = texture;

            IsPopup = isPopup;

            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
        }


        public override void LoadContent()
        {
            font = ScreenManager.GameContent.gameFont;

            if (texture != null)
                origin = new Vector2(texture.Width, texture.Height) / 2;
            else
                origin = font.MeasureString(text) / 2;
        }


        #endregion

        #region Handle Input and Draw


        public override void HandleInput(InputState input)
        {
            if (ScreenState != ScreenState.Active) return;

            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (input.IsMenuSelect(ControllingPlayer, out playerIndex)
                || input.IsMouseLeftButtonClick())
            {
                // Raise the accepted event, then exit the screen.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                // Raise the cancelled event, then exit the screen.
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 1 / 3);

            Vector2 pos = ScreenManager.GameContent.viewportSize / 2;

            // Fade the popup alpha during transitions.
            Color color = new Color(Color.White, TransitionAlpha);

            spriteBatch.Begin();

            if (texture != null)
                spriteBatch.Draw(texture, pos - origin, color);
            else
            {
                spriteBatch.Draw(ScreenManager.GameContent.gradient, new Rectangle((int)(pos - origin).X - hPad,
                  (int)(pos - origin).Y - vPad, ((int)origin.X + hPad) * 2, ((int)origin.Y + vPad) * 2), color);

                spriteBatch.DrawString(font, text, pos - origin, color);
            }

            spriteBatch.End();
        }


        #endregion
    }
}
