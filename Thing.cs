using Microsoft.Xna.Framework;

namespace Doom;

public class Thing
{
    public Thing(Vector2 position, short angle, 
        short type, short flags)
    {
        Position = position;
        Angle = angle;
        Type = type;
        Flags = flags;
    }

    public Vector2 Position { get; init; }
    public short Angle { get; init; }
    public short Type { get; init; }
    public short Flags { get; init; }
}