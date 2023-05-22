using Microsoft.Xna.Framework;

namespace Doom;

public class Player
{
    public const int ThingIndex = 0;
    private readonly DoomEngine _engine;
    private readonly Thing _thing;
    private Vector2 _position;
    private ushort _angle;

    public Player(DoomEngine engine)
    {
        _engine = engine;
        _thing = _engine.WADData.Things[ThingIndex];
        _position = _thing.Position;
        _angle = _thing.Angle;
    }

    public Vector2 Position => _position;

    public void Update(GameTime gameTime)
    {

    }
}