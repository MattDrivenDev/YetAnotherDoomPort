namespace Doom;

public class Sector
{
    public short FloorHeight { get; init; }
    public short CeilingHeight { get; init; }
    public string FloorTexture { get; init; }
    public string CeilingTexture { get; init; }
    public ushort LightLevel { get; init; }
    public ushort Type { get; init; }
    public ushort Tag { get; init; }
}