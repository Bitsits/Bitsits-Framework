using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace BitSits_Framework
{
    class Level : IDisposable
    {
        #region Fields

        public int Score { get; private set; }

        public bool IsLevelUp { get; private set; }
        public bool ReloadLevel { get; private set; }
        int levelIndex;

        GameContent gameContent;

        #endregion

        #region Initialization


        public Level(GameContent gameContent, int levelIndex)
        {
            this.gameContent = gameContent;
            this.levelIndex = levelIndex;
        }


        private void LoadTiles(int levelIndex)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            lines = gameContent.content.Load<List<string>>("Levels/level" + levelIndex.ToString("0"));

            width = lines[0].Length;
            // Loop over every tile position,
            for (int y = 0; y < lines.Count; ++y)
            {
                if (lines[y].Length != width)
                    throw new Exception(String.Format(
                        "The length of line {0} is different from all preceeding lines.", lines.Count));

                for (int x = 0; x < lines[0].Length; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    LoadTile(tileType, x, y);
                }
            }
        }


        private void LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                case '.': break;

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format(
                        "Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }


        private Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }


        public void Dispose() { }


        #endregion

        #region Update and HandleInput


        public void Update(GameTime gameTime)
        { }


        public void HandleInput(InputState input, int playerIndex)
        { }


        #endregion

        #region Draw


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        { }


        #endregion
    }
}
