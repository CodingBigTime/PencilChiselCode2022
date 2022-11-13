using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Content;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;
using PencilChiselCode.Source.GameStates;
using Penumbra;

namespace PencilChiselCode;

public class Game1 : Game
{
    public readonly int Width = 1366;
    public readonly int Height = 768;
    public readonly GraphicsDeviceManager Graphics;
    public PenumbraComponent Penumbra;
    public SpriteBatch SpriteBatch;
    public Dictionary<string, Texture2D> TextureMap { get; } = new();
    public Dictionary<string, SoundEffect> SoundMap { get; } = new();
    public Dictionary<string, BitmapFont> FontMap { get; } = new();
    public Dictionary<string, SpriteSheet> SpriteSheetMap { get; } = new();
    public static Game1 Instance { get; private set; }
    public readonly ScreenManager ScreenManager;
    public OrthographicCamera Camera;
    public TiledMapRenderer TiledMapRenderer;
    public List<TiledMap> TiledMaps = new();

    public Game1()
    {
        ScreenManager = new ScreenManager();
        Components.Add(ScreenManager);
        Penumbra = new PenumbraComponent(this);
        Penumbra.AmbientColor = Color.Black;
        Instance = this;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content/Resources";
        IsMouseVisible = true;
        Window.AllowUserResizing = false;
        Window.Title = "PCC";
        Graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
    }

    protected override void Initialize()
    {
        Graphics.IsFullScreen = false;
        Graphics.PreferredBackBufferWidth = Width;
        Graphics.PreferredBackBufferHeight = Height;
        Graphics.ApplyChanges();
        Camera = new OrthographicCamera(GetViewportAdapter());
        Penumbra.Initialize();
        base.Initialize();
        ScreenManager.LoadScreen(new MenuState(this));
    }

    public BoxingViewportAdapter GetViewportAdapter() => new(Window, GraphicsDevice, Width, Height);

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        TextureMap.Add("start_button_normal", Content.Load<Texture2D>("Textures/GUI/Buttons/start_button_normal"));
        TextureMap.Add("start_button_hover", Content.Load<Texture2D>("Textures/GUI/Buttons/start_button_hover"));
        TextureMap.Add("start_button_pressed", Content.Load<Texture2D>("Textures/GUI/Buttons/start_button_pressed"));
        TextureMap.Add("exit_button_normal", Content.Load<Texture2D>("Textures/GUI/Buttons/exit_button_normal"));
        TextureMap.Add("exit_button_hover", Content.Load<Texture2D>("Textures/GUI/Buttons/exit_button_hover"));
        TextureMap.Add("exit_button_pressed", Content.Load<Texture2D>("Textures/GUI/Buttons/exit_button_pressed"));
        TextureMap.Add("resume_button_normal", Content.Load<Texture2D>("Textures/GUI/Buttons/resume_button_normal"));
        TextureMap.Add("resume_button_hover", Content.Load<Texture2D>("Textures/GUI/Buttons/resume_button_hover"));
        TextureMap.Add("resume_button_pressed", Content.Load<Texture2D>("Textures/GUI/Buttons/resume_button_pressed"));
        TextureMap.Add("menu_button_normal", Content.Load<Texture2D>("Textures/GUI/Buttons/menu_button_normal"));
        TextureMap.Add("menu_button_hover", Content.Load<Texture2D>("Textures/GUI/Buttons/menu_button_hover"));
        TextureMap.Add("menu_button_pressed", Content.Load<Texture2D>("Textures/GUI/Buttons/menu_button_pressed"));

        TextureMap.Add("e_button", Content.Load<Texture2D>("Textures/GUI/e_button"));
        TextureMap.Add("f_button", Content.Load<Texture2D>("Textures/GUI/f_button"));
        TextureMap.Add("attribute_bar", Content.Load<Texture2D>("Textures/GUI/attribute_bar"));
        TextureMap.Add("comfy_bar", Content.Load<Texture2D>("Textures/GUI/comfy_bar"));
        TextureMap.Add("fireplace_bar", Content.Load<Texture2D>("Textures/GUI/fireplace_bar"));

        TextureMap.Add("twigs", Content.Load<Texture2D>("Textures/Entity/twigs"));
        TextureMap.Add("bush_empty", Content.Load<Texture2D>("Textures/Entity/bush_empty"));
        TextureMap.Add("bush_berry", Content.Load<Texture2D>("Textures/Entity/bush_berry"));
        TextureMap.Add("follower", Content.Load<Texture2D>("Textures/Entity/follower"));

        TextureMap.Add("fire_01", Content.Load<Texture2D>("Textures/Tiles/fire_01"));
        TextureMap.Add("fire_02", Content.Load<Texture2D>("Textures/Tiles/fire_02"));
        TextureMap.Add("fire_03", Content.Load<Texture2D>("Textures/Tiles/fire_03"));
        TextureMap.Add("fire_04", Content.Load<Texture2D>("Textures/Tiles/fire_04"));

        SoundMap.Add("button_press", Content.Load<SoundEffect>("Sounds/button_press"));
        SoundMap.Add("button_release", Content.Load<SoundEffect>("Sounds/button_release"));
        SoundMap.Add("pickup_branches", Content.Load<SoundEffect>("Sounds/pickup_branches"));
        SoundMap.Add("fuel_fire", Content.Load<SoundEffect>("Sounds/fuel_fire"));

        FontMap.Add("12", Content.Load<BitmapFont>("Fonts/lunchds_12"));
        FontMap.Add("16", Content.Load<BitmapFont>("Fonts/lunchds_16"));
        FontMap.Add("24", Content.Load<BitmapFont>("Fonts/lunchds_24"));
        FontMap.Add("32", Content.Load<BitmapFont>("Fonts/lunchds_32"));

        var fireSpriteSheet = Content.Load<SpriteSheet>("Animations/fire.spritesheet", new JsonContentLoader());
        SpriteSheetMap.Add("fire", fireSpriteSheet);
        var playerSpriteSheet = Content.Load<SpriteSheet>("Animations/player.spritesheet", new JsonContentLoader());
        SpriteSheetMap.Add("player", playerSpriteSheet);

        for (var i = 1; i <= 2; ++i)
        {
            TiledMaps.Add(Content.Load<TiledMap>($"Textures/Tiles/tilemap_{i}"));
        }

        TiledMapRenderer = new TiledMapRenderer(GraphicsDevice, TiledMaps[0]);
    }

    protected override void Update(GameTime gameTime)
    {
        ScreenManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        ScreenManager.Draw(gameTime);

        base.Draw(gameTime);
    }

    public int GetWindowWidth() => Window.ClientBounds.Width;
    public int GetWindowHeight() => Window.ClientBounds.Height;
    public Vector2 GetWindowDimensions() => new(GetWindowWidth(), GetWindowHeight());
}