using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace PencilChiselCode.Source;

public class Attribute
{
    private Vector2 _position;
    private float _scale;
    private Vector2 _size;
    private readonly Texture2D _texture;
    private readonly Texture2D _filledTexture;
    private Vector2 _offset;
    private Color _color;
    public float MaxValue;
    public float ChangeRate;

    public float Value
    {
        get => _value;
        set => _value = Math.Clamp(value, 0, MaxValue);
    }

    private float _value;

    public Attribute(
        Vector2 position,
        float scale,
        Texture2D baseTexture,
        Texture2D filledTexture,
        Vector2 offset,
        Vector2 size,
        Color color,
        float maxValue,
        float value,
        float changeRate
    )
    {
        _position = position;
        _scale = scale;
        _size = size;
        _texture = baseTexture;
        _filledTexture = filledTexture;
        _offset = offset;
        _color = color;
        MaxValue = maxValue;
        Value = value;
        ChangeRate = changeRate;
    }

    public static Attribute operator +(Attribute a, float b)
    {
        a.Value += b;
        return a;
    }

    public static Attribute operator -(Attribute a, float b)
    {
        a.Value -= b;
        return a;
    }

    public static bool operator >(Attribute a, float b) => a.Value > b;
    public static bool operator <(Attribute a, float b) => a.Value < b;
    public static bool operator >=(Attribute a, float b) => a.Value >= b;
    public static bool operator <=(Attribute a, float b) => a.Value <= b;

    public float Percent() => Value / MaxValue;
    public bool IsEmpty() => Value <= 0;

    public void Update(GameTime gameTime) => Value += ChangeRate * gameTime.GetElapsedSeconds();

    public void Draw(SpriteBatch spriteBatch)
    {
        if (_texture != null)
        {
            spriteBatch.Draw(_texture, _position, null, _color, 0F, _offset, _scale, SpriteEffects.None, 0F);
            spriteBatch.Draw(_filledTexture, _position,
                new Rectangle(0, 0, (int)(_texture.Width * (Value / MaxValue)), _texture.Height), _color, 0F,
                _offset, _scale, SpriteEffects.None, 0F);
        }
        else
        {
            spriteBatch.FillRectangle(_position, _size, Color.Black);
            spriteBatch.FillRectangle(_position, new(_size.X * (Value / MaxValue), _size.Y), _color);
        }
    }

    public static AttributeBuilder Builder => new();

    public class AttributeBuilder
    {
        private Vector2 _position;
        private Vector2 _size;
        private Color _color;
        private float _scale;
        private Texture2D _baseTexture;
        private Texture2D _filledTexture;
        private Vector2 _offset;
        private float _maxValue;
        private float _value;
        private float _changeRate;

        internal AttributeBuilder()
        {
            _baseTexture = null;
            _filledTexture = null;
            _size = new Vector2(100, 10);
            _color = Color.White;
            _changeRate = 0;
            _scale = 1F;
            _offset = Vector2.Zero;
            _maxValue = 100;
            _value = 100;
            _changeRate = 0F;
            _position = Vector2.Zero;
        }

        public AttributeBuilder WithPosition(Vector2 position)
        {
            _position = position;
            return this;
        }

        public AttributeBuilder WithSizeAndColor(Vector2 size, Color color)
        {
            _size = size;
            _color = color;
            return this;
        }

        public AttributeBuilder WithScale(float scale)
        {
            _scale = scale;
            return this;
        }

        public AttributeBuilder WithTextures(Texture2D baseTexture, Texture2D filledTexture)
        {
            _baseTexture = baseTexture;
            _filledTexture = filledTexture;
            _size = new Vector2(baseTexture.Width, baseTexture.Height);
            return this;
        }

        public AttributeBuilder WithOffset(Vector2 offset)
        {
            _offset = offset;
            return this;
        }

        public AttributeBuilder WithMaxValue(float maxValue)
        {
            _maxValue = maxValue;
            return this;
        }

        public AttributeBuilder WithValue(float value)
        {
            _value = value;
            return this;
        }

        public AttributeBuilder WithChangeRate(float changeRate)
        {
            _changeRate = changeRate;
            return this;
        }
        public Attribute Build() => new(_position, _scale, _baseTexture, _filledTexture, _offset, _size * _scale, _color, _maxValue, _value, _changeRate);
    }
}