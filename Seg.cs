namespace Doom;

public struct Seg
{
    public Seg(short startVertex, short endVertex, 
        short angle, short linedef, short direction, 
        short offset)
    {
        StartVertex = startVertex;
        EndVertex = endVertex;
        Angle = angle;
        Linedef = linedef;
        Direction = direction;
        Offset = offset;
    }

    public short StartVertex { get; init; }
    public short EndVertex { get; init; }
    public short Angle { get; init; }
    public short Linedef { get; init; }
    public short Direction { get; init; }
    public short Offset { get; init; }
}