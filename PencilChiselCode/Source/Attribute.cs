using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace PencilChiselCode.Source
{
    public class Attribute
    {
        private Vector2 _position;
        private Vector2 _size;
        private Texture2D _texture;
        private Vector2 _offset;
        private Color _color;
        public readonly float MaxValue;
        public readonly float ChangeRate;
        public float Value
        {
            get { return value_; }
            set
            {
                value_ = Math.Clamp(value, 0, MaxValue);
            }
        }
        float value_;

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

        public Attribute(Vector2 position, Vector2 size, Texture2D texture, Vector2 offset, Color color, float maxValue, float changeRate = 0F)
        {
            _position = position;
            _size = size;
            _texture = texture;
            _color = color;
            MaxValue = maxValue;
            Value = maxValue;
            ChangeRate = changeRate;
        }

        public Attribute(Vector2 position, Vector2 size, Texture2D texture, Vector2 offset, Color color, float maxValue, float value, float changeRate = 0F)
        {
            _position = position;
            _size = size;
            _texture = texture;
            _offset = offset;
            _color = color;
            MaxValue = maxValue;
            Value = value;
            ChangeRate = changeRate;
        }



        public float ChangeValue(float value)
        {
            Value += value;

            return Value;
        }

        public float Percent() {
            return Value / MaxValue;
        }

        public bool IsEmpty()
        {
            return Value <= 0;
        }

        public void Update(GameTime gameTime)
        {
            ChangeValue(ChangeRate * gameTime.GetElapsedSeconds());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_position == null) return;
            if (_texture != null)
            {
                spriteBatch.Draw(_texture, _position, null, _color, 0F, _offset, _size, SpriteEffects.None, 0F);
            }
            spriteBatch.FillRectangle(_position, _size, Color.Black);
            spriteBatch.FillRectangle(_position, new(_size.X * (Value / MaxValue), _size.Y), _color);
        }
    }
}