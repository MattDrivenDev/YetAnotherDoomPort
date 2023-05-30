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
        _graphics.IsFullScreen = false;
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

    public BSP BSP { get; private set; }

    public ViewRenderer ViewRenderer { get; private set; }

    public SegHandler SegHandler { get; private set; }

    protected override void Initialize()
    {
        WADData = new WADData(this, Settings.StartMap);
        Player = new Player(this);
        ViewRenderer = new ViewRenderer(this);
        SegHandler = new SegHandler(this);
        BSP = new BSP(this);
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
        SegHandler.Update(gameTime);
        BSP.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        if (Settings.UseMapRenderer)
        {
            MapRenderer.Draw(_spriteBatch);
        }
        else
        {
            ViewRenderer.Draw(_spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
