namespace Doom;

public class Sidedef
{
    public short XOffset { get; init; }
    public short YOffset { get; init; }
    public string UpperTexture { get; init; }
    public string LowerTexture { get; init; }
    public string MiddleTexture { get; init; }
    public ushort SectorId { get; init; }
    public Sector Sector { get; set; }
}