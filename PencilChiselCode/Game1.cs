using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Screens;
using MonoGame.Extended.ViewportAdapters;
using PencilChiselCode.Source;

namespace PencilChiselCode;

public class Game1 : Game
{
    public readonly int Width = 800;
    public readonly int Height = 800;
    public readonly GraphicsDeviceManager Graphics;
    public SpriteBatch SpriteBatch;
    public Dictionary<string, Texture2D> TextureMap { get; } = new();
    public Dictionary<string, SoundEffect> SoundMap { get; } = new();
    public static Game1 Instance { get; private set; }
    public readonly ScreenManager ScreenManager;
    public OrthographicCamera Camera;

    public Game1()
    {
        ScreenManager = new ScreenManager();
        Components.Add(ScreenManager);
        Instance = this;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content/Resources";
        IsMouseVisible = true;
        Window.AllowUserResizing = false;
        Window.Title = "PCC";
    }

    protected override void Initialize()
    {
        Graphics.IsFullScreen = false;
        Graphics.PreferredBackBufferWidth = Width;
        Graphics.PreferredBackBufferHeight = Height;
        Graphics.ApplyChanges();
        var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, Width, Height);
        Camera = new OrthographicCamera(viewportAdapter);
        base.Initialize();
        ScreenManager.LoadScreen(new MenuState(this));
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        TextureMap.Add("start_button_normal", Content.Load<Texture2D>("Textures/GUI/Buttons/start_button_normal"));
        TextureMap.Add("start_button_hover", Content.Load<Texture2D>("Textures/GUI/Buttons/start_button_hover"));
        TextureMap.Add("start_button_pressed", Content.Load<Texture2D>("Textures/GUI/Buttons/start_button_pressed"));
        TextureMap.Add("exit_button_normal", Content.Load<Texture2D>("Textures/GUI/Buttons/exit_button_normal"));
        TextureMap.Add("exit_button_hover", Content.Load<Texture2D>("Textures/GUI/Buttons/exit_button_hover"));
        TextureMap.Add("exit_button_pressed", Content.Load<Texture2D>("Textures/GUI/Buttons/exit_button_pressed"));
        TextureMap.Add("player_down", Content.Load<Texture2D>("Textures/Entity/player_01"));
        TextureMap.Add("player_up", Content.Load<Texture2D>("Textures/Entity/player_02"));
        TextureMap.Add("player_left", Content.Load<Texture2D>("Textures/Entity/player_03"));
        TextureMap.Add("twigs", Content.Load<Texture2D>("Textures/Entity/twigs"));

        SoundMap.Add("button_press", Content.Load<SoundEffect>("Sounds/button_press"));
        SoundMap.Add("button_release", Content.Load<SoundEffect>("Sounds/button_release"));
    }

    protected override void Update(GameTime gameTime)
    {
        ScreenManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Crimson);
        ScreenManager.Draw(gameTime);
        base.Draw(gameTime);
    }

    public int GetWindowWidth() => Window.ClientBounds.Width;
    public int GetWindowHeight() => Window.ClientBounds.Height;
    public Vector2 GetWindowDimensions() => new(GetWindowWidth(), GetWindowHeight());
}