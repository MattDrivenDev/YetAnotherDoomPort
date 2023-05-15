using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

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
        Directory = ReadDirectory();
    }

    public WADHeader WADHeader { get; init; }
    public LumpInfo[] Directory { get; init; }

    private WADHeader ReadHeader()
    {
        var identification = ReadString(WADHeader.IdentificationPosition, WADHeader.IdentificationLength);
        var numLumps = ReadInt(WADHeader.NumLumpsPosition);
        var infoTableOffset = ReadInt(WADHeader.InfoTableOffsetPosition);

        return new WADHeader(identification, numLumps, infoTableOffset);
    }

    private LumpInfo[] ReadDirectory()
    {
        var directory = new LumpInfo[WADHeader.NumLumps];
        for (var i = 0; i < WADHeader.NumLumps; i++)
        {
            var offset = WADHeader.InfoTableOffset + i * 16;
            var lumpOffset = ReadInt(offset);
            var lumpSize = ReadInt(offset + 4);
            var lumpName = ReadString(offset + 8, 8);
            directory[i] = new LumpInfo(lumpOffset, lumpSize, lumpName);
        }
        return directory;
    }

    public Vector2 ReadVertex(int offset)
    {
        var x = ReadShort(offset);
        var y = ReadShort(offset + 2);
        return new Vector2(x, y);
    }

    public byte ReadByte(int offset)
    {
        var bytes = ReadBytes(offset, 1);
        return bytes[0];
    }

    public short ReadShort(int offset)
    {
        var bytes = ReadBytes(offset, 2);
        return BitConverter.ToInt16(bytes);
    }

    public int ReadInt(int offset)
    {
        var bytes = ReadBytes(offset, 4);
        return BitConverter.ToInt32(bytes);
    }

    public string ReadString(int offset, int count)
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