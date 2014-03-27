using System;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;

namespace BitSits_Framework
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields

        GameContent gameContent;

        int prevScore;
        float score; 

        // Meta-level game state.
        private const int MaxLevelIndex = 1;    //Number of Levels
        private int levelIndex = 0;
        private Level level;
        bool load;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            this.gameContent = ScreenManager.GameContent;
            LoadNextLevel();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (load)
            {
                if (level.IsLevelUp)
                {
                    load = false;
                    MessageBoxScreen m = new MessageBoxScreen("LevelUp!");
                    m.Accepted += MessageBoxAccepted;
                    m.Cancelled += MessageBoxAccepted;
                    ScreenManager.AddScreen(m, null);
                }
                else if (level.ReloadLevel)
                {
                    load = false;
                    MessageBoxScreen m = new MessageBoxScreen("Reload Level!");
                    m.Accepted += MessageBoxAccepted;
                    m.Cancelled += MessageBoxAccepted;
                    ScreenManager.AddScreen(m, null);
                }
            }

            if (IsActive) level.Update(gameTime);
        }


        void MessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            if (level.IsLevelUp)
            {
                prevScore += level.Score; score = prevScore;
                LoadNextLevel();
            }
            else if (level.ReloadLevel) ReloadCurrentLevel();
        }


        private void LoadNextLevel()
        {
            if (levelIndex == MaxLevelIndex)
            {
                //LoadingScreen.Load(ScreenManager, false, null, new QuickMenuScreen());
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
                return;
            }

            // Unloads the content for the current level before loading the next one.
            if (level != null) level.Dispose();

            // Load the level.
            level = new Level(gameContent, levelIndex); ++levelIndex;
            load = true;

            //ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null) throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                level.HandleInput(input, playerIndex);
            }
        }


        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            level.Draw(gameTime, spriteBatch);

            DrawScore(gameTime, spriteBatch);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0) ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }

        private void DrawScore(GameTime gameTime, SpriteBatch spriteBatch)
        {
            score = Math.Min(score + (float)gameTime.ElapsedGameTime.TotalSeconds * 50, prevScore + level.Score);
        }


        #endregion
    }
}
