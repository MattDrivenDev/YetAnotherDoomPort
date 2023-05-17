using System;
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

    public static void DrawLine(
        this SpriteBatch spriteBatch, 
        Vector2 start, 
        Vector2 end, 
        Color color, 
        Single thickness)
    {
        EnsurePixel(spriteBatch);

        var edge = end - start;
        var rotation = MathF.Atan2(edge.Y, edge.X);
        var lineScale = new Vector2(edge.Length(), thickness);

        spriteBatch.Draw(
            _pixel, 
            start, 
            null, 
            color, 
            rotation, 
            Vector2.Zero, 
            lineScale, 
            SpriteEffects.None, 
            0);
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

    public static void DrawEmptyRectangle(
        this SpriteBatch spriteBatch, 
        Rectangle rectangle,
        Color color,
        int thickness = 1)
    {
        EnsurePixel(spriteBatch);
        var topLeft = new Vector2(rectangle.X, rectangle.Y);
        var topRight = new Vector2(rectangle.X + rectangle.Width, rectangle.Y);
        var bottomLeft = new Vector2(rectangle.X, rectangle.Y + rectangle.Height);
        var bottomRight = new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
        spriteBatch.DrawLine(topLeft, topRight, color, thickness);
        spriteBatch.DrawLine(topRight, bottomRight, color, thickness);
        spriteBatch.DrawLine(bottomRight, bottomLeft, color, thickness);
        spriteBatch.DrawLine(bottomLeft, topLeft, color, thickness);
    }
}