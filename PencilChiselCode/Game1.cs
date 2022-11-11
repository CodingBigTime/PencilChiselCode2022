using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PencilChiselCode.Source;

namespace PencilChiselCode;

public class Game1 : Game
{
    public readonly GraphicsDeviceManager Graphics;
    public SpriteBatch SpriteBatch;
    public Dictionary<string, Texture2D> TextureMap { get; } = new();
    public Dictionary<string, SoundEffect> SoundMap { get; } = new();
    private Button _button;
    private Player _player;
    public static Game1 Instance { get; private set; }

    public Game1()
    {
        Instance = this;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content/Resources";
        IsMouseVisible = true;
        Window.AllowUserResizing = false;
        Window.Title = "PCC";
    }

    protected override void Initialize()
    {
        base.Initialize();
        Graphics.IsFullScreen = false;
        Graphics.PreferredBackBufferWidth = 800;
        Graphics.PreferredBackBufferHeight = 800;
        Graphics.ApplyChanges();
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
        TextureMap.Add("player", Content.Load<Texture2D>("Textures/Entity/player"));

        SoundMap.Add("button_press", Content.Load<SoundEffect>("Sounds/button_press"));
        SoundMap.Add("button_release", Content.Load<SoundEffect>("Sounds/button_release"));

        _button = new Button(TextureMap["start_button_normal"], TextureMap["start_button_hover"],
            TextureMap["start_button_pressed"]);
        _player = new Player(TextureMap["player"], new Vector2(150, 150));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        _button.Update(gameTime);
        _player.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Crimson);

        SpriteBatch.Begin();

        _button.Draw(SpriteBatch);
        _player.Draw(SpriteBatch);

        SpriteBatch.End();

        base.Draw(gameTime);
    }

    public int GetWindowWidth() => Window.ClientBounds.Width;
    public int GetWindowHeight() => Window.ClientBounds.Height;
    public Vector2 GetWindowDimensions() => new(GetWindowWidth(), GetWindowHeight());
}