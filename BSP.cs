using System.Linq;
using Microsoft.Xna.Framework;

namespace Doom;

public class BSP
{
    private readonly DoomEngine _engine;
    private readonly WADData _wadData;
    private readonly Player _player;
    private readonly Node[] _nodes;
    private readonly SubSector[] _subSectors;
    private readonly Seg[] _segs;
    private readonly Node _rootNode;

    public BSP(DoomEngine engine)
    {
        _engine = engine;
        _wadData = _engine.WADData;
        _player = _engine.Player;
        _nodes = _wadData.Nodes;
        _subSectors = _wadData.SubSectors;
        _segs = _wadData.Segs;
        _rootNode = _nodes.Last();
    }

    public Node RootNode => _rootNode;

    public void Update(GameTime gameTime)
    {
        
    }
}