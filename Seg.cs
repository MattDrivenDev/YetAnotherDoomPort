using Microsoft.Xna.Framework;

namespace Doom;

public struct Seg
{
    public Seg(short startVertexId, short endVertexId, 
        short angle, short linedefId, short direction, 
        short offset)
    {
        StartVertexId = startVertexId;
        EndVertexId = endVertexId;
        Angle = angle;
        LinedefId = linedefId;
        Direction = direction;
        Offset = offset;
    }

    public short StartVertexId { get; init; }
    public short EndVertexId { get; init; }
    public short Angle { get; init; }
    public short LinedefId { get; init; }
    public short Direction { get; init; }
    public short Offset { get; init; }
}