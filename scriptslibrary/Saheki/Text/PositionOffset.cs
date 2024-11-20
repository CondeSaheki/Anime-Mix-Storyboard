using OpenTK;
using StorybrewCommon.Storyboarding;

namespace Saheki
{
    public static partial class Text
    {
        /// <summary>
        /// Calculates the position offset for the given <paramref name="size"/> and <paramref name="origin"/>.
        /// </summary>
        /// <param name="size">The dimensions of the object.</param>
        /// <param name="origin">The origin of the object.</param>
        /// <returns>The position offset.</returns>
        /// <remarks>
        /// For horizontal origins, the x-coordinate is used to offset the object from the left edge of the screen.
        /// For vertical origins, the y-coordinate is used to offset the object from the top edge of the screen.
        /// </remarks>
        public static Vector2 PositionOffset(Vector2 size, OsbOrigin origin)
        {
            switch (origin)
            {
                case OsbOrigin.TopCentre:
                    return new Vector2(-size.X / 2, 0);
                case OsbOrigin.TopRight:
                    return new Vector2(-size.X, 0);
                case OsbOrigin.CentreLeft:
                    return new Vector2(0, -size.Y / 2);
                case OsbOrigin.Centre:
                    return new Vector2(-size.X / 2, -size.Y / 2);
                case OsbOrigin.CentreRight:
                    return new Vector2(-size.X, -size.Y / 2);
                case OsbOrigin.BottomLeft:
                    return new Vector2(0, -size.Y);
                case OsbOrigin.BottomCentre:
                    return new Vector2(-size.X / 2, -size.Y);
                case OsbOrigin.BottomRight:
                    return new Vector2(-size.X, -size.Y);
            }
            return Vector2.Zero;
        }
    }
}