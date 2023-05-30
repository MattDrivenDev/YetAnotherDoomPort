using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Doom;

public class BSP
{
    const ushort SubSectorIdentifier = short.MaxValue + 1;
    private readonly DoomEngine _engine;
    private readonly WADData _wadData;
    private readonly Player _player;
    private readonly Node[] _nodes;
    private readonly SubSector[] _subSectors;
    private readonly Seg[] _segs;
    private readonly ushort _rootNodeIndex;

    public BSP(DoomEngine engine)
    {
        _engine = engine;
        _wadData = _engine.WADData;
        _player = _engine.Player;
        _nodes = _wadData.Nodes;
        _subSectors = _wadData.SubSectors;
        _segs = _wadData.Segs;
        _rootNodeIndex = (ushort)(_nodes.Length - 1);
    }

    public int RootNodeIndex => _rootNodeIndex;
    public LinkedList<(Seg, int)> SegsToDraw { get; } = new();
    public LinkedList<(int, int, int)> VerticalLinesToDraw { get; } = new();

    public void Update(GameTime gameTime)
    {
        VerticalLinesToDraw.Clear();
        SegsToDraw.Clear();
        RenderBSPNode(_rootNodeIndex);
    }

    private int AngleToX(float angle)
    {
        float x;
        if (angle > 0)
        {
            x = Settings.ScreenDistance - MathF.Tan(MathHelper.ToRadians(angle)) * Settings.HalfWidth;
        } 
        else
        {
            x = -MathF.Tan(MathHelper.ToRadians(angle)) * Settings.HalfWidth + (int)Settings.ScreenDistance;
        }
        
        return (int)x;
    }

    private (int, int, float)? CanAddSegToFOV(Vector2 a, Vector2 b)
    {
        var angle1 = PointToAngle(a);
        var angle2 = PointToAngle(b);
        var span = NormalizeAngleInDegrees(angle1 - angle2);
        
        // Backface culling
        if (span >= 180f)
        {
            return null;
        }

        var angle1a = angle1;

        var playerAngle = _player.Angle;
        angle1 -= playerAngle;
        angle2 -= playerAngle;
        var span1 = NormalizeAngleInDegrees(angle1 + Settings.HalfFOV);
        var span2 = NormalizeAngleInDegrees(Settings.HalfFOV - angle2);

        if (span1 > Settings.FOV)
        {
            if (span1 >= span + Settings.FOV)
            {
                return null;
            }

            // Clipping
            angle1 = Settings.HalfFOV;
        }

        if (span2 > Settings.FOV)
        {
            if (span2 >= span + Settings.FOV)
            {
                return null;
            }

            // Clipping
            angle2 = -Settings.HalfFOV;
        }

        var x1 = AngleToX(angle1);
        var x2 = AngleToX(angle2);

        return (x1, x2, angle1a);
    }

    private void RenderSubSector(int subSectorId)
    {
        var subSector = _subSectors[subSectorId];
        var segCount = subSector.SegCount;

        for (var i = 0; i < segCount; i++)
        {
            var seg = _segs[subSector.FirstSeg  + i];
            var a = _wadData.Vertexes[seg.StartVertexId];
            var b = _wadData.Vertexes[seg.EndVertexId];

            var drawSegResult = CanAddSegToFOV(a, b);
            if (drawSegResult.HasValue)
            {
                var (x1, x2, angle) = drawSegResult.Value;
                SegsToDraw.AddLast((seg, subSectorId));
                VerticalLinesToDraw.AddLast((x1, x2, subSectorId));
            }
        }
    }

    private float NormalizeAngleInDegrees(float angle)
    {
        var maxDegrees = MathHelper.ToDegrees(MathHelper.TwoPi);
        angle %= maxDegrees;        
        return angle < 0 ? angle + maxDegrees : angle;
    }

    private bool CheckBBox(BoundingBox bBox)
    {
        var a = new Vector2(bBox.Left, bBox.Bottom);
        var b = new Vector2(bBox.Left, bBox.Top);
        var c = new Vector2(bBox.Right, bBox.Top);
        var d = new Vector2(bBox.Right, bBox.Bottom);

        var sides = new List<(Vector2, Vector2)>();
        var playerPosition = _player.Position;
        if (playerPosition.X < bBox.Left)
        {
            if (playerPosition.Y > bBox.Top)
            {
                sides.AddRange(new[] { (b, a), (c, b) });
            }
            else if (playerPosition.Y < bBox.Bottom)
            {
                sides.AddRange(new[] { (b, a), (a, d) });
            }
            else
            {
                sides.Add((b, a));
            }
        }
        else if (playerPosition.X > bBox.Right)
        {
            if (playerPosition.Y > bBox.Top)
            {
                sides.AddRange(new[] { (c, b), (d, c) });
            }
            else if (playerPosition.Y < bBox.Bottom)
            {
                sides.AddRange(new[] { (a, d), (d, c) });
            }
            else
            {
                sides.Add((d, c));
            }
        }
        else
        {
            if (playerPosition.Y > bBox.Top)
            {
                sides.Add((c, b));
            }
            else if (playerPosition.Y < bBox.Bottom)
            {
                sides.Add((a, d));
            }
            else
            {
                return true;
            }
        }

        foreach (var (v1, v2) in sides)
        {
            var angle1 = PointToAngle(v1);
            var angle2 = PointToAngle(v2);
            var span = NormalizeAngleInDegrees(angle1 - angle2);
            angle1 -= _player.Angle;
            var span1 = NormalizeAngleInDegrees(angle1 + Settings.HalfFOV);

            if (span1 > Settings.FOV)
            {
                if (span1 >= span + Settings.FOV)
                {
                    continue;
                }
            }

            return true;
        }

        return false;
    }

    private float PointToAngle(Vector2 point)
    {
        var delta = point - _player.Position;
        var angle = MathHelper.ToDegrees(MathF.Atan2(delta.Y, delta.X));
        return angle;        
    }

    private void RenderBSPNode(ushort nodeId)
    {
        if (nodeId >= SubSectorIdentifier)
        {
            var subSectorId = nodeId;
            subSectorId -= SubSectorIdentifier;
            RenderSubSector(subSectorId);
            return;
        }

        var node = _nodes[nodeId];

        if (IsPlayerOnBackSide(node))
        {
            RenderBSPNode(node.BackChild);
            if (CheckBBox(node.FrontBoundingBox))
            {
                RenderBSPNode(node.FrontChild);
            }
        }
        else
        {
            RenderBSPNode(node.FrontChild);
            if (CheckBBox(node.BackBoundingBox))
            {
                RenderBSPNode(node.BackChild);
            }
        }
    }

    private bool IsPlayerOnBackSide(Node node)
    {
        var deltaX = _player.Position.X - node.PartitionX;
        var deltaY = _player.Position.Y - node.PartitionY;
        return (deltaX * node.DeltaPartitionY) - (deltaY * node.DeltaPartitionX) <= 0;
    }
}