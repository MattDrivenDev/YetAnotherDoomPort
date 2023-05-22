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

    public void Update(GameTime gameTime)
    {
        SegsToDraw.Clear();
        RenderBSPNode(_rootNodeIndex);
    }

    private void RenderSubSector(int subSectorId)
    {
        var subSector = _subSectors[subSectorId];
        var segCount = subSector.SegCount;

        for (var i = 0; i < segCount; i++)
        {
            var seg = _segs[subSector.FirstSeg  + i];
            SegsToDraw.AddLast((seg, subSectorId));
        }
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
            RenderBSPNode(node.FrontChild);
        }
        else
        {
            RenderBSPNode(node.FrontChild);
            RenderBSPNode(node.BackChild);
        }
    }

    private bool IsPlayerOnBackSide(Node node)
    {
        var deltaX = _player.Position.X - node.PartitionX;
        var deltaY = _player.Position.Y - node.PartitionY;
        return (deltaX * node.DeltaPartitionY) - (deltaY * node.DeltaPartitionX) <= 0;
    }
}