using System;
using Microsoft.Xna.Framework;

namespace Doom;

public static class Settings
{
    public static string StartMap = "E1M1";
    public static int DoomWidth = 320;
    public static int DoomHeight = 200;
    public static int Scale = 4;
    public static int Width = DoomWidth * Scale;
    public static int Height = DoomHeight * Scale;
    public static int HalfWidth = Width / 2;
    public static int HalfHeight = Height / 2;
    public static int TargetFps = 60;
    public static int FOV = 90;
    public static int HalfFOV = FOV / 2;
    public static float PlayerSpeed = 0.3f;
    public static float PlayerRotationSpeed = 0.12f;
    public static float ScreenDistance = HalfWidth / MathF.Tan(MathHelper.ToRadians(HalfFOV));
} 