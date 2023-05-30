namespace Doom;

public class Node
{
    public short PartitionX { get; init; }
    public short PartitionY { get; init; }
    public short DeltaPartitionX { get; init; }
    public short DeltaPartitionY { get; init; }
    public BoundingBox FrontBoundingBox { get; init; }
    public BoundingBox BackBoundingBox { get; init; }
    public ushort FrontChild { get; init; }
    public ushort BackChild { get; init; }
}

public class BoundingBox
{
    public short Top { get; init; }
    public short Bottom { get; init; }
    public short Left { get; init; }
    public short Right { get; init; }
}