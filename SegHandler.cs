using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Doom;

public class SegHandler
{
    private readonly DoomEngine _engine;
    private readonly Player _player;
    private readonly ViewRenderer _renderer;
    private const float MaxScale = 64.0f;
    private const float MinScale = 0.00390625f;
    private readonly float[] XToAngle;

    public SegHandler(DoomEngine engine)
    {
        _engine = engine;
        _player = _engine.Player;
        _renderer = _engine.ViewRenderer;
        XToAngle = CreateXToAngleLookup();
    }

    public Seg CurrentSeg { get; private set; }
    public float CurrentRwAngle1 { get; private set; }
    public int[] ScreenRange { get; private set; }
    public int[] UpperClip { get; private set; }
    public int[] LowerClip { get; private set; }

    private float[] CreateXToAngleLookup()
    {
        var table = new float[Settings.Width + 1];

        for (var i = 0; i < table.Length; i++)
        {
            var angle = MathHelper.ToDegrees(MathF.Atan((Settings.HalfWidth - i) / Settings.ScreenDistance));
            table[i] = angle;
        }

        return table;
    }

    public void Update(GameTime gameTime)
    {
        InitializeFloorAndCeilingClipHeights();
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

        // Logic indicates a window
        if (frontSector.CeilingHeight != backSector.CeilingHeight ||
            frontSector.FloorHeight != backSector.FloorHeight)
        {
            ClipPortalWalls(x1, x2, colorSeed);
            return;
        }

        // Reject empty lines used for triggers and
        // other special effects. Identical floor
        // and ceiling textures, light levels, and 
        // no middle texture.
        if (backSector.CeilingTexture == frontSector.CeilingTexture
            && backSector.FloorTexture == frontSector.FloorTexture
            && backSector.LightLevel == frontSector.LightLevel
            && CurrentSeg.Linedef.FrontSidedef.MiddleTexture == null)
        {
            return;
        }

        // Different light levels and textures
        ClipPortalWalls(x1, x2, colorSeed);
    }

    private void ClipSolidWalls(int xStart, int xEnd, int colorSeed = 0)
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
        if (xStart >= xEnd)
        {
            return;
        }

        var wall = new int[xEnd - xStart];
        for (var i = 0; i < wall.Length; i++)
        {
            wall[i] = xStart + i;
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
            DrawSolidWallRange(xStart, xEnd -1, colorSeed);
        }
        else
        {
            var sortedIntersection = intersection.OrderBy(i => i).ToArray();
            var x = sortedIntersection.First();
            var x2 = sortedIntersection.Last();
            for (var i = 0; i < sortedIntersection.Length - 1; i++)
            {
                var x1 = sortedIntersection[i];
                x2 = sortedIntersection[i + 1];
                if (x2 - x1 > 1)
                {
                    DrawSolidWallRange(x, x1, colorSeed);
                    x = x2;;
                }
            }            
            DrawSolidWallRange(x, x2, colorSeed);
        }

        // Remove the intersection from the available screen range.
        ScreenRange = ScreenRange.Except(intersection).ToArray();
    }

    private void ClipPortalWalls(int xStart, int xEnd, int colorSeed = 0)
    {
        // Something will have gone wrong if we have
        // this condition as true.
        if (xStart >= xEnd)
        {
            return;
        }

        var wall = new int[xEnd - xStart];
        for (var i = 0; i < wall.Length; i++)
        {
            wall[i] = xStart + i;
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
            DrawPortalWallRange(xStart, xEnd - 1, colorSeed);
        }
        else
        {
            var sortedIntersection = intersection.OrderBy(i => i).ToArray();
            var x = sortedIntersection.First();
            for (var i = 0; i < sortedIntersection.Length - 1; i++)
            {
                var x1 = sortedIntersection[i];
                var x2 = sortedIntersection[i + 1];
                if (x2 - x1 > 1)
                {
                    DrawPortalWallRange(x, x1, colorSeed);
                    x = x2;;
                }
            }
            DrawPortalWallRange(x, sortedIntersection.Last(), colorSeed);
        }
    }

    private void DrawSolidWallRange(int x1, int x2, int colorSeed = 0)
    {
        var wallTexture = CurrentSeg.Linedef.FrontSidedef.MiddleTexture;
        var ceilingTexture = CurrentSeg.FrontSector.CeilingTexture;
        var floorTexture = CurrentSeg.FrontSector.FloorTexture;
        var lightLevel = CurrentSeg.FrontSector.LightLevel;

        var worldFrontZ1 = CurrentSeg.FrontSector.CeilingHeight - _player.Height;
        var worldFrontZ2 = CurrentSeg.FrontSector.FloorHeight - _player.Height;

        var drawWall = wallTexture != "-";
        var drawCeiling = worldFrontZ1 > 0;
        var drawFloor = worldFrontZ2 < 0;

        var rwNormalAngle = CurrentSeg.Angle + 90;
        var offsetAngle = rwNormalAngle - CurrentRwAngle1;

        var hypotenuse = Vector2.Distance(_player.Position, CurrentSeg.StartVertex);
        var rwDistance = hypotenuse * (float)MathF.Cos(MathHelper.ToRadians(offsetAngle)); 

        var rwScale1 = ScaleFromGlobalAngle(x1, rwNormalAngle, rwDistance);
        var scale2 = ScaleFromGlobalAngle(x2, rwNormalAngle, rwDistance);
        var rwScaleStep = (scale2 - rwScale1) / (x2 - x1);

        var wallY1 = Settings.HalfHeight - worldFrontZ1 * rwScale1;
        var wallY1Step = -rwScaleStep * worldFrontZ1;
        var wallY2 = Settings.HalfHeight - worldFrontZ2 * rwScale1;
        var wallY2Step = -rwScaleStep * worldFrontZ2;

        for (var i = x1; i < x2 + 1; i++)
        {                                   
            var drawWallY1 = wallY1 - 1;
            var drawWallY2 = wallY2;
            
            if (drawCeiling)
            {
                var cy1 = UpperClip[i] + 1;
                var cy2 = Math.Min(drawWallY1 - 1, LowerClip[i] - 1);
                _renderer.VertsToDraw.AddLast(
                    new Vert
                    {
                        X = i,
                        YTop = cy1,
                        YBottom = (int)cy2,
                        Texture = ceilingTexture,
                        LightLevel = lightLevel
                    });
            }

            if (drawWall)
            {
                var wy1 = Math.Max(drawWallY1, UpperClip[i] + 1);
                var wy2 = Math.Min(drawWallY2, LowerClip[i] - 1);                
                _renderer.VertsToDraw.AddLast(
                    new Vert
                    {
                        X = i,
                        YTop = (int)wy1,
                        YBottom = (int)wy2,
                        Texture = wallTexture,
                        LightLevel = lightLevel
                    });                
            }

            if (drawFloor)
            {
                var fy1 = Math.Max(drawWallY2 + 1, UpperClip[i] + 1);
                var fy2 = LowerClip[i] - 1;
                _renderer.VertsToDraw.AddLast(
                    new Vert
                    {
                        X = i,
                        YTop = (int)fy1,
                        YBottom = fy2,
                        Texture = floorTexture,
                        LightLevel = lightLevel
                    });
            }

            wallY1 += wallY1Step;
            wallY2 += wallY2Step;
        }
    }

    private void DrawPortalWallRange(int x1, int x2, int colorSeed = 0)
    {
        var frontSector = CurrentSeg.FrontSector;
        var backSector = CurrentSeg.BackSector;
        var side = CurrentSeg.Linedef.FrontSidedef;

        var upperWallTexture = CurrentSeg.Linedef.FrontSidedef.UpperTexture;
        var lowerWallTexture = CurrentSeg.Linedef.FrontSidedef.LowerTexture;
        var ceilingTexture = CurrentSeg.FrontSector.CeilingTexture;
        var floorTexture = CurrentSeg.FrontSector.FloorTexture;
        var lightLevel = CurrentSeg.FrontSector.LightLevel;

        var worldFrontZ1 = CurrentSeg.FrontSector.CeilingHeight - _player.Height;
        var worldFrontZ2 = CurrentSeg.FrontSector.FloorHeight - _player.Height;
        var worldBackZ1 = CurrentSeg.BackSector.CeilingHeight - _player.Height;
        var worldBackZ2 = CurrentSeg.BackSector.FloorHeight - _player.Height;

        var drawCeiling = false;
        var drawFloor = false;
        var drawUpperWall = false;
        var drawLowerWall = false;

        if (worldFrontZ1 != worldBackZ1
            || frontSector.LightLevel != backSector.LightLevel
            || frontSector.CeilingTexture != backSector.CeilingTexture)
        {
            drawUpperWall = side.UpperTexture != "-" && worldBackZ1 < worldFrontZ1;
            drawCeiling = worldFrontZ1 >= 0;
        }

        if (worldFrontZ2 != worldBackZ2
            || frontSector.LightLevel != backSector.LightLevel
            || frontSector.FloorTexture != backSector.FloorTexture)
        {
            drawLowerWall = side.LowerTexture != "-" && worldBackZ2 > worldFrontZ2;
            drawFloor = worldFrontZ2 <= 0;
        }

        // Nothing to draw here
        if (!drawCeiling && !drawFloor && !drawUpperWall && !drawLowerWall)
        {
            return;
        }

        var rwNormalAngle = CurrentSeg.Angle + 90;
        var offsetAngle = rwNormalAngle - CurrentRwAngle1;

        var hypotenuse = Vector2.Distance(_player.Position, CurrentSeg.StartVertex);
        var rwDistance = hypotenuse * (float)MathF.Cos(MathHelper.ToRadians(offsetAngle)); 

        var rwScale1 = ScaleFromGlobalAngle(x1, rwNormalAngle, rwDistance);
        var scale2 = ScaleFromGlobalAngle(x2, rwNormalAngle, rwDistance);
        var rwScaleStep = (scale2 - rwScale1) / (x2 - x1);

        var wallY1 = Settings.HalfHeight - worldFrontZ1 * rwScale1;
        var wallY1Step = -rwScaleStep * worldFrontZ1;
        var wallY2 = Settings.HalfHeight - worldFrontZ2 * rwScale1;
        var wallY2Step = -rwScaleStep * worldFrontZ2;

        var portalY1 = 0f;
        var portalY1Step = 0f;
        if (worldBackZ1 > worldFrontZ2)
        {
            portalY1 = Settings.HalfHeight - worldBackZ1 * rwScale1;
            portalY1Step = -rwScaleStep * worldBackZ1;
        }
        else
        {
            portalY1 = wallY2;
            portalY1Step = wallY2Step;
        }

        var portalY2 = 0f;
        var portalY2Step = 0f;
        if (worldBackZ2 < worldFrontZ1)
        {
            portalY2 = Settings.HalfHeight - worldBackZ2 * rwScale1;
            portalY2Step = -rwScaleStep * worldBackZ2;
        }
        else
        {
            portalY2 = wallY1;
            portalY2Step = wallY1Step;
        }

        for (var i = x1; i < x2 + 1; i++)
        {                                   
            var drawWallY1 = wallY1 - 1;
            var drawWallY2 = wallY2;
            
            if (drawUpperWall)
            {
                var drawUpperWallY1 = wallY1 - 1;
                var drawUpperWallY2 = portalY1;

                if (drawCeiling)
                {
                    var cy1 = UpperClip[i] + 1;
                    var cy2 = (int)Math.Min(drawWallY1 - 1, LowerClip[i] - 1);
                    _renderer.VertsToDraw.AddLast(
                        new Vert
                        {
                            X = i,
                            YTop = cy1,
                            YBottom = cy2,
                            Texture = ceilingTexture,
                            LightLevel = lightLevel
                        });
                }

                var wy1 = (int)Math.Max(drawUpperWallY1, UpperClip[i] + 1);
                var wy2 = (int)Math.Min(drawUpperWallY2, LowerClip[i] - 1);
                _renderer.VertsToDraw.AddLast(
                    new Vert
                    {
                        X = i,
                        YTop = wy1,
                        YBottom = wy2,
                        Texture = upperWallTexture,
                        LightLevel = lightLevel
                    });     

                if (UpperClip[i] < wy2)
                {
                    UpperClip[i] = wy2;
                }

                portalY1 += portalY1Step;
            }

            if (drawCeiling)
            {
                var cy1 = UpperClip[i] + 1;
                var cy2 = (int)Math.Min(drawWallY1 - 1, LowerClip[i] - 1);
                _renderer.VertsToDraw.AddLast(
                    new Vert
                    {
                        X = i,
                        YTop = cy1,
                        YBottom = cy2,
                        Texture = ceilingTexture,
                        LightLevel = lightLevel
                    });
                
                if (UpperClip[i] < cy2)
                {
                    UpperClip[i] = cy2;
                }
            }

            if (drawLowerWall)
            {
                if (drawFloor)
                {
                    var fy1 = (int)Math.Max(drawWallY2 + 1, UpperClip[i] + 1);
                    var fy2 = LowerClip[i] - 1;
                    _renderer.VertsToDraw.AddLast(
                        new Vert
                        {
                            X = i,
                            YTop = fy1,
                            YBottom = fy2,
                            Texture = floorTexture,
                            LightLevel = lightLevel
                        });
                }

                var drawLowerWallY1 = portalY2 - 1;
                var drawLowerWallY2 = wallY2;

                var wy1 = (int)Math.Max(drawLowerWallY1, UpperClip[i] + 1);
                var wy2 = (int)Math.Min(drawLowerWallY2, LowerClip[i] - 1);
                _renderer.VertsToDraw.AddLast(
                    new Vert
                    {
                        X = i,
                        YTop = wy1,
                        YBottom = wy2,
                        Texture = lowerWallTexture,
                        LightLevel = lightLevel
                    });     

                if (LowerClip[i] > wy1)
                {
                    LowerClip[i] = wy1;
                }

                portalY2 += portalY2Step;
            }

            if (drawFloor)
            {
                var fy1 = (int)Math.Max(drawWallY2 + 1, UpperClip[i] + 1);
                var fy2 = LowerClip[i] - 1;
                _renderer.VertsToDraw.AddLast(
                    new Vert
                    {
                        X = i,
                        YTop = fy1,
                        YBottom = fy2,
                        Texture = floorTexture,
                        LightLevel = lightLevel
                    });

                if (LowerClip[i] > drawWallY2 + 1)
                {
                    LowerClip[i] = fy1;
                }
            }

            wallY1 += wallY1Step;
            wallY2 += wallY2Step;
        }
    }

    private float ScaleFromGlobalAngle(int x, float rwNormalAngle, float rwDistance)
    {        
        var xAngle = XToAngle[x];
        var numerator = Settings.ScreenDistance * MathF.Cos(MathHelper.ToRadians(rwNormalAngle - xAngle - _player.Angle));
        var denominator = rwDistance * MathF.Cos(MathHelper.ToRadians(xAngle));

        var scale = numerator / denominator;
        scale = Math.Min(MaxScale, Math.Max(MinScale, scale));
        return scale;
    }

    private void InitializeScreenRange()
    {
        ScreenRange = new int[Settings.Width];
        for (var i = 0; i < ScreenRange.Length; i++)
        {
            ScreenRange[i] = i;
        }
    }

    private void InitializeFloorAndCeilingClipHeights()
    {
        UpperClip = new int[Settings.Width];
        LowerClip = new int[Settings.Width];
        for (var i = 0; i < Settings.Width; i++)
        {
            UpperClip[i] = -1;
            LowerClip[i] = Settings.Height;           
        }
    }
}