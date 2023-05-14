namespace Doom;

public class WADData
{
    private readonly DoomEngine _engine;
    private readonly WADReader _reader;

    public WADData(DoomEngine engine)
    {
        _engine = engine;
        _reader = new WADReader(engine.WADPath);
        _reader.Close();
    }
}