using System;
using Microsoft.Xna.Framework;

namespace Doom;

public class WADData
{
    private readonly DoomEngine _engine;
    private readonly WADReader _reader;
    
    public WADData(DoomEngine engine, string mapName = "E1M1")
    {
        _engine = engine;
        _reader = new WADReader(engine.WADPath);

        MapName = mapName;
        MapIndex = GetLumpIndex(mapName);
        Vertexes = GetLumpData(_reader.ReadVertex, MapIndex + LumpIndices.VERTEXES, 4, 0);
        Linedefs = GetLumpData(_reader.ReadLinedef, MapIndex + LumpIndices.LINEDEFS, 14, 0);
        Nodes = GetLumpData(_reader.ReadNode, MapIndex + LumpIndices.NODES, 28, 0);
        SubSectors = GetLumpData(_reader.ReadSubSector, MapIndex + LumpIndices.SSECTORS, 4, 0);
        Segs = GetLumpData(_reader.ReadSeg, MapIndex + LumpIndices.SEGS, 12, 0);
        Things = GetLumpData(_reader.ReadThing, MapIndex + LumpIndices.THINGS, 10, 0);
        Sidedefs = GetLumpData(_reader.ReadSidedef, MapIndex + LumpIndices.SIDEDEFS, 30, 0);
        Sectors = GetLumpData(_reader.ReadSector, MapIndex + LumpIndices.SECTORS, 26, 0);

        _reader.Close();

        UpdateData();
    }

    public string MapName { get; init; }
    public int MapIndex { get; init; }
    public Vector2[] Vertexes { get; init; }
    public Linedef[] Linedefs { get; init; }
    public Node[] Nodes { get; init; }
    public SubSector[] SubSectors { get; init; }
    public Seg[] Segs { get; init; }
    public Thing[] Things { get; init; }
    public Sidedef[] Sidedefs { get; init; }
    public Sector[] Sectors { get; init; }

    private void UpdateData()
    {
        UpdateLinedefs();
        UpdateSidedefs();
        UpdateSegs();
    }

    private void UpdateLinedefs()
    {
        foreach (var line in Linedefs)
        {
            line.FrontSidedef = Sidedefs[line.FrontSidedefId];

            // 65535 or 0xFFFF represents undefined sidedef
            line.BackSidedef = line.BackSidedefId == ushort.MaxValue 
                ? null
                : Sidedefs[line.BackSidedefId];
        }
    }

    private void UpdateSidedefs()
    {
        foreach (var side in Sidedefs)
        {
            side.Sector = Sectors[side.SectorId];
        }
    } 

    private void UpdateSegs()
    {
        foreach (var seg in Segs)
        {
            seg.StartVertex = Vertexes[seg.StartVertexId];
            seg.EndVertex = Vertexes[seg.EndVertexId];
            seg.Linedef = Linedefs[seg.LinedefId];
            
            // 0 = front is frontside
            // 1 = back is frontside
            Sector frontSector;
            Sector backSector;
            if (seg.Direction == 0)
            {
                frontSector = seg.Linedef.FrontSidedef.Sector;
                backSector = seg.Linedef.BackSidedef?.Sector;
            }
            else
            {
                frontSector = seg.Linedef.BackSidedef.Sector;
                backSector = seg.Linedef.FrontSidedef?.Sector;
            }

            seg.FrontSector = frontSector;
            seg.BackSector = (LindefFlags.TwoSided & seg.Linedef.Flags) > 0
                ? backSector
                : null;

            var degs = FromBAMToDegrees(seg.Angle);
            seg.Angle = seg.Angle < 0 ? (short)(360 + degs) : (short)degs;
        }
    }

    public static float FromBAMToRadians(short bamAngle)
    {
        const float radiansPerBAM = MathHelper.TwoPi / 65536f;
        return radiansPerBAM * bamAngle;
    }

    public static float FromBAMToDegrees(short bamAngle) => 
        MathHelper.ToDegrees(FromBAMToRadians(bamAngle));

    private T[] GetLumpData<T>(Func<int, T> read, int lumpIndex, int numberOfBytes, int headerLength)
    {
        var lumpInfo = _reader.Directory[lumpIndex];
        var count = lumpInfo.LumpSize / numberOfBytes;
        var data = new T[count];
        for (var i = 0; i < count; i++)
        {
            var offset = lumpInfo.LumpOffset + i * numberOfBytes + headerLength;
            data[i] = read(offset);
        }
        return data;
    }

    private int GetLumpIndex(string lumpName)
    {
        for (var i = 0; i < _reader.Directory.Length; i++)
        {
            if (_reader.Directory[i].LumpName == lumpName)
            {
                return i;
            }
        }

        throw new Exception($"Lump {lumpName} not found");
    }
}