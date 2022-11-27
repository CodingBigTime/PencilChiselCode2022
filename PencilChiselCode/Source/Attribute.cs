using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace PencilChiselCode.Source
{
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

        public Attribute(Vector2 position, Color color, float maxValue, float changeRate = 0F)
        {
            _position = position;
            _size = new(100, 10);
            _texture = null;
            _color = color;
            MaxValue = maxValue;
            Value = maxValue;
            ChangeRate = changeRate;
        }

        public Attribute(Vector2 position, Vector2 size, Color color, float maxValue, float changeRate = 0F)
        {
            _position = position;
            _size = size;
            _texture = null;
            _color = color;
            MaxValue = maxValue;
            Value = maxValue;
            ChangeRate = changeRate;
        }

        public Attribute(Vector2 position, float scale, Texture2D baseTexture, Texture2D filledTexture, Vector2 offset,
            float maxValue, float changeRate = 0F)
        {
            _position = position;
            _scale = scale;
            _texture = baseTexture;
            _filledTexture = filledTexture;
            _size = new Vector2(baseTexture.Width, baseTexture.Height) * scale;
            _color = Color.White;
            MaxValue = maxValue;
            Value = maxValue;
            _offset = offset;
            ChangeRate = changeRate;
        }

        public Attribute(Vector2 position, float scale, Texture2D baseTexture, Texture2D filledTexture, Vector2 offset,
            float maxValue, float value,
            float changeRate = 0F)
        {
            _position = position;
            _scale = scale;
            _size = new Vector2(baseTexture.Width, baseTexture.Height) * scale;
            _texture = baseTexture;
            _filledTexture = filledTexture;
            _offset = offset;
            _color = Color.White;
            MaxValue = maxValue;
            Value = value;
            ChangeRate = changeRate;
        }


        public float ChangeValue(float value)
        {
            Value += value;

            return Value;
        }

        public float Percent() => Value / MaxValue;
        public bool IsEmpty() => Value <= 0;

        public void Update(GameTime gameTime)
        {
            ChangeValue(ChangeRate * gameTime.GetElapsedSeconds());
        }

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
    }
}