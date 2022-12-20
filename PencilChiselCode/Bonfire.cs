using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Content;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Serialization;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using PencilChiselCode.Source;
using PencilChiselCode.Source.GameStates;
using PencilChiselCode.Source.GUI;
using Penumbra;

namespace PencilChiselCode;

public class Bonfire : Game
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
    public readonly ScreenManager ScreenManager;
    public TiledMapRenderer TiledMapRenderer;
    public List<TiledMap> TiledMaps = new();
    public const int TreeVariations = 3;
    public Dictionary<string, Song> SongMap { get; } = new();
    public Controls Controls;
    public MouseValues MouseValues { get; set; }

    public int DebugMode { get; set; }
    public bool IsVSyncEnabled => Graphics.SynchronizeWithVerticalRetrace;

    public Bonfire()
    {
        ScreenManager = new ScreenManager();
        Penumbra = new PenumbraComponent(this);
        Penumbra.AmbientColor = Color.Black;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content/Resources";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
        Graphics.SynchronizeWithVerticalRetrace = false;
        IsFixedTimeStep = false;
        Controls = new();
        MouseValues = new();
    }

    protected override void Initialize()
    {
        Graphics.IsFullScreen = false;
        Graphics.PreferredBackBufferWidth = Width;
        Graphics.PreferredBackBufferHeight = Height;
        Graphics.ApplyChanges();
        Penumbra.Initialize();
        Window.Title = "Bonfire";
        base.Initialize();
        var menuState = new MenuState(this);
        ScreenManager.LoadScreen(menuState, new FadeTransition(GraphicsDevice, menuState.BgColor));
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        TextureMap.Add(
            "start_button_normal",
            Content.Load<Texture2D>("Textures/GUI/Buttons/start_button_normal")
        );
        TextureMap.Add(
            "start_button_hover",
            Content.Load<Texture2D>("Textures/GUI/Buttons/start_button_hover")
        );
        TextureMap.Add(
            "start_button_pressed",
            Content.Load<Texture2D>("Textures/GUI/Buttons/start_button_pressed")
        );
        TextureMap.Add(
            "settings_button_normal",
            Content.Load<Texture2D>("Textures/GUI/Buttons/settings_button_normal")
        );
        TextureMap.Add(
            "settings_button_hover",
            Content.Load<Texture2D>("Textures/GUI/Buttons/settings_button_hover")
        );
        TextureMap.Add(
            "settings_button_pressed",
            Content.Load<Texture2D>("Textures/GUI/Buttons/settings_button_pressed")
        );
        TextureMap.Add(
            "exit_button_normal",
            Content.Load<Texture2D>("Textures/GUI/Buttons/exit_button_normal")
        );
        TextureMap.Add(
            "exit_button_hover",
            Content.Load<Texture2D>("Textures/GUI/Buttons/exit_button_hover")
        );
        TextureMap.Add(
            "exit_button_pressed",
            Content.Load<Texture2D>("Textures/GUI/Buttons/exit_button_pressed")
        );
        TextureMap.Add(
            "resume_button_normal",
            Content.Load<Texture2D>("Textures/GUI/Buttons/resume_button_normal")
        );
        TextureMap.Add(
            "resume_button_hover",
            Content.Load<Texture2D>("Textures/GUI/Buttons/resume_button_hover")
        );
        TextureMap.Add(
            "resume_button_pressed",
            Content.Load<Texture2D>("Textures/GUI/Buttons/resume_button_pressed")
        );
        TextureMap.Add(
            "menu_button_normal",
            Content.Load<Texture2D>("Textures/GUI/Buttons/menu_button_normal")
        );
        TextureMap.Add(
            "menu_button_hover",
            Content.Load<Texture2D>("Textures/GUI/Buttons/menu_button_hover")
        );
        TextureMap.Add(
            "menu_button_pressed",
            Content.Load<Texture2D>("Textures/GUI/Buttons/menu_button_pressed")
        );
        TextureMap.Add(
            "restart_button_normal",
            Content.Load<Texture2D>("Textures/GUI/Buttons/restart_button_normal")
        );
        TextureMap.Add(
            "restart_button_hover",
            Content.Load<Texture2D>("Textures/GUI/Buttons/restart_button_hover")
        );
        TextureMap.Add(
            "restart_button_pressed",
            Content.Load<Texture2D>("Textures/GUI/Buttons/restart_button_pressed")
        );
        TextureMap.Add("checkbox_empty", Content.Load<Texture2D>("Textures/GUI/checkbox_empty"));
        TextureMap.Add(
            "checkbox_selected",
            Content.Load<Texture2D>("Textures/GUI/checkbox_selected")
        );

        TextureMap.Add("logo", Content.Load<Texture2D>("Textures/GUI/logo"));

        TextureMap.Add("x_button", Content.Load<Texture2D>("Textures/GUI/x_button"));
        TextureMap.Add("y_button", Content.Load<Texture2D>("Textures/GUI/y_button"));
        TextureMap.Add("a_button", Content.Load<Texture2D>("Textures/GUI/a_button"));
        TextureMap.Add("b_button", Content.Load<Texture2D>("Textures/GUI/b_button"));
        TextureMap.Add("e_key", Content.Load<Texture2D>("Textures/GUI/e_key"));
        TextureMap.Add("f_key", Content.Load<Texture2D>("Textures/GUI/f_key"));
        TextureMap.Add("q_key", Content.Load<Texture2D>("Textures/GUI/q_key"));
        TextureMap.Add("x_key", Content.Load<Texture2D>("Textures/GUI/x_key"));
        TextureMap.Add("space_key", Content.Load<Texture2D>("Textures/GUI/space_key"));
        TextureMap.Add("attribute_bar", Content.Load<Texture2D>("Textures/GUI/attribute_bar"));
        TextureMap.Add("comfy_bar", Content.Load<Texture2D>("Textures/GUI/comfy_bar"));
        TextureMap.Add("fireplace_bar", Content.Load<Texture2D>("Textures/GUI/fireplace_bar"));
        TextureMap.Add("twig", Content.Load<Texture2D>("Textures/GUI/twig"));
        TextureMap.Add("berry", Content.Load<Texture2D>("Textures/GUI/berry"));
        TextureMap.Add("campfire", Content.Load<Texture2D>("Textures/GUI/campfire"));

        TextureMap.Add("twigs", Content.Load<Texture2D>("Textures/Entity/twigs"));
        TextureMap.Add("bush_empty", Content.Load<Texture2D>("Textures/Entity/bush_empty"));
        TextureMap.Add("bush_berry", Content.Load<Texture2D>("Textures/Entity/bush_berry"));
        TextureMap.Add("follower", Content.Load<Texture2D>("Textures/Entity/follower"));

        for (var i = 1; i <= TreeVariations; ++i)
        {
            TextureMap.Add($"tree_{i}", Content.Load<Texture2D>($"Textures/Entity/tree_{i}"));
        }

        TextureMap.Add($"flower_lamp_1", Content.Load<Texture2D>($"Textures/Entity/flower_lamp_1"));

        SoundMap.Add("button_press", Content.Load<SoundEffect>("Sounds/button_press"));
        SoundMap.Add("button_release", Content.Load<SoundEffect>("Sounds/button_release"));
        SoundMap.Add("pickup_branches", Content.Load<SoundEffect>("Sounds/pickup_branches"));
        SoundMap.Add("fuel_fire", Content.Load<SoundEffect>("Sounds/fuel_fire"));
        SoundMap.Add("light_fire", Content.Load<SoundEffect>("Sounds/light_fire"));

        FontMap.Add("12", Content.Load<BitmapFont>("Fonts/lunchds_12"));
        FontMap.Add("16", Content.Load<BitmapFont>("Fonts/lunchds_16"));
        FontMap.Add("24", Content.Load<BitmapFont>("Fonts/lunchds_24"));
        FontMap.Add("32", Content.Load<BitmapFont>("Fonts/lunchds_32"));

        SongMap.Add("bonfire_song", Content.Load<Song>("Sounds/bonfire_song"));

        var fireSpriteSheet = Content.Load<SpriteSheet>(
            "Animations/fire.spritesheet",
            new JsonContentLoader()
        );
        SpriteSheetMap.Add("fire", fireSpriteSheet);
        var playerSpriteSheet = Content.Load<SpriteSheet>(
            "Animations/player.spritesheet",
            new JsonContentLoader()
        );
        SpriteSheetMap.Add("player", playerSpriteSheet);

        for (var i = 1; i <= 7; ++i)
        {
            TiledMaps.Add(Content.Load<TiledMap>($"Textures/Tiles/tilemap_{i}"));
        }

        TiledMapRenderer = new TiledMapRenderer(GraphicsDevice, TiledMaps[0]);
    }

    public void Start()
    {
        ScreenManager.LoadScreen(
            new IngameState(this),
            new FadeTransition(GraphicsDevice, Color.Black, 0.5F)
        );
        ResetPenumbra();
    }

    public void ResetPenumbra()
    {
        Penumbra = new PenumbraComponent(this) { AmbientColor = Color.Black };
        Penumbra.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        if (Controls.JustPressed(ControlKeys.Debug))
        {
            DebugMode = (DebugMode + 1) % 3;
        }

        ScreenManager.Update(gameTime);
        MouseValues.Update();
        Controls.Update();
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

    public int GetScreenWidth() => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

    public int GetScreenHeight() => GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

    public Vector2 GetScreenDimensions() => new(GetScreenWidth(), GetScreenHeight());

    public AbsoluteBox GetRootBox() => new(this, new Vector2(0), GetWindowDimensions);

    public void SetVSync(bool enabled)
    {
        Graphics.SynchronizeWithVerticalRetrace = enabled;
        Graphics.ApplyChanges();
    }

    public WindowMode GetWindowMode()
    {
        if (Graphics.IsFullScreen)
        {
            return Window.IsBorderless ? WindowMode.BorderlessFullscreen : WindowMode.Fullscreen;
        }
        return WindowMode.Windowed;
    }

    public void SetWindowMode(WindowMode windowMode)
    {
        Debug.WriteLine(windowMode);
        switch (windowMode)
        {
            case WindowMode.Fullscreen:
                Graphics.PreferredBackBufferWidth = Width;
                Graphics.PreferredBackBufferHeight = Height;
                Graphics.IsFullScreen = true;
                Graphics.ApplyChanges();
                Window.IsBorderless = false;
                break;
            case WindowMode.Windowed:
                Graphics.PreferredBackBufferWidth = Width;
                Graphics.PreferredBackBufferHeight = Height;
                Graphics.IsFullScreen = false;
                Graphics.ApplyChanges();
                Window.IsBorderless = false;
                var ayo = GetScreenDimensions();
                Window.Position = (GetScreenDimensions() / 2 - GetWindowDimensions() / 2).ToPoint();
                break;
            case WindowMode.BorderlessFullscreen:
                Graphics.PreferredBackBufferWidth = GraphicsAdapter
                    .DefaultAdapter
                    .CurrentDisplayMode
                    .Width;
                Graphics.PreferredBackBufferHeight = GraphicsAdapter
                    .DefaultAdapter
                    .CurrentDisplayMode
                    .Height;
                Graphics.ApplyChanges();
                Window.Position = new Point(0, 0);
                Window.IsBorderless = true;
                break;
        }
    }
}
