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

    public Sidedef ReadSidedef(int offset) =>
        new Sidedef
        {
            XOffset = ReadShort(offset),
            YOffset = ReadShort(offset + 2),
            UpperTexture = ReadString(offset + 4, 8),
            LowerTexture = ReadString(offset + 12, 8),
            MiddleTexture = ReadString(offset + 20, 8),
            SectorId = ReadUShort(offset + 28)
        };

    public Sector ReadSector(int offset) =>
        new Sector
        {
            FloorHeight = ReadShort(offset),
            CeilingHeight = ReadShort(offset + 2),
            FloorTexture = ReadString(offset + 4, 8),
            CeilingTexture = ReadString(offset + 12, 8),
            LightLevel = ReadUShort(offset + 20),
            Type = ReadUShort(offset + 22),
            Tag = ReadUShort(offset + 24)
        };

    public Thing ReadThing(int offset) =>
        new Thing
        {
            Position = new Vector2(ReadShort(offset), ReadShort(offset + 2)),
            Angle = ReadUShort(offset + 4),
            Type = ReadUShort(offset + 6),
            Flags = ReadUShort(offset + 8)
        };

    public Seg ReadSeg(int offset) =>
        new Seg
        {
            StartVertexId = ReadShort(offset),
            EndVertexId = ReadShort(offset + 2),
            Angle = ReadShort(offset + 4),
            LinedefId = ReadShort(offset + 6),
            Direction = ReadShort(offset + 8),
            Offset = ReadShort(offset + 10)
        };

    public SubSector ReadSubSector(int offset) =>
        new SubSector
        {
            SegCount = ReadShort(offset),
            FirstSeg = ReadShort(offset + 2)
        };

    public Node ReadNode(int offset) =>
        new Node
        {
            PartitionX = ReadShort(offset),
            PartitionY = ReadShort(offset + 2),
            DeltaPartitionX = ReadShort(offset + 4),
            DeltaPartitionY = ReadShort(offset + 6),
            FrontBoundingBox = new BoundingBox
            {
                Top = ReadShort(offset + 8),
                Bottom = ReadShort(offset + 10),
                Left = ReadShort(offset + 12),
                Right = ReadShort(offset + 14)
            },
            BackBoundingBox = new BoundingBox
            {
                Top = ReadShort(offset + 16),
                Bottom = ReadShort(offset + 18),
                Left = ReadShort(offset + 20),
                Right = ReadShort(offset + 22)
            },
            FrontChild = ReadUShort(offset + 24),
            BackChild = ReadUShort(offset + 26)
        };

    public Linedef ReadLinedef(int offset) =>
        new Linedef
        {
            StartVertexId = ReadUShort(offset),
            EndVertexId = ReadUShort(offset + 2),
            Flags = ReadUShort(offset + 4),
            LineType = ReadUShort(offset + 6),
            SectorTag = ReadUShort(offset + 8),
            FrontSidedefId = ReadUShort(offset + 10),
            BackSidedefId = ReadUShort(offset + 12)
        };

    public Vector2 ReadVertex(int offset) =>
        new Vector2(ReadShort(offset), ReadShort(offset + 2));

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