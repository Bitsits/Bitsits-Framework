using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BitSits_Framework
{
    /// <summary>
    /// All the Contents of the Game is loaded and stored here
    /// so that all other screen can copy from here
    /// </summary>
    public class GameContent
    {
        public ContentManager content;
        public Vector2 viewportSize;
        
        public Random random = new Random();

        // Textures
        public Texture2D blank, gradient;
        public Texture2D menuBackground, mainMenuTitle;

        // Fonts
        public SpriteFont debugFont, gameFont;
        public int gameFontSize;

        // Audio objects
        public AudioEngine audioEngine;
        public SoundBank soundBank;
        public WaveBank waveBank;
        

        /// <summary>
        /// Load GameContents
        /// </summary>
        public GameContent(GameComponent screenManager)
        {
            content = screenManager.Game.Content;
            Viewport viewport = screenManager.Game.GraphicsDevice.Viewport;
            viewportSize = new Vector2(viewport.Width, viewport.Height);

            blank = content.Load<Texture2D>("Graphics/blank");
            gradient = content.Load<Texture2D>("Graphics/gradient");
            menuBackground = content.Load<Texture2D>("Graphics/menuBackground");

            mainMenuTitle = content.Load<Texture2D>("Graphics/mainMenuTitle");

            debugFont = content.Load<SpriteFont>("Fonts/debugFont");

            gameFontSize = 60;
            gameFont = content.Load<SpriteFont>("Fonts/chunky" + gameFontSize.ToString());

            //MediaPlayer.Volume = 1; MediaPlayer.IsRepeating = true;
            //SoundEffect.MasterVolume = 1;

            // Initialize audio objects.
            //audioEngine = new AudioEngine("Content/Audio/Audio.xgs");
            //soundBank = new SoundBank(audioEngine, "Content/Audio/Sound Bank.xsb");
            //waveBank = new WaveBank(audioEngine, "Content/Audio/Wave Bank.xwb");

            //soundBank.GetCue("music").Play();


            //Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            screenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload GameContents
        /// </summary>
        public void UnloadContent() { content.Unload(); }
    }
}
