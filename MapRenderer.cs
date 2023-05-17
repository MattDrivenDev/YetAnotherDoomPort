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
    private readonly Linedef[] _linedefs;
    private readonly (Vector2, Vector2) _mapBounds;
    private readonly SpriteFont _font;
    private readonly Player _player;
    private readonly Node[] _nodes;
    private readonly BSP _bsp;

    public MapRenderer(DoomEngine engine)
    {
        _engine = engine;
        _player = _engine.Player;
        _wadData = _engine.WADData;
        _mapBounds = GetMapBounds(_wadData.Vertexes);
        _vertexes = MapVertexes(_wadData.Vertexes);
        _linedefs = _wadData.Linedefs;
        _nodes = _wadData.Nodes;
        _bsp = _engine.BSP;
        _font = _engine.Content.Load<SpriteFont>("debug");

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
        DrawMapName(spriteBatch);
        // DrawVertexes(spriteBatch);
        DrawLinedefs(spriteBatch);
        DrawPlayer(spriteBatch);
        DrawNode(spriteBatch, _bsp.RootNode);
    }

    private void DrawBoundingBox(SpriteBatch spriteBatch, Node.BBox bbox, Color color)
    {
        var topLeft = new Vector2(RemapX(bbox.Left), RemapY(bbox.Top));
        var bottomRight = new Vector2(RemapX(bbox.Right), RemapY(bbox.Bottom));
        var rectangle = new Rectangle(
            (int)topLeft.X, 
            (int)topLeft.Y, 
            (int)(bottomRight.X - topLeft.X), 
            (int)(bottomRight.Y - topLeft.Y));
        spriteBatch.DrawEmptyRectangle(rectangle, color, 1);
    }

    private void DrawNode(SpriteBatch spriteBatch, Node node)
    {
        DrawBoundingBox(spriteBatch, node.FrontBoundingBox, Color.Green);
        DrawBoundingBox(spriteBatch, node.BackBoundingBox, Color.Red);

        var partitionStart = new Vector2(RemapX(node.PartitionX), RemapY(node.PartitionY));
        var partitionEnd = new Vector2(RemapX(node.PartitionX + node.DeltaPartitionX), RemapY(node.PartitionY + node.DeltaPartitionY));
        spriteBatch.DrawLine(partitionStart, partitionEnd, Color.Blue, 1);
    } 

    private void DrawPlayer(SpriteBatch spriteBatch)
    {
        var playerPosition = new Vector2(RemapX(_player.Position.X), RemapY(_player.Position.Y));
        spriteBatch.DrawCircle(playerPosition, 3, Color.DarkOrange);
    }

    private void DrawMapName(SpriteBatch spriteBatch)
    {
        var mapName = $"MAP: {_wadData.MapName}";
        var mapNameSize = _font.MeasureString(mapName);
        var mapNamePosition = new Vector2(Settings.Width - mapNameSize.X - 10, 10);
        spriteBatch.DrawString(_font, mapName, mapNamePosition, Color.White);
    }

    private void DrawScreenResolution(SpriteBatch spriteBatch)
    {
        var thickness = 2;

        // Draw the top border
        spriteBatch.DrawLine(new Vector2(0, 0), new Vector2(Settings.Width, 0), Color.White, thickness);

        // Draw the left border
        spriteBatch.DrawLine(new Vector2(thickness, 0), new Vector2(thickness, Settings.Height), Color.White, thickness);

        // Draw the right border
        spriteBatch.DrawLine(new Vector2(Settings.Width, 0), new Vector2(Settings.Width, Settings.Height), Color.White, thickness);
        
        // Draw the bottom border
        spriteBatch.DrawLine(new Vector2(0, Settings.Height - thickness), new Vector2(Settings.Width, Settings.Height - thickness), Color.White, thickness);
    }

    private void DrawVertexes(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < _vertexes.Length; i++)
        {
            spriteBatch.DrawCircle(_vertexes[i], 2, Color.White);
        }
    }

    private void DrawLinedefs(SpriteBatch spriteBatch)
    {
        for (var i = 0; i < _linedefs.Length; i++)
        {
            var linedef = _linedefs[i];
            var startVertex = _vertexes[linedef.StartVertex];
            var endVertex = _vertexes[linedef.EndVertex];
            spriteBatch.DrawLine(startVertex, endVertex, new Color(70 ,70 ,70), 1);
        }
    }
}