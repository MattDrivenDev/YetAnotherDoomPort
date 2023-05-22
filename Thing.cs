using Microsoft.Xna.Framework;

namespace Doom;

public class Thing
{
    public Thing(Vector2 position, ushort angle, 
        ushort type, ushort flags)
    {
        Position = position;
        Angle = angle;
        Type = type;
        Flags = flags;
    }

    public Vector2 Position { get; init; }
    public ushort Angle { get; init; }
    public ushort Type { get; init; }
    public ushort Flags { get; init; }
}