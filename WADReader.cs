using System;
using System.IO;
using System.Text;

namespace Doom;

public class WADReader : IDisposable
{
    private readonly FileStream _fileStream;

    public WADReader(string wadPath)
    {
        if (!File.Exists(wadPath))
        {
            throw new FileNotFoundException("WAD file not found", wadPath);
        }

        _fileStream = new FileStream(wadPath, System.IO.FileMode.Open);
        WADHeader = ReadHeader();
        Console.WriteLine(WADHeader);
    }

    public WADHeader WADHeader { get; init; }

    private WADHeader ReadHeader()
    {
        var identification = ReadString(WADHeader.IdentificationPosition, WADHeader.IdentificationLength);
        var numLumps = ReadInt(WADHeader.NumLumpsPosition);
        var infoTableOffset = ReadInt(WADHeader.InfoTableOffsetPosition);

        return new WADHeader(identification, numLumps, infoTableOffset);
    }

    private int ReadInt(int offset)
    {
        var bytes = ReadBytes(offset, 4);
        return BitConverter.ToInt32(bytes);
    }

    private string ReadString(int offset, int count)
    {
        var bytes = ReadBytes(offset, count);
        return Encoding.ASCII.GetString(bytes);
    }

    private byte[] ReadBytes(int offset, int count)
    {
        var bytes = new byte[count];
        _fileStream.Seek(offset, SeekOrigin.Begin);
        _fileStream.Read(bytes, 0, count);
        return bytes;
    }

    public void Close()
    {
        _fileStream.Close();
    }

    public void Dispose()
    {
        Close();
    }
}