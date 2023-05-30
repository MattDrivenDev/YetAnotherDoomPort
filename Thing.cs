using Microsoft.Xna.Framework;

namespace Doom;

public class Thing
{
    public Vector2 Position { get; init; }
    public ushort Angle { get; init; }
    public ushort Type { get; init; }
    public ushort Flags { get; init; }
}