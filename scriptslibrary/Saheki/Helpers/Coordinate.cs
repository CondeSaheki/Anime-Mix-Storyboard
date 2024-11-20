using OpenTK;

namespace Saheki
{
    public static partial class Helpers
    {
        /// <summary>
        /// The width of the screen in pixels.
        /// </summary>
        private const float ScreenWidth = 1920;

        /// <summary>
        /// The height of the screen in pixels.
        /// </summary>
        private const float ScreenHeight = 1080;

        /// <summary>
        /// The width of the screen in pixels.
        /// </summary>
        private const float OsuWidth = 640;

        /// <summary>
        /// The height of the screen in pixels.
        /// </summary>
        private const float OsuHeight = 480;

        /// <summary>
        /// Converts screen coordinates to osu! coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate on the screen.</param>
        /// <param name="y">The y-coordinate on the screen.</param>
        /// <returns>A Vector2 representing the osu! coordinates.</returns>
        public static Vector2 ScreenToOsu(float x, float y)
        {
            var xOsu = (OsuHeight * x - (OsuHeight / 2) * ScreenWidth + (OsuWidth / 2) * ScreenHeight) / ScreenHeight;
            var yOsu = y * OsuHeight / ScreenHeight;
            return new Vector2(xOsu, yOsu);
        }

        /// <summary>
        /// Converts screen coordinates to osu! coordinates.
        /// </summary>
        /// <param name="position">The position on the screen.</param>
        /// <returns>A Vector2 representing the osu! coordinates.</returns>
        public static Vector2 ScreenToOsu(Vector2 position)
        {
            return ScreenToOsu(position.X, position.Y);
        }

        /// <summary>
        /// Converts the given pixels to osu! pixels.
        /// </summary>
        /// <param name="pixels">The pixels to convert.</param>
        /// <param name="height">The height of the screen in pixels, default is 1080.</param>
        /// <returns>A pixels in osu! pixels.</returns>
        public static float PixelToOsu(int pixels)
        {
            return pixels * OsuHeight / ScreenHeight;
        }
        
        /// <summary>
        /// Converts the given normal pixels to osu! pixels.
        /// </summary>
        /// <param name="x">The x-coordinate in pixels.</param>
        /// <param name="y">The y-coordinate in pixels.</param>
        /// <returns>A Vector2 representing the osu! pixels.</returns>
        public static Vector2 PixelToOsu(int x, int y)
        {
            return new Vector2(PixelToOsu(x), PixelToOsu(y));
        }

        /// <summary>
        /// Converts the given size in normal pixels to osu! pixels.
        /// </summary>
        /// <param name="size">The size in pixels to convert.</param>
        /// <returns>The size in osu! pixels.</returns>
        public static Vector2 PixelToOsu(Vector2 size)
        {
            return PixelToOsu((int)size.X, (int)size.Y);
        }

        /// <summary>
        /// Converts osu! coordinates to screen coordinates.
        /// </summary>
        /// <param name="x">The x-coordinate in osu! coordinates.</param>
        /// <param name="y">The y-coordinate in osu! coordinates.</param>
        /// <returns>A Vector2 representing the screen coordinates.</returns>
        public static Vector2 OsuToScreen(float x, float y)
        {
            var xScreen = (x * ScreenHeight + (OsuHeight / 2) * ScreenWidth - (OsuWidth / 2) * ScreenHeight) / OsuHeight;
            var yScreen = y * ScreenHeight / OsuHeight;
            return new Vector2(xScreen, yScreen);
        }

        /// <summary>
        /// Converts osu! coordinates to screen coordinates.
        /// </summary>
        /// <param name="position">The position in osu! coordinates.</param>
        /// <returns>A Vector2 representing the screen coordinates.</returns>
        public static Vector2 OsuToScreen(Vector2 position)
        {
            return OsuToScreen(position.X, position.Y);
        }
    }
}