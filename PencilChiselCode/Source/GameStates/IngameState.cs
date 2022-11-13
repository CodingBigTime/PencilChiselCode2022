using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;
using MonoGame.Extended.Tiled;

namespace PencilChiselCode.Source.GameStates;

public class IngameState : GameScreen
{
    public static readonly Color BgColor = Color.Green;
    private Game1 _game => (Game1)Game;
    private Player _player;
    private Companion _companion;
    private bool _showDebug;

    public readonly HashSet<Keys> PreviousPressedKeys = new();

    private static float _cameraSpeed = 10.0F;
    private AttributeGroup _followerAttributes;
    private int _fps;
    private TimeSpan _fpsCounterGameTime;
    private static bool _pauseState;
    private Button _pauseButton;
    private Button _menuButton;
    private List<TiledMap> _maps;
    private readonly List<string> _debugData = new() { "", "", "" };

    private int MapIndex =>
        (int)Math.Abs(Math.Floor(_game.Camera.GetViewMatrix().Translation.X / _maps[0].WidthInPixels));

    public IngameState(Game game) : base(game)
    {
    }

    public List<Pickupable> Pickupables { get; } = new();
    public List<CampFire> Campfires { get; } = new();

    public override void LoadContent()
    {
        base.LoadContent();
        var resumeButton = _game.TextureMap["resume_button_normal"];
        var resumeButtonSize = new Size2(resumeButton.Width, resumeButton.Height);
        _pauseButton = new Button(resumeButton,
            _game.TextureMap["resume_button_hover"],
            _game.TextureMap["resume_button_pressed"],
            Utils.GetCenterStartCoords(resumeButtonSize, Game1.Instance.GetWindowDimensions()),
            () => { _pauseState = false; }
        );
        var menuButton = _game.TextureMap["menu_button_normal"];
        var menuButtonSize = new Size2(menuButton.Width, menuButton.Height);
        _menuButton = new Button(menuButton,
            _game.TextureMap["menu_button_hover"],
            _game.TextureMap["menu_button_pressed"],
            Utils.GetCenterStartCoords(menuButtonSize, Game1.Instance.GetWindowDimensions()) + Vector2.UnitY * 100,
            () =>
            {
                Game1.Instance.ScreenManager.LoadScreen(new MenuState(_game),
                    new FadeTransition(Game1.Instance.GraphicsDevice, MenuState.BgColor));
            }
        );
        for (var i = 0; i < 10; ++i)
        {
            var pickupable = new Pickupable(PickupableTypes.Twig, _game.TextureMap["twigs"],
                _game.SoundMap["pickup_branches"],
                new Vector2(100 + Utils.RANDOM.Next(1, 20) * 50, Utils.RANDOM.Next(1, 15) * 50),
                0.5F);
            Pickupables.Add(pickupable);
        }

        _companion = new Companion(_game, new Vector2(100, 100), 50F);
        _player = new Player(_game, new Vector2(150, 150));

        Campfires.Add(new CampFire(_game, new Vector2(500, 400))); // TEMP

        _maps = new List<TiledMap>();
        for (var i = 0; i < 3; ++i)
        {
            AddRandomMap();
        }

        _followerAttributes = new AttributeGroup(new List<Attribute>
        {
            new(new(10, 10), new(100, 10), Color.Brown, 100, -0.5F),
            new(new(10, 30), new(100, 10), Color.LightBlue, 100, -1F),
            new(new(10, 50), new(100, 10), Color.Orange, 100, -5F),
            new(new(10, 70), new(100, 10), Color.Blue, 100, -2F)
        });
    }

    private void AddRandomMap() => _maps.Add(_game.TiledMaps[Utils.RANDOM.Next(0, _game.TiledMaps.Count)]);

    public override void Update(GameTime gameTime)
    {
        var oldMapIndex = MapIndex;
        _game.TiledMapRenderer.Update(gameTime);
        var keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.Escape) && !PreviousPressedKeys.Contains(Keys.Escape))
        {
            _pauseState = !_pauseState;
        }

        if (_pauseState)
        {
            _pauseButton.Update(gameTime);
            _menuButton.Update(gameTime);
        }
        else
        {
            _game.Camera.Move(Vector2.UnitX * _cameraSpeed * gameTime.GetElapsedSeconds());
            _companion.Update(this, gameTime, _player.Position);
            _player.Update(this, gameTime);
            _followerAttributes.Update(gameTime);
            Pickupables.ForEach(pickupable => pickupable.Update(gameTime));
            Campfires.ForEach(campfire => { campfire.Update(gameTime); });
            if (Campfires.Any(campfire => campfire.IsInRange(_companion.Position)))
            {
                _followerAttributes.Attributes[2].ChangeValue(10F * gameTime.GetElapsedSeconds());
            }
        }

        if (keyState.IsKeyDown(Keys.F3) && !PreviousPressedKeys.Contains(Keys.F3))
        {
            _showDebug = !_showDebug;
        }

        if (keyState.IsKeyDown(Keys.Space) && !PreviousPressedKeys.Contains(Keys.Space))
        {
            _companion.StopResumeFollower();
        }

        if (oldMapIndex != MapIndex)
        {
            _maps.RemoveAt(0);
            AddRandomMap();
        }

        PreviousPressedKeys.Clear();
        PreviousPressedKeys.UnionWith(keyState.GetPressedKeys());
        _debugData[1] = $"Translation: {_game.Camera.GetViewMatrix().Translation}";
        _debugData[2] = $"Map Index: {MapIndex}";
    }

    public override void Draw(GameTime gameTime)
    {
        _game.Penumbra.BeginDraw();

        _game.Penumbra.Transform = Matrix.CreateTranslation(-_game.Camera.Position.X, -_game.Camera.Position.Y, 0);
        _game.GraphicsDevice.Clear(BgColor);
        var transformMatrix = _game.Camera.GetViewMatrix();

        for (var i = 0; i < _maps.Count; ++i)
        {
            _game.TiledMapRenderer.LoadMap(_maps[i]);
            _game.TiledMapRenderer.Draw(
                transformMatrix * Matrix.CreateTranslation(768 * (i + MapIndex-1), 0, 0));
        }

        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

        Pickupables.ForEach(pickupable => pickupable.Draw(_game.SpriteBatch));

        _companion.Draw(_game.SpriteBatch);
        _player.Draw(_game.SpriteBatch);

        Campfires.ForEach(campfire => campfire.Draw(_game.SpriteBatch)); // TEMP

        _game.SpriteBatch.End();
        _game.Penumbra.Draw(gameTime);

        DrawUI(gameTime);
    }

    private void DrawUI(GameTime gameTime)
    {
        var transformMatrix = _game.Camera.GetViewMatrix();
        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        _player.DrawPopupButton(_game.SpriteBatch);
        Campfires.ForEach(campfire => campfire.DrawUI(_game.SpriteBatch)); // TEMP
        _game.SpriteBatch.End();

        _game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _followerAttributes.Draw(_game.SpriteBatch);
        if (gameTime.TotalGameTime.Subtract(_fpsCounterGameTime).Milliseconds >= 500)
        {
            _fps = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            _fpsCounterGameTime = gameTime.TotalGameTime;
            _debugData[0] = $"FPS: {_fps}";
        }

        if (_pauseState)
        {
            _pauseButton.Draw(_game.SpriteBatch);
            _menuButton.Draw(_game.SpriteBatch);
        }

        if (_showDebug)
        {
            for (var i = 0; i < _debugData.Count; ++i)
            {
                _game.SpriteBatch.DrawString(_game.FontMap["16"], _debugData[i],
                    new Vector2(16, 16 * (i + 1)), Color.White);
            }
        }

        _game.SpriteBatch.End();
    }
}