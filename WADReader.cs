using System;
using System.IO;
using System.Text;
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

    public Thing ReadThing(int offset)
    {
        var x = ReadShort(offset);
        var y = ReadShort(offset + 2);

        return new Thing(
            position: new Vector2(x, y), 
            angle: ReadUShort(offset + 4), 
            type: ReadUShort(offset + 6), 
            flags: ReadUShort(offset + 8));
    }

    public Seg ReadSeg(int offset)
    {
        return new Seg
        {
            StartVertexId = ReadShort(offset),
            EndVertexId = ReadShort(offset + 2),
            Angle = ReadShort(offset + 4),
            LinedefId = ReadShort(offset + 6),
            Direction = ReadShort(offset + 8),
            Offset = ReadShort(offset + 10)
        };
    }

    public SubSector ReadSubSector(int offset)
    {
        var segCount = ReadShort(offset);
        var firstSeg = ReadShort(offset + 2);
        return new SubSector(segCount, firstSeg);
    }

    public Node ReadNode(int offset)
    {
        return new Node
        {
            PartitionX = ReadShort(offset),
            PartitionY = ReadShort(offset + 2),
            DeltaPartitionX = ReadShort(offset + 4),
            DeltaPartitionY = ReadShort(offset + 6),
            FrontBoundingBox = new Node.BBox
            {
                Top = ReadShort(offset + 8),
                Bottom = ReadShort(offset + 10),
                Left = ReadShort(offset + 12),
                Right = ReadShort(offset + 14)
            },
            BackBoundingBox = new Node.BBox
            {
                Top = ReadShort(offset + 16),
                Bottom = ReadShort(offset + 18),
                Left = ReadShort(offset + 20),
                Right = ReadShort(offset + 22)
            },
            FrontChild = ReadUShort(offset + 24),
            BackChild = ReadUShort(offset + 26)
        };
    }

    public Linedef ReadLinedef(int offset)
    {
        return new Linedef
        {
            StartVertex = ReadShort(offset),
            EndVertex = ReadShort(offset + 2),
            Flags = ReadShort(offset + 4),
            LineType = ReadShort(offset + 6),
            SectorTag = ReadShort(offset + 8),
            FrontSidedef = ReadShort(offset + 10),
            BackSidedef = ReadShort(offset + 12)
        };
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

    public ushort ReadUShort(int offset)
    {
        var bytes = ReadBytes(offset, 2);
        return BitConverter.ToUInt16(bytes);
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