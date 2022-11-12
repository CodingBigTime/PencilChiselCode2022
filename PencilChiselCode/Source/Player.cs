using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PencilChiselCode.Source.GameStates;
using Penumbra;

namespace PencilChiselCode.Source;

public class Player
{
    public Vector2 Size;
    public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                PointLight.Position = new Vector2(value.X, value.Y);
                Spotlight.Position = new Vector2(value.X, value.Y);
            }
        }
        Vector2 position;
    private const float sqrt1_2 = 0.70710678118654752440084436210485F;
    private const float PI = (float)Math.PI;
    private Game1 _game;
    private Vector2 _speed;
    private readonly static int _lightScale = 200;
    private readonly static float _scale = 2F;
    private readonly static float _maxSpeed = 80F;
    private readonly static float _acceleration = 1000F;
    private readonly static float _friction = 2.75F;
    private uint _twigs = 0;
    private PopupButton _popupButton;

    public Player(Game1 game, Vector2 position)
    {
        _game = game;
        Position = position;
        _popupButton = new(game);
        var texturePlayerDown = _game.TextureMap["player_down"];
        Size = new(texturePlayerDown.Width * _scale, texturePlayerDown.Height * _scale);
    }

    public Light PointLight { get; } = new PointLight
    {
        Scale = new Vector2(_lightScale),
        Color = Color.White,
        ShadowType = ShadowType.Occluded
    };

    public Light Spotlight { get; } = new Spotlight
    {
        Scale = new Vector2(_lightScale * 2),
        Color = Color.White,
        ShadowType = ShadowType.Occluded,
        ConeDecay = 2.5F
    };

    public void Draw(SpriteBatch spriteBatch)
    {
        var angle = (float)Math.Atan2(_speed.Y, _speed.X);
        var (texture, spriteEffect) = angle switch {
            >= -PI / 4 and <= PI / 4 => (_game.TextureMap["player_left"], SpriteEffects.FlipHorizontally),
            > PI / 4 and < 3 * PI / 4 => (_game.TextureMap["player_down"], SpriteEffects.None),
            >= 3 * PI / 4 or <= -3 * PI / 4 => (_game.TextureMap["player_left"], SpriteEffects.None),
            > -3 * PI / 4 and < -PI / 4 => (_game.TextureMap["player_up"], SpriteEffects.None),
            _ => (_game.TextureMap["player_down"], SpriteEffects.None)
        };
        spriteBatch.Draw(
            texture: texture, 
            position: Position - Size/2,
            sourceRectangle: null,
            color: Color.White,
            rotation: 0,
            origin: Vector2.Zero,
            scale: _scale,
            effects: spriteEffect,
            layerDepth: 0
        );
    }

    public void DrawPopupButton(SpriteBatch spriteBatch)
    {
        _popupButton?.Draw(spriteBatch, Position, Size);
    }

    public void Update(IngameState state, GameTime gameTime)
    {
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var keyState = Keyboard.GetState();
        var left = keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left);
        var up = keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up);
        var down = keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down);
        var right = keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right);
        var mx = Convert.ToSingle(right) - Convert.ToSingle(left);
        var my = Convert.ToSingle(down) - Convert.ToSingle(up);
        var angle = (float)Math.Atan2(_speed.Y, _speed.X);
        
        Spotlight.Rotation = angle;

        if (mx != 0 && my != 0)
        {
            // Normalize the diagonal movement
            mx *= sqrt1_2;
            my *= sqrt1_2;
        }

        var ax = _acceleration * mx * delta;
        var ay = _acceleration * my * delta;

        _speed += new Vector2(ax, ay);
        _speed += -(_speed * _friction * delta);
        var biasX = Math.Abs(_speed.X) / (float) Math.Sqrt(_speed.X * _speed.X + _speed.Y * _speed.Y);
        var biasY = Math.Abs(_speed.Y) / (float) Math.Sqrt(_speed.X * _speed.X + _speed.Y * _speed.Y);
        _speed.X = Math.Clamp(_speed.X, -_maxSpeed * biasX, _maxSpeed * biasX);
        _speed.Y = Math.Clamp(_speed.Y, -_maxSpeed * biasY, _maxSpeed * biasY);
        Position = new(Position.X + _speed.X * delta, Position.Y + _speed.Y * delta);

        if (Position.X >= _game.Camera.Center.X + _game.Width / 2F - Size.X / 2F)
        {
            _speed.X = 0;
            Position = new(Math.Min(Position.X, _game.Camera.Center.X + _game.Width / 2F - Size.X / 2F), Position.Y);
        }

        if (Position.Y <= Size.Y / 2F || Position.Y >= _game.Height - Size.Y / 2F)
        {
            _speed.Y = 0;
            Position = new(Position.X, Math.Clamp(Position.Y, Size.Y / 2F, _game.Height - Size.Y / 2F));
        }


        var pickupable = state.Pickupables.Find(pickupable =>
            Utils.CreateCircle(Position, Size.GetAverageSize()).Expand(8)
                .Intersects(Utils.CreateCircle(pickupable.Position, pickupable.Size.GetAverageSize()).Expand(8)));
        var canPickup = pickupable != null;
        if (canPickup)
        {
            if (_popupButton == null)
            {
                _popupButton = new PopupButton(_game);
            }
            _popupButton.Update(state, gameTime);
        }
        else
        {
            _popupButton = null;
        }
        if (keyState.IsKeyDown(Keys.E))
        {
            if (!canPickup) return;
            switch (pickupable.Type)
            {
                case PickupableTypes.Twig:
                    ++_twigs;
                    break;
            }

            pickupable.PickupSound.Play();
            state.Pickupables.Remove(pickupable);
        }
    }
}