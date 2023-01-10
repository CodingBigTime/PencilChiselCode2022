using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source;

public class Companion
{
    public Vector2 Size;
    public Vector2 Position;
    private const float PI = (float)Math.PI;
    private readonly float _scale = 1F;
    private Vector2 _speed;
    private Vector2 _movementSpeed;
    private const float MinimumFollowDistance = 50F;
    private bool _isStationary;
    private readonly IngameState _state;
    private Bonfire Game => _state.Game;
    public readonly Attribute ComfyMeter;

    public Companion(IngameState state, Vector2 position, float speed)
    {
        _state = state;
        _isStationary = false;
        _movementSpeed.X = speed;
        _movementSpeed.Y = speed;
        Position = position;
        var textureCompanionDown = Game.TextureMap["follower"];
        Size = new Vector2(
            textureCompanionDown.Width * _scale,
            textureCompanionDown.Height * _scale
        );
        var attributeTexture = Game.TextureMap["attribute_bar"];
        var comfyAttributeTexture = Game.TextureMap["comfy_bar"];
        ComfyMeter = Attribute.Builder
            .WithPosition(
                new Vector2(
                    Game.GetWindowWidth() / 2F,
                    Game.GetWindowHeight() - attributeTexture.Height * 3F
                )
            )
            .WithTextures(attributeTexture, comfyAttributeTexture)
            .WithScale(3F)
            .WithOffset(attributeTexture.Bounds.Center.ToVector2())
            .WithChangeRate(gameTime =>
            {
                if (Game.DebugMode >= 1 || state.Daytime <= 0.2F)
                {
                    return 0F;
                }

                if (Position.X < _state.Camera.Position.X + IngameState.DarknessEndOffset)
                {
                    return -48F * state.Daytime * gameTime.GetElapsedSeconds();
                }

                if (_state.Campfires.Any(campfire => campfire.IsInRange(Position)))
                {
                    return 16F * gameTime.GetElapsedSeconds();
                }

                if (
                    Vector2.Distance(_state.Player.Position, Position)
                    > IngameState.MinimumFollowerPlayerDistance
                )
                {
                    return -8F * gameTime.GetElapsedSeconds();
                }

                return state.Daytime * -6F;
            })
            .Build();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var angle = (float)Math.Atan2(_speed.Y, _speed.X);
        var (texture, spriteEffect) = angle switch
        {
            >= -PI / 4
            and <= PI / 4
                => (Game.TextureMap["follower"], SpriteEffects.FlipHorizontally),
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
        ComfyMeter.Update(gameTime);

        var (playerPosX, playerPosY) = playerPosition;
        var delta = gameTime.GetElapsedSeconds();
        var width = Math.Abs(Position.X - playerPosX);
        var height = Math.Abs(Position.Y - playerPosY);
        var h = (float)Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));

        _movementSpeed.X =
            Math.Sqrt(width * width + height * height) > MinimumFollowDistance && !_isStationary
                ? _movementSpeed.Y
                : 0;

        if (playerPosX > Position.X)
        {
            Position.X += _movementSpeed.X * (width / h) * delta;
        }
        else if (playerPosX < Position.X)
        {
            Position.X -= _movementSpeed.X * (width / h) * delta;
        }

        if (playerPosY > Position.Y)
        {
            Position.Y += _movementSpeed.X * (height / h) * delta;
        }
        else if (playerPosY < Position.Y)
        {
            Position.Y -= _movementSpeed.X * (height / h) * delta;
        }

        if (Game.DebugMode >= 2)
        {
            ComfyMeter.Value = 100F;
        }
    }

    public bool IsAnxious() => ComfyMeter <= 0;

    public void ToggleSitting() => _isStationary = !_isStationary;
}
