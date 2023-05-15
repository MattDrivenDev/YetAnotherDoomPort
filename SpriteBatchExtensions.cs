using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Doom;

public static class SpriteBatchExtensions
{
    private static Texture2D _pixel;

    private static void EnsurePixel(SpriteBatch spriteBatch)
    {
        if (_pixel == null)
        {
            _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] {Color.White});
        }
    }

    public static void DrawCircle(
        this SpriteBatch spriteBatch,
        Vector2 position, 
        int radius, 
        Color color)
    {
        EnsurePixel(spriteBatch);
        for (int y = -radius; y <= radius; y++)
        for (int x = -radius; x <= radius; x++)
        {
            if (x * x + y * y <= radius * radius)
            {
                spriteBatch.Draw(_pixel, position + new Vector2(x, y), color);
            }
        }
    }

    public static void DrawRectangle(
        this SpriteBatch spriteBatch, 
        Rectangle rectangle,
        Color color)
    {
        EnsurePixel(spriteBatch);
        spriteBatch.Draw(_pixel, rectangle, color);
    }
}