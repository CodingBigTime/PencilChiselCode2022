﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
namespace PencilChiselCode.Source;

public class GroundEntity
{
    public Texture2D Texture { get; set; }
    public Vector2 Size => new(Texture.Width, Texture.Height);

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            PointLight.Position = new Vector2(value.X, value.Y);
            Hull.Position = new Vector2(value.X, value.Y + Size.Y * 0.6F);
        }
    }

    private Vector2 _position;
    public float Rotation;
    private Vector2 _scale;
    private Bonfire _game;
    private Color _glow;
    private Color _color = Color.White;

    public GroundEntity(Bonfire game, Texture2D texture, Vector2 position, Vector2 scale,
        Color glow, float rotation = 0F)
    {
        _game = game;
        Texture = texture;
        Position = position;
        Rotation = rotation;
        _scale = scale;
        _game.Penumbra.Lights.Add(PointLight);
        PointLight.Color = glow;
    }

    public GroundEntity(Bonfire game, Texture2D texture, Vector2 position, Vector2 scale, float rotation = 0F)
    {
        _game = game;
        Texture = texture;
        Position = position;
        Rotation = rotation;
        _scale = scale;
        _game.Penumbra.Hulls.Add(Hull);
    }

    public Light PointLight { get; } = new PointLight
    {
        Scale = new(128),
        Radius = 1,
        ShadowType = ShadowType.Occluded
    };

    // public Hull Hull { get; } = Hull.CreateCircle(radius: 7);
    public Hull Hull { get; } = Hull.CreateRectangle(scale: new(8, 16));

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, _color, Rotation, Size / 2, _scale, SpriteEffects.None, 0);
    }

    public void Update(GameTime gameTime, Vector2 playerPosition)
    {
        if (Size.Y > 32 
            && playerPosition.Y < _position.Y + 32
            && playerPosition.Y > _position.Y - 64 
            && Math.Abs(playerPosition.X - _position.X) < 32) {
            _color.A = 150;
        } else {
            _color.A = 255;
        }
    }
}