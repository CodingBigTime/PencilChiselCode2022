using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace PencilChiselCode.Source.GameStates;

public class IngameState : GameScreen
{
    public static readonly Color BgColor = Color.Green;
    private Game1 _game => (Game1)Game;
    private Player _player;
    private Companion _companion;
    private CampFire _testCampFire;
    private bool _showDebug;
    private readonly HashSet<Keys> _previousPressedKeys = new();
    private static float _cameraSpeed = 10.0F;
    private AttributeGroup _followerAttributes;
    private int _fps;
    private TimeSpan _fpsCounterGameTime;
    private static bool _pauseState;
    private Button _pauseButton;
    private Button _menuButton;

    public IngameState(Game game) : base(game)
    {
    }

    public List<Pickupable> Pickupables { get; } = new();

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
        Pickupables.Add(new Pickupable(PickupableTypes.Twig, _game.TextureMap["twigs"],
            _game.SoundMap["pickup_branches"], new Vector2(300, 300),
            0.5F));
        _companion = new Companion(_game, new Vector2(100, 100),50F);
        _player = new Player(_game, new Vector2(150, 150));
        
        _testCampFire = new CampFire(_game, new Vector2(500, 400));  // TEMP
        _game.Penumbra.Lights.Add(_testCampFire.PointLight);  // TEMP

        _game.Penumbra.Lights.Add(_player.PointLight);
        _game.Penumbra.Lights.Add(_player.Spotlight);

        _followerAttributes = new AttributeGroup(new List<Attribute>
        {
            new(new(10, 10), new(100, 10), Color.Brown, 100, -0.5F),
            new(new(10, 30), new(100, 10), Color.LightBlue, 100, -1F),
            new(new(10, 50), new(100, 10), Color.Orange, 100, -5F),
            new(new(10, 70), new(100, 10), Color.Blue, 100, -2F)
        });
    }

    public override void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.Escape) && !_previousPressedKeys.Contains(Keys.Escape))
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
            _companion.Update(this,gameTime,_player.Position);
            _player.Update(this, gameTime);
            _followerAttributes.Update(gameTime);
            Pickupables.ForEach(pickupable => pickupable.Update(gameTime));
            _testCampFire.Update(gameTime);  // TEMP
            if (_testCampFire.isInRange(_companion.Position))
            {
                _followerAttributes.Attributes[2].ChangeValue(10F * gameTime.GetElapsedSeconds());
            }
        }

        if (keyState.IsKeyDown(Keys.F3) && !_previousPressedKeys.Contains(Keys.F3))
        {
            _showDebug = !_showDebug;
        }
        if (keyState.IsKeyDown(Keys.Space) && !_previousPressedKeys.Contains(Keys.Space))
        {
            Debug.WriteLine("STOP");
            _companion.StopResumeFollower();
        }

        _previousPressedKeys.Clear();
        _previousPressedKeys.UnionWith(keyState.GetPressedKeys());
    }

    public override void Draw(GameTime gameTime)
    {
        _game.Penumbra.BeginDraw();

        _game.Penumbra.Transform = Matrix.CreateTranslation(-_game.Camera.Position.X, -_game.Camera.Position.Y, 0);
        _game.GraphicsDevice.Clear(BgColor);
        var transformMatrix = _game.Camera.GetViewMatrix();

        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

        Pickupables.ForEach(pickupable => pickupable.Draw(_game.SpriteBatch));

        _companion.Draw(_game.SpriteBatch);
        _player.Draw(_game.SpriteBatch);

        _testCampFire.Draw(_game.SpriteBatch);  // TEMP

        _game.SpriteBatch.End();
        _game.Penumbra.Draw(gameTime);

        DrawUI(gameTime);
    }

    private void DrawUI(GameTime gameTime)
    {
        var transformMatrix = _game.Camera.GetViewMatrix();
        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        _player.DrawPopupButton(_game.SpriteBatch);
        _testCampFire.DrawUI(_game.SpriteBatch);  // TEMP
        _game.SpriteBatch.End();

        _game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _followerAttributes.Draw(_game.SpriteBatch);
        if (gameTime.TotalGameTime.Subtract(_fpsCounterGameTime).Milliseconds >= 500)
        {
            _fps = (int)(1 / gameTime.ElapsedGameTime.TotalSeconds);
            _fpsCounterGameTime = gameTime.TotalGameTime;
        }

        if (_pauseState)
        {
            _pauseButton.Draw(_game.SpriteBatch);
            _menuButton.Draw(_game.SpriteBatch);
        }

        if (_showDebug)
        {
            _game.SpriteBatch.DrawString(_game.FontMap["16"], $"FPS: {_fps}",
                new Vector2(16, 16), Color.Black);
        }

        _game.SpriteBatch.End();
    }
}