using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source;

public class IngameState : GameScreen
{
    private static readonly Color _bgColor = Color.Green;
    private static float _cameraSpeed = 10.0F;
    private Game1 _game => (Game1)base.Game;
    private Player _player;
    private AttributeGroup _followerAttributes;
    private static Boolean _pauseState;
    public Button PauseButton;
    private Boolean _waspPressed = false;

    public IngameState(Game game) : base(game)
    {
    }

    public List<Pickupable> Pickupables { get; } = new();


    public override void LoadContent()
    {
        base.LoadContent();
        _pauseState = false;
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
        _followerAttributes = new AttributeGroup(new List<Attribute> {
            new Attribute(new Vector2(10, 10), null, Color.Brown, 100, -0.5F),
            new Attribute(new Vector2(10, 30), null, Color.LightBlue, 100, -1F),
            new Attribute(new Vector2(10, 50), null, Color.Orange, 100, -5F),
            new Attribute(new Vector2(10, 70), null, Color.Blue, 100, -2F)
        });
    }

    public override void Update(GameTime gameTime)
    {
        var keyState = Keyboard.GetState();
        if (keyState.IsKeyDown(Keys.P) && !_waspPressed)
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

        _waspPressed = keyState.IsKeyDown(Keys.P);
    }

    public override void Draw(GameTime gameTime)
    {
        _game.GraphicsDevice.Clear(_bgColor);
        var transformMatrix = _game.Camera.GetViewMatrix();
        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        Pickupables.ForEach(pickupable => pickupable.Draw(Game1.Instance.SpriteBatch));
        _player.Draw(Game1.Instance.SpriteBatch);
        _game.SpriteBatch.End();

        DrawUI();
    }

    private void DrawUI()
    {
        _game.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _followerAttributes.Draw(Game1.Instance.SpriteBatch);
        if (_pauseState)
        {
            PauseButton.Draw(Game1.Instance.SpriteBatch);
        }
        _game.SpriteBatch.End();
    }
}