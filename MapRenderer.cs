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
        var outerMinX = 30;
        var outerMaxX = Settings.Width - 30;
        var minX = _mapBounds.Item1.X;
        var maxX = _mapBounds.Item2.X;
        return Math.Max(minX, Math.Min(x, maxX) - minX) * (outerMaxX - outerMinX) / (maxX - minX) + outerMinX;
    }

    private float RemapY(float y)
    {
        var outerMinY = 30;
        var outerMaxY = Settings.Height - 30;
        var minY = _mapBounds.Item1.Y;
        var maxY = _mapBounds.Item2.Y;
        return Settings.Height - (Math.Max(minY, Math.Min(y, maxY) - minY) * (outerMaxY - outerMinY) / (maxY - minY) + outerMinY);
    }

    private (Vector2, Vector2) GetMapBounds(Vector2[] vertexes)
    {
        var minX = vertexes.Min(v => v.X);
        var maxX = vertexes.Max(v => v.X);
        var minY = vertexes.Min(v => v.Y);
        var maxY = vertexes.Max(v => v.Y);
        return (new Vector2(minX, minY), new Vector2(maxX, maxY));
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawVertexes(spriteBatch);
    }

    private void DrawVertexes(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < _vertexes.Length; i++)
        {
            spriteBatch.DrawCircle(_vertexes[i], 2, Color.White);
        }
    }
}