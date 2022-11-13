﻿using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
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
    private const float Sqrt12 = 0.70710678118654752440084436210485F;
    private const float PI = (float)Math.PI;
    private Game1 _game;
    private AnimatedSprite _animatedSprite;
    private Vector2 _speed;
    private static readonly int _lightScale = 200;
    private static readonly float _scale = 2F;
    private static readonly float _maxSpeed = 80F;
    private static readonly float _acceleration = 1000F;
    private static readonly float _friction = 2.75F;
    private uint _twigs;
    private PopupButton _popupButton;

    public Player(Game1 game, Vector2 position)
    {
        _game = game;
        Position = position;
        _animatedSprite = new AnimatedSprite(_game.SpriteSheetMap["player"]);
        _animatedSprite.Play("right");
        Size = new(_animatedSprite.TextureRegion.Width * _scale, _animatedSprite.TextureRegion.Height * _scale);
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
        _animatedSprite.Play(
            angle switch 
            {
                >= -PI / 4 and <= PI / 4 => "right",
                > PI / 4 and < 3 * PI / 4 => "down",
                >= 3 * PI / 4 or <= -3 * PI / 4 => "left",
                > -3 * PI / 4 and < -PI / 4 => "up",
                _ => "right"
            }
        );
        _animatedSprite.Draw(
            spriteBatch: spriteBatch, 
            position: Position,
            rotation: 0,
            scale: new(_scale)
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
        _animatedSprite.Update(gameTime);

        if (mx != 0 && my != 0)
        {
            // Normalize the diagonal movement
            mx *= Sqrt12;
            my *= Sqrt12;
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

        var nearestCampfire = state.Campfires
            .OrderBy(campfire => Vector2.DistanceSquared(campfire.Position, Position))
            .FirstOrDefault(campfire => Vector2.DistanceSquared(campfire.Position, Position) < 100 * 100);
        if (nearestCampfire != null)
        {
            _popupButton ??= new PopupButton(_game, _game.TextureMap["f_button"]);
        }
        if (!state.PreviousPressedKeys.Contains(Keys.F) && keyState.IsKeyDown(Keys.F) && nearestCampfire != null && _twigs > 0)
        {
            nearestCampfire.FeedFire(10F);
            --_twigs;
        }

        var nearestPickupable = state.Pickupables
            .OrderBy(pickupable => Vector2.DistanceSquared(pickupable.Position, Position))
            .FirstOrDefault(pickupable => Vector2.DistanceSquared(pickupable.Position, Position) < 100 * 100);
        if (nearestPickupable != null)
        {
            _popupButton ??= new PopupButton(_game, _game.TextureMap["e_button"]);
        }
        _popupButton?.Update(state, gameTime);

        if (!state.PreviousPressedKeys.Contains(Keys.E) && keyState.IsKeyDown(Keys.E) && nearestPickupable != null)
        {
            switch (nearestPickupable.Type)
            {
                case PickupableTypes.Twig:
                    ++_twigs;
                    break;
            }

            nearestPickupable.PickupSound.Play();
            state.Pickupables.Remove(nearestPickupable);
        }
        if (nearestPickupable == null && (nearestCampfire == null || _twigs <= 0))
        {
            _popupButton = null;
        }
    }
}