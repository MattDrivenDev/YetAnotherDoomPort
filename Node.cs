namespace Doom;

public struct Node
{
    public Node(short partitionX, short partitionY, 
        short deltaPartitionX, short deltaPartitionY, 
        BBox frontBoundingBox, BBox backBoundingBox, 
        ushort frontChild, ushort backChild)
    {
        PartitionX = partitionX;
        PartitionY = partitionY;
        DeltaPartitionX = deltaPartitionX;
        DeltaPartitionY = deltaPartitionY;
        FrontBoundingBox = frontBoundingBox;
        BackBoundingBox = backBoundingBox;
        FrontChild = frontChild;
        BackChild = backChild;
    }

    public short PartitionX { get; init; }
    public short PartitionY { get; init; }
    public short DeltaPartitionX { get; init; }
    public short DeltaPartitionY { get; init; }
    public BBox FrontBoundingBox { get; init; }
    public BBox BackBoundingBox { get; init; }
    public ushort FrontChild { get; init; }
    public ushort BackChild { get; init; }

    public struct BBox
    {
        public BBox(short top, short bottom, short left, short right)
        {
            Top = top;
            Bottom = bottom;
            Left = left;
            Right = right;
        }

        public short Top { get; init; }
        public short Bottom { get; init; }
        public short Left { get; init; }
        public short Right { get; init; }
    }
}