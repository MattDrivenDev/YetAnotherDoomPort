namespace Doom;

public struct Linedef
{
    public Linedef(short startVertex, short endVertex, 
        short flags, short lineType, short sectorTag, 
        short frontSidedef, short backSidedef)
    {
        StartVertex = startVertex;
        EndVertex = endVertex;
        Flags = flags;
        LineType = lineType;
        SectorTag = sectorTag;
        FrontSidedef = frontSidedef;
        BackSidedef = backSidedef;
    }

    public short StartVertex { get; init; }
    public short EndVertex { get; init; }
    public short Flags { get; init; }
    public short LineType { get; init; }
    public short SectorTag { get; init; }
    public short FrontSidedef { get; init; }
    public short BackSidedef { get; init; }
}