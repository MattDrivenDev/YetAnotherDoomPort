namespace Doom;

public struct LumpInfo
{
    public LumpInfo(int lumpOffset, int lumpSize, string lumpName)
    {
        LumpOffset = lumpOffset;
        LumpSize = lumpSize;
        LumpName = lumpName.Replace("\0", "");
    }

    public int LumpOffset { get; init; }
    public int LumpSize { get; init; }
    public string LumpName { get; init; }

    public override string ToString() => $"{nameof(LumpInfo)} [" +
        $"LumpOffset: {LumpOffset}, LumpSize: {LumpSize}, LumpName: {LumpName} ]";
}