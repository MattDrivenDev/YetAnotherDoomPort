namespace Doom;

public struct WADHeader
{
    public const int IdentificationPosition = 0;
    public const int IdentificationLength = 4;
    public const int NumLumpsPosition = 4;
    public const int InfoTableOffsetPosition = 8;

    public WADHeader(string identification, int numLumps, int infoTableOffset)
    {
        Identification = identification;
        NumLumps = numLumps;
        InfoTableOffset = infoTableOffset;
    }

    public string Identification { get; init; }
    public int NumLumps { get; init; }
    public int InfoTableOffset { get; init; }

    public override string ToString() => $"{nameof(WADHeader)} [" +
        $"Identification: {Identification}, NumLumps: {NumLumps}, InfoTableOffset: {InfoTableOffset} ]";
}