using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source;

public class Companion
{
    public Vector2 Size;
    public Vector2 Position;
    private const float PI = (float)Math.PI;
    private readonly float _scale = 1F;
    private Vector2 _speed;
    private Vector2 _movement_speed;
    private const float _minimumDistance = 50F;
    private bool _isAFK;
    private readonly BonfireGameState _state;
    private Bonfire Game => _state.Game;

    public Companion(BonfireGameState state, Vector2 position, float speed)
    {
        _state = state;
        _isAFK = false;
        _movement_speed.X = speed;
        _movement_speed.Y = speed;
        Position = position;
        var textureCompanionDown = Game.TextureMap["follower"];
        Size = new(textureCompanionDown.Width * _scale, textureCompanionDown.Height * _scale);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float angle = (float)Math.Atan2(_speed.Y, _speed.X);
        var (texture, spriteEffect) = angle switch
        {
            >= -PI / 4 and <= PI / 4 => (Game.TextureMap["follower"], SpriteEffects.FlipHorizontally),
            > PI / 4 and < 3 * PI / 4 => (Game.TextureMap["follower"], SpriteEffects.None),
            >= 3 * PI / 4 or <= -3 * PI / 4 => (Game.TextureMap["follower"], SpriteEffects.None),
            > -3 * PI / 4 and < -PI / 4 => (Game.TextureMap["follower"], SpriteEffects.None),
            _ => (Game.TextureMap["follower"], SpriteEffects.None)
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

    public void Update(GameTime gameTime, Vector2 playerPosition)
    {
        var (playerPosX, playerPosY) = playerPosition;
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var width = Math.Abs(Position.X - playerPosX);
        var height = Math.Abs(Position.Y - playerPosY);
        var h = (float)Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));

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