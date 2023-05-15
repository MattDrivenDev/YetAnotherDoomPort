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

        foreach (var vertex in Vertexes)
        {
            Console.WriteLine(vertex);
        }

        _reader.Close();
    }

    public string MapName { get; init; }
    public int MapIndex { get; init; }
    public Vector2[] Vertexes { get; init; }

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