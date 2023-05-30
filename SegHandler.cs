using System.Linq;
using Microsoft.Xna.Framework;

namespace Doom;

public class SegHandler
{
    private readonly DoomEngine _engine;
    private readonly Player _player;
    private readonly ViewRenderer _renderer;

    public SegHandler(DoomEngine engine)
    {
        _engine = engine;
        _player = _engine.Player;
        _renderer = _engine.ViewRenderer;
    }

    public Seg CurrentSeg { get; private set; }
    public float CurrentRwAngle1 { get; private set; }
    public int[] ScreenRange { get; private set; }

    public void Update(GameTime gameTime)
    {
        InitializeScreenRange();
    }

    public void HandleSeg(Seg seg, int x1, int x2, float rwAngle1, int colorSeed)
    {
        CurrentSeg = seg;
        CurrentRwAngle1 = rwAngle1;

        // If the x-coordinates are the same, then the seg has 
        // 0 horizontal width and should not be drawn.
        if (x1 == x2)
        {
            return;
        }

        var backSector = seg.BackSector;
        var frontSector = seg.FrontSector;

        // If the seg is one-sided (front) then we know that
        // it is a solid wall.
        if (backSector == null)
        {
            ClipSolidWalls(x1, x2, colorSeed);
            return;
        }
    }

    private void ClipSolidWalls(int x1, int x2, int colorSeed = 0)
    {
        // If the horizontal range of the screen has
        // been exhausted, then we can return. In fact,
        // we should ALSO tell the BSP to stop traversing.
        if (ScreenRange.Length == 0)
        {
            _engine.BSP.CanTraverse = false;
            return;
        }

        // Something will have gone wrong if we have
        // this condition as true.
        if (x1 >= x2)
        {
            return;
        }

        var wall = new int[x2 - x1];
        for (var i = 0; i < wall.Length; i++)
        {
            wall[i] = x1 + i;
        }

        var intersection = ScreenRange.Intersect(wall).ToArray();
        
        // If the intersection is empty, then we can just ignore.
        if (intersection.Length == 0)
        {
            return;
        }

        // If the intersection is the same length as the wall, then
        // we can just draw the entire wall.
        if (intersection.Length == wall.Length)
        {            
            DrawSolidWallRange(x1, x2, colorSeed);
        }
        else
        {
            var sortedIntersection = intersection.OrderBy(i => i).ToArray();
            var i1 = sortedIntersection.First();
            var i2 = sortedIntersection.Last() + 1;
            DrawSolidWallRange(i1, i2, colorSeed);
        }

        // Remove the intersection from the available screen range.
        ScreenRange = ScreenRange.Except(intersection).ToArray();
    }

    private void DrawSolidWallRange(int x1, int x2, int colorSeed = 0)
    {
        for (var i = x1; i < x2; i++)
        {            
            _renderer.VerticalLinesToDraw.AddLast((i, i, colorSeed));
        }
    }

    private void InitializeScreenRange()
    {
        ScreenRange = new int[Settings.Width];
        for (var i = 0; i < ScreenRange.Length; i++)
        {
            ScreenRange[i] = i;
        }
    }
}