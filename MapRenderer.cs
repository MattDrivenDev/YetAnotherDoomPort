using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Doom;

public class MapRenderer
{
    private readonly DoomEngine _engine;
    private readonly WADData _wadData;
    private readonly Vector2[] _vertexes;
    private readonly (Vector2, Vector2) _mapBounds;

    public MapRenderer(DoomEngine engine)
    {
        _engine = engine;
        _wadData = engine.WADData;
        _mapBounds = GetMapBounds(_wadData.Vertexes);
        _vertexes = MapVertexes(_wadData.Vertexes);

        foreach (var vertex in _vertexes)
        {
            Console.WriteLine(vertex);
        }
    }

    private Vector2[] MapVertexes(Vector2[] vertexes)
    {
        return vertexes.Select(v => new Vector2(RemapX(v.X), RemapY(v.Y))).ToArray();
    }

    private float RemapX(float x)
    {        
        // Define the outer boundaries for the remapping
        var outerMinX = 30;
        var outerMaxX = Settings.Width - 30;

        // Get the minimum and maximum X values from the map bounds
        var minX = _mapBounds.Item1.X;
        var maxX = _mapBounds.Item2.X;

        // Clamp the input X value within the map bounds
        var clampedX = Math.Max(minX, Math.Min(x, maxX));

        // Scale the clamped X value
        var normalizedX = (clampedX - minX) / (maxX - minX);

        // Remap the normalized X value to the outer range
        var remappedX = normalizedX * (outerMaxX - outerMinX) + outerMinX;

        return remappedX;
    }

    private float RemapY(float y)
    {
        // Define the outer boundaries for the remapping
        var outerMinY = 30;
        var outerMaxY = Settings.Height - 30;

        // Get the minimum and maximum Y values from the map bounds
        var minY = _mapBounds.Item1.Y;
        var maxY = _mapBounds.Item2.Y;

        // Clamp the input Y value within the map bounds
        var clampedY = Math.Max(minY, Math.Min(y, maxY) - minY);  

        // Scale the clamped Y value
        var scaledY = clampedY * (outerMaxY - outerMinY) / (maxY - minY);  

        // Remap the scaled Y value to the outer boundaries
        var remappedY = Settings.Height - scaledY - outerMinY;  

        return remappedY;
    }

    private (Vector2, Vector2) GetMapBounds(Vector2[] vertexes)
    {
        var minX = vertexes.Min(v => v.X);
        var minY = vertexes.Min(v => v.Y);
        var lowerBounds = new Vector2(minX, minY);

        var maxX = vertexes.Max(v => v.X);
        var maxY = vertexes.Max(v => v.Y);
        var upperBounds = new Vector2(maxX, maxY);

        return (lowerBounds, upperBounds);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawScreenResolution(spriteBatch);
        DrawVertexes(spriteBatch);
    }

    private void DrawScreenResolution(SpriteBatch spriteBatch)
    {
        var thickness = 2;
        spriteBatch.DrawLine(new Vector2(0, 0), new Vector2(Settings.Width, 0), Color.White, thickness);
        spriteBatch.DrawLine(new Vector2(thickness, 0), new Vector2(thickness, Settings.Height), Color.White, thickness);
        spriteBatch.DrawLine(new Vector2(Settings.Width - thickness, 0), new Vector2(Settings.Width - thickness, Settings.Height), Color.White, thickness);
        spriteBatch.DrawLine(new Vector2(0, Settings.Height - thickness), new Vector2(Settings.Width, Settings.Height - thickness), Color.White, thickness);
    }

    private void DrawVertexes(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < _vertexes.Length; i++)
        {
            spriteBatch.DrawCircle(_vertexes[i], 2, Color.White);
        }
    }
}