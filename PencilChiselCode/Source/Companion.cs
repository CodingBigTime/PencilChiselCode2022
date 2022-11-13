using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source;

public class Companion
{
    public Vector2 Size;
    public Vector2 Position;
    private const float sqrt1_2 = 0.70710678118654752440084436210485F;
    private const float PI = (float)Math.PI;
    private readonly float _scale = 1F;
    private Game1 _game;
    private Vector2 _speed;
    private Vector2 _movement_speed;
    private readonly float _maxSpeed = 80F;
    private readonly float _acceleration = 16F;
    private readonly float _friction = 0.95F;
    private uint _twigs = 0;
    private float _minimumDistance = 50F;
    private Boolean _isAFK;

    public Companion(Game1 game, Vector2 position,float speed)
    {
        _isAFK = false;
        _movement_speed.X = speed;
        _movement_speed.Y = speed;
        _game = game;
        Position = position;
        var textureCompanionDown = _game.TextureMap["follower"];
        Size = new(textureCompanionDown.Width * _scale , textureCompanionDown.Height * _scale);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float angle = (float)Math.Atan2(_speed.Y, _speed.X);
        var (texture, spriteEffect) = angle switch {
            >= -PI / 4 and <= PI / 4 => (_game.TextureMap["follower"], SpriteEffects.FlipHorizontally),
            > PI / 4 and < 3 * PI / 4 => (_game.TextureMap["follower"], SpriteEffects.None),
            >= 3 * PI / 4 or <= -3 * PI / 4 => (_game.TextureMap["follower"], SpriteEffects.None),
            > -3 * PI / 4 and < -PI / 4 => (_game.TextureMap["follower"], SpriteEffects.None),
            _ => (_game.TextureMap["follower"], SpriteEffects.None)
        };
        spriteBatch.Draw(
            texture: texture,
            position: Position - new Vector2(Size.X / 2F, Size.Y / 2),
            sourceRectangle: null,
            color: Color.White,
            rotation: 0,
            origin: new(0, 0),
            scale: _scale,
            effects: spriteEffect,
            layerDepth: 0
        );
    }

    public void Update(IngameState state,GameTime gameTime,Vector2 playerPosition)
    {

        var (playerPosX, playerPosY) = playerPosition;
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var width = Math.Abs(Position.X - playerPosX);
        var height = Math.Abs(Position.Y - playerPosY);
        var h = (float) Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));

        if (Math.Sqrt(width * width + height * height) > _minimumDistance && !_isAFK)
        {
            _movement_speed.X = _movement_speed.Y;
        }
        else
        {
            _movement_speed.X = 0;
        }
        if (playerPosX > Position.X)
        {
            Position.X += _movement_speed.X * (width / h) * delta;
        }
        else if (playerPosX < Position.X)
        {
            Position.X -= _movement_speed.X * (width / h) * delta;
        }
        if (playerPosY > Position.Y)
        {
            Position.Y += _movement_speed.X * (height / h) * delta;
        }
        else if (playerPosY < Position.Y)
        {
            Position.Y -= _movement_speed.X * (height / h) * delta;
        }
    }

    public void StopResumeFollower()
    {
        _isAFK = !_isAFK;
    }
}