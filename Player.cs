using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Doom;

public class Player
{
    public const int ThingIndex = 0;
    private readonly DoomEngine _engine;
    private readonly Thing _thing;
    private Vector2 _position;
    private float _angle;
    private readonly float DiagonalMovementSpeedCorrection = 1 / MathF.Sqrt(2);

    public Player(DoomEngine engine)
    {
        _engine = engine;
        _thing = _engine.WADData.Things[ThingIndex];
        _position = _thing.Position;
        _angle = MathHelper.ToRadians(_thing.Angle);
    }

    public Vector2 Position => _position;
    public float Angle => _angle;

    public void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        Rotate(gameTime, keyboardState);
        Move(gameTime, keyboardState);
    }

    private void Move(GameTime gameTime, KeyboardState keyboardState)
    {
        var movementSpeed = Settings.PlayerSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        var sin = MathF.Sin(_angle);
        var cos = MathF.Cos(_angle);        
        sin *= movementSpeed;
        cos *= movementSpeed;
        var delta = Vector2.Zero;

        if (keyboardState.IsKeyDown(Keys.W))
        {
            delta.X += cos;
            delta.Y += sin;
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            delta.X -= cos;
            delta.Y -= sin;
        }

        if (keyboardState.IsKeyDown(Keys.A))
        {
            delta.X -= sin;
            delta.Y += cos;
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            delta.X += sin;
            delta.Y -= cos;
        }

        _position += delta;
    }

    private void Rotate(GameTime gameTime, KeyboardState keyboardState)
    {
        var rotationSpeed = Settings.PlayerRotationSpeed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

        if (keyboardState.IsKeyDown(Keys.Left))
        {
            _angle += rotationSpeed;
        }
        
        if (keyboardState.IsKeyDown(Keys.Right))
        {
            _angle -= rotationSpeed;
        }
    }
}