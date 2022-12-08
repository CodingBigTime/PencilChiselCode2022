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
        Size = new(textureCompanionDown.Width * _scale, textureCompanionDown.Height * _scale);
        var attributeTexture = Game.TextureMap["attribute_bar"];
        var comfyAttributeTexture = Game.TextureMap["comfy_bar"];
        ComfyMeter =
            new Attribute(
                new Vector2(Game.GetWindowWidth() / 2F, Game.GetWindowHeight() - attributeTexture.Height * 3F),
                3F,
                attributeTexture,
                comfyAttributeTexture,
                attributeTexture.Bounds.Center.ToVector2(),
                100,
                -2F
            );
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
        ComfyMeter.Update(gameTime);

        var (playerPosX, playerPosY) = playerPosition;
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var width = Math.Abs(Position.X - playerPosX);
        var height = Math.Abs(Position.Y - playerPosY);
        var h = (float)Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));

        _movementSpeed.X = Math.Sqrt(width * width + height * height) > MinimumFollowDistance && !_isStationary
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

        var followerPlayerDistance = Vector2.Distance(_state.Player.Position, Position);

        if (Position.X < Game.Camera.Position.X + IngameState.DarknessEndOffset)
        {
            ComfyMeter.ChangeValue(-24F * gameTime.GetElapsedSeconds());
        }
        else if (_state.Campfires.Any(campfire => campfire.IsInRange(Position)))
        {
            ComfyMeter.ChangeValue(8F * gameTime.GetElapsedSeconds());
        }
        else if (followerPlayerDistance > IngameState.MinimumFollowerPlayerDistance)
        {
            ComfyMeter.ChangeValue(-4F * gameTime.GetElapsedSeconds());
        }

        if (
            !_state.Game.Controls.JustPressed(ControlKeys.FEED) ||
            followerPlayerDistance > 100 ||
            _state.Player.Inventory[PickupableTypes.Bush] < 1
        ) return;
        _state.Player.ReduceBerries(1);
        ComfyMeter.ChangeValue(10F);
    }

    public bool IsAnxious() => ComfyMeter.Value <= 0;
    public void ToggleSitting() => _isStationary = !_isStationary;
}