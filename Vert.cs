namespace Doom;

public struct Vert
{
    public int X { get; init; }
    public int YTop { get; init; }
    public int YBottom { get; init; }
    public string Texture { get; init; }
    public ushort LightLevel { get; init; }
}