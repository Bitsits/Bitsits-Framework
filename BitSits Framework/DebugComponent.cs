using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    class DebugComponent : DrawableGameComponent
    {
        public bool DebugMode = false;

        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont font;

        Point mousePos;

        KeyboardState prevKeyboardState;

        public DebugComponent(Game game)
            : base(game)
        {
            content = game.Content;
            DebugMode = true;
        }

        protected override void LoadContent()
        {
            font = content.Load<SpriteFont>("Fonts/DebugFont");
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            MouseState mouseState = Mouse.GetState();
            mousePos = new Point(mouseState.X, mouseState.Y);

            //if (keyboardState.IsKeyDown(Keys.F1) && prevKeyboardState.IsKeyUp(Keys.F1))
            //    DebugMode = !DebugMode;

            prevKeyboardState = keyboardState;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            float fps = (1000.0f / (float)gameTime.ElapsedRealTime.TotalMilliseconds);

            string text = "fps : " + fps.ToString("00") + "\n"
                + "X = " + mousePos.X + " Y = " + mousePos.Y;

            spriteBatch.DrawString(font, text, Vector2.Zero, Color.White);

            spriteBatch.End();
        }
    }
}
