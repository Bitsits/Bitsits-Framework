using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    class QuickMenuScreen : GameScreen
    {
        public override void HandleInput(InputState input)
        {
            PlayerIndex playerIndex;
            if (input.IsMouseLeftButtonClick() || input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                LoadingScreen.Load(ScreenManager, false, PlayerIndex.One,
                    new GameplayScreen(), new PauseMenuScreen());

                ExitScreen();
            }
        }


        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 1 / 3);


            // Fade the popup alpha during transitions.
            Color color = new Color(Color.White, TransitionAlpha);

            spriteBatch.Begin();

            spriteBatch.Draw(ScreenManager.GameContent.menuBackground, Vector2.Zero, color);

            spriteBatch.End();
        }
    }
}
