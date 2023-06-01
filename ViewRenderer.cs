using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Doom;

public class ViewRenderer
{
    private readonly DoomEngine _engine;
    private Dictionary<string, Color> _colors = new();

    public ViewRenderer(DoomEngine engine)
    {
        _engine = engine;
    }

    public LinkedList<(int, int, int)> VerticalLinesToDraw { get; } = new();
    public LinkedList<Vert> VertsToDraw { get; } = new();

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawVerticalLines(spriteBatch);
        DrawVerts(spriteBatch);
    }

    private void DrawVerticalLines(SpriteBatch spriteBatch)
    {
        while (VerticalLinesToDraw.Count > 0)
        {
            var (x1, x2, subSectorId) = VerticalLinesToDraw.First.Value;
            VerticalLinesToDraw.RemoveFirst();

            var color = GetRandomColor(subSectorId);

            var startX1 = new Vector2(x1, 0);
            var endX1 = new Vector2(x1, Settings.Height);
            spriteBatch.DrawLine(startX1, endX1, color, 1);

            var startX2 = new Vector2(x2, 0);
            var endX2 = new Vector2(x2, Settings.Height);
            spriteBatch.DrawLine(startX2, endX2, color, 1);     
        }
    }

    private Color GetRandomColor(int seed)
    {
        var random = new Random(seed);
        var r = random.Next(100, 255);
        var g = random.Next(100, 255);
        var b = random.Next(100, 255);
        return new Color(r, g, b);
    }

    private void DrawVerts(SpriteBatch spriteBatch)
    {
        while (VertsToDraw.Count > 0)
        {
            // We've already clipped the segs front to back, so
            // we can draw them in reverse order with painter's
            // algorithm... This is a bit of a hack, but it works
            // to hide some of the bugs in the top/bottom clipping.
            // Ideally, I'd go back and fix the clipping - and I
            // will completely (since I'd like to make it a little
            // more easy to reason about), but for now, this works.
            var vert = VertsToDraw.Last.Value;
            VertsToDraw.RemoveLast();

            if (vert.X < 0 || vert.X > Settings.Width)
                continue;

            var color = GetColor(vert.Texture, vert.LightLevel);

            var startX1 = new Vector2(vert.X, vert.YTop);
            var endX1 = new Vector2(vert.X, vert.YBottom);
            spriteBatch.DrawLine(startX1, endX1, color, 1);
        }
    }

    private Color GetColor(string texture, ushort lightLevel)
    {
        var key = texture + lightLevel;

        if (!_colors.ContainsKey(key))
        {
            var t = texture.GetHashCode();
            var l = lightLevel / 255f;
            var random = new Random(t);

            var r = (int)(random.Next(50, 255) * l);
            var g = (int)(random.Next(50, 255) * l);
            var b = (int)(random.Next(50, 255) * l);
            var color = new Color(r, g, b);

            _colors.Add(key, color);
        }

        return _colors[key];
    }
}