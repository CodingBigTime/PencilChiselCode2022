using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;

namespace PencilChiselCode.Source.GameStates;

public class IngameState : GameScreen
{
    private static readonly Color _bgColor = Color.Green;
    private Game1 _game => (Game1)Game;
    private Player _player;
    private bool _showDebug;
    private readonly HashSet<Keys> _previousPressedKeys = new();
    private static float _cameraSpeed = 10.0F;
    private AttributeGroup _followerAttributes;
    private int _fps;
    private TimeSpan _fpsCounterGameTime;
    private static bool _pauseState;
    private Button _pauseButton;

    public IngameState(Game game) : base(game)
    {
    }

    public List<Pickupable> Pickupables { get; } = new();

    public override void LoadContent()
    {
        base.LoadContent();
        _pauseButton = new Button(_game.TextureMap["start_button_normal"],
            _game.TextureMap["start_button_hover"],
            _game.TextureMap["start_button_pressed"],
            () => { _pauseState = false; }
        );
        Pickupables.Add(new Pickupable(PickupableTypes.Twig, _game.TextureMap["twigs"],
            _game.SoundMap["pickup_branches"], new Vector2(300, 300),
            0.5F));
        _player = new Player(_game, new Vector2(150, 150));
        _game.Penumbra.Lights.Add(_player.PointLight);
        _game.Penumbra.Lights.Add(_player.Spotlight);
        _followerAttributes = new AttributeGroup(new List<Attribute>
        {
            new(new Vector2(10, 10), null, Color.Brown, 100, -0.5F),
            new(new Vector2(10, 30), null, Color.LightBlue, 100, -1F),
            new(new Vector2(10, 50), null, Color.Orange, 100, -5F),
            new(new Vector2(10, 70), null, Color.Blue, 100, -2F)
        });
    }

    public override void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.P) && !_previousPressedKeys.Contains(Keys.P))
        {
            _pauseState = !_pauseState;
        }

        if (_pauseState)
        {
            _pauseButton.Update(gameTime);
        }
        else
        {
            _game.Camera.Move(Vector2.UnitX * _cameraSpeed * gameTime.GetElapsedSeconds());
            _player.Update(this, gameTime);
            _followerAttributes.Update(gameTime);
            Pickupables.ForEach(pickupable => pickupable.Update(gameTime));
        }

        if (keyState.IsKeyDown(Keys.F3) && !_previousPressedKeys.Contains(Keys.F3))
        {
            _showDebug = !_showDebug;
        }

        _previousPressedKeys.Clear();
        _previousPressedKeys.UnionWith(keyState.GetPressedKeys());
    }

    public override void Draw(GameTime gameTime)
    {
        _game.Penumbra.BeginDraw();

        _game.Penumbra.Transform = Matrix.CreateTranslation(-_game.Camera.Position.X, -_game.Camera.Position.Y, 0);
        _game.GraphicsDevice.Clear(_bgColor);
        var transformMatrix = _game.Camera.GetViewMatrix();
        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        Pickupables.ForEach(pickupable => pickupable.Draw(_game.SpriteBatch));


        _player.Draw(_game.SpriteBatch);

        _game.SpriteBatch.End();

        _game.Penumbra.Draw(gameTime);

        DrawUI(gameTime);
    }

    private void DrawUI(GameTime gameTime)
    {
        var transformMatrix = _game.Camera.GetViewMatrix();
        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        _player.DrawPopupButton(_game.SpriteBatch);
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
        }

        if (_showDebug)
        {
            _game.SpriteBatch.DrawString(_game.FontMap["16"], $"FPS: {_fps}",
                new Vector2(16, 16), Color.Black);
        }

        _game.SpriteBatch.End();
    }
}