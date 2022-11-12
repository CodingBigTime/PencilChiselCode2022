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
    private HashSet<Keys> _previousPressedKeys = new();
    private static float _cameraSpeed = 10.0F;
    private AttributeGroup _followerAttributes;
    private static bool _pauseState;
    public Button PauseButton;

    public IngameState(Game game) : base(game)
    {
    }

    public List<Pickupable> Pickupables { get; } = new();

    public override void LoadContent()
    {
        base.LoadContent();
        PauseButton = new Button(Game1.Instance.TextureMap["start_button_normal"],
            Game1.Instance.TextureMap["start_button_hover"],
            Game1.Instance.TextureMap["start_button_pressed"],
            () =>
            {
                _pauseState = false;
            }
        );
        Pickupables.Add(new Pickupable(PickupableTypes.Twig, Game1.Instance.TextureMap["twigs"], new Vector2(300, 300),
            0.5F));
        _player = new Player(_game, new Vector2(150, 150));
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
            PauseButton.Update(gameTime);
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
        _game.GraphicsDevice.Clear(_bgColor);
        var transformMatrix = _game.Camera.GetViewMatrix();
        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        Pickupables.ForEach(pickupable => pickupable.Draw(Game1.Instance.SpriteBatch));


        _player.Draw(Game1.Instance.SpriteBatch);

        _game.SpriteBatch.End();
        DrawUI(gameTime);
    }

    private void DrawUI(GameTime gameTime)
    {
        _game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _followerAttributes.Draw(Game1.Instance.SpriteBatch);

        if (_pauseState)
        {
            PauseButton.Draw(Game1.Instance.SpriteBatch);
        }
        if (_showDebug)
        {
            _game.SpriteBatch.DrawString(_game.FontMap["16"], $"FPS: {1 / gameTime.ElapsedGameTime.TotalSeconds}",
                new Vector2(16, 16), Color.Black);
        }
        _game.SpriteBatch.End();
    }
}