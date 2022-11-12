using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source;

public class Companion
{
    public Vector2 Size;
    public Vector2 Position;
    public Vector2 PlayerPosition;
    private const float sqrt1_2 = 0.70710678118654752440084436210485F;
    private const float PI = (float)Math.PI;
    private readonly float _scale = 0.5F;
    private Game1 _game;
    private Vector2 _speed;
    private readonly float _maxSpeed = 80F;
    private readonly float _acceleration = 16F;
    private readonly float _friction = 0.95F;
    private uint _twigs = 0;
    
    public Companion(Game1 game, Vector2 position)
    {
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
        PlayerPosition = playerPosition;
        Position.X = PlayerPosition.X - 25;
        Position.Y = PlayerPosition.Y - 25;
        

    }
    
}