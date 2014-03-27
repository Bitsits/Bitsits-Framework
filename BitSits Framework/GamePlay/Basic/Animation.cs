using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BitSits_Framework
{
    /// <summary>
    /// Represents an animated texture.
    /// </summary>
    /// <remarks>
    /// Currently, this class assumes that each frame of animation is
    /// as wide as each animation is tall. The number of frames in the
    /// animation are inferred from this.
    /// </remarks>
    class Animation
    {
        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        /// <summary>
        /// When the end of the animation is reached, should it
        /// continue playing from the beginning?
        /// </summary>
        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        /// <summary>
        /// Gets the number of frames in the animation.
        /// </summary>
        public int FrameCount
        {
            get { return frameCount; }
        }
        int frameCount;

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth
        {
            get { return Texture.Width / FrameCount; }
        }

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        /// <summary>
        /// Gets the Origin of a frame in the animation.
        /// </summary>
        public Vector2 OriginFactor
        {
            get { return originFactor; }
        }
        Vector2 originFactor;


        /// <summary>
        /// Constructors a new animation. when bottom-center is origin & Square Frame
        /// </summary>        
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;

            // Assume square frames.
            this.frameCount = texture.Width / texture.Height;

            // Gets a texture origin at the bottom center of each frame.
            this.originFactor = new Vector2(0.5f, 1);
        }

        /// <summary>
        /// Constructors a new animation.
        /// </summary>
        public Animation(Texture2D texture, int frameCount, float frameTime, bool isLooping,
            Vector2 originFactor)
            : this(texture, frameTime, isLooping)
        {
            this.frameCount = frameCount;
            this.originFactor = originFactor;
        }
    }
}
