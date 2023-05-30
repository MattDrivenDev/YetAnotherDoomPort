namespace Doom;

public class Linedef
{
    public ushort StartVertexId { get; init; }
    public ushort EndVertexId { get; init; }
    public ushort Flags { get; init; }
    public ushort LineType { get; init; }
    public ushort SectorTag { get; init; }
    public ushort FrontSidedefId { get; init; }
    public ushort BackSidedefId { get; init; }
    public Sidedef FrontSidedef { get; set; }
    public Sidedef BackSidedef { get; set; }
}