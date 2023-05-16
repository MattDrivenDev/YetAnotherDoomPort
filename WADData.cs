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

        foreach (var vertex in Vertexes)
        {
            Console.WriteLine(vertex);
        }

        _reader.Close();
    }

    public string MapName { get; init; }
    public int MapIndex { get; init; }
    public Vector2[] Vertexes { get; init; }
    public Linedef[] Linedefs { get; init; }
    public Node[] Nodes { get; init; }
    public SubSector[] SubSectors { get; init; }
    public Seg[] Segs { get; init; }
    public Thing[] Things { get; init; }

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