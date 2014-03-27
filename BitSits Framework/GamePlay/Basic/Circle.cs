using System;
using Microsoft.Xna.Framework;

namespace BitSits_Framework
{
    /// <summary>
    /// Represents a 2D circle.
    /// </summary>
    struct Circle
    {
        /// <summary>
        /// Center position of the circle.
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// Radius of the circle.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Constructs a new circle.
        /// </summary>
        public Circle(Vector2 position, float radius)
        {
            Center = position;
            Radius = radius;
        }

        /// <summary>
        /// Determines if a circle intersects a rectangle.
        /// </summary>
        /// <returns>True if the circle and rectangle overlap. False otherwise.</returns>
        public bool Intersects(Rectangle rectangle)
        {
            Vector2 v = new Vector2(MathHelper.Clamp(Center.X, rectangle.Left, rectangle.Right),
                                    MathHelper.Clamp(Center.Y, rectangle.Top, rectangle.Bottom));

            Vector2 direction = Center - v;
            float distanceSquared = direction.LengthSquared();

            return ((distanceSquared >= 0) && (distanceSquared < Radius * Radius));
        }

        /// <summary>
        /// Determines if a circle intersects a circle.
        /// </summary>
        /// <returns>True if the circle and circle overlap. False otherwise.</returns>
        public bool Intersects(Circle circle)
        {
            float dist = Vector2.Distance(Center, circle.Center);
            float minDist = Radius + circle.Radius;

            return dist <= minDist;
        }

        public Vector2 LinearDisplacement(Vector2 end, float displacement)
        {
            return LinearDisplacement(Center, end, displacement);
        }

        public static Vector2 LinearDisplacement(Vector2 start, Vector2 end, float displacement)
        {
            float a = start.X, b = start.Y, c = end.X, d = end.Y;

            float P = (d - b);
            float B = (c - a);
            float H = (float)Math.Sqrt(P * P + B * B);

            // cos = B / H  sin = P / H
            return start + displacement * new Vector2(B / H, P / H);
        }
    }
}
