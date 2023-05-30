using Microsoft.Xna.Framework;

namespace Doom;

public class Seg
{
    public short StartVertexId { get; init; }
    public short EndVertexId { get; init; }
    public short Angle { get; set; }
    public short LinedefId { get; init; }
    public short Direction { get; init; }
    public short Offset { get; init; }
    public Vector2 StartVertex { get; set; }
    public Vector2 EndVertex { get; set; }
    public Linedef Linedef { get; set; }
    public Sector FrontSector { get; set; }
    public Sector BackSector { get; set; }
}