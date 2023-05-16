using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Doom;

public class DoomEngine : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public DoomEngine(string wadPath = "content/doom1.wad")
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = Settings.Width;
        _graphics.PreferredBackBufferHeight = Settings.Height;
        _graphics.IsFullScreen = true;
        Content.RootDirectory = "Content";
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1d / Settings.TargetFps);
        IsMouseVisible = true;
        _graphics.ApplyChanges();
        WADPath = wadPath;
    }

    public string WADPath { get; init; }

    public WADData WADData { get; private set; }

    public MapRenderer MapRenderer { get; private set; }

    public Player Player { get; private set; }

    protected override void Initialize()
    {
        WADData = new WADData(this, Settings.StartMap);
        Player = new Player(this);
        MapRenderer = new MapRenderer(this);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Player.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        MapRenderer.Draw(_spriteBatch);
        //_spriteBatch.DrawString

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
