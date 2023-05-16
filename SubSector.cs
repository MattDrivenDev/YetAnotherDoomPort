namespace Doom;

public struct SubSector
{
    public SubSector(short segCount, short firstSeg)
    {
        SegCount = segCount;
        FirstSeg = firstSeg;
    }

    public short SegCount { get; init; }
    public short FirstSeg { get; init; }
}