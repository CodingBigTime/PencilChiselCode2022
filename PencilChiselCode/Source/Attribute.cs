using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace PencilChiselCode.Source
{
    public class Attribute
    {
        private Vector2 _position;
        private Texture2D _texture;
        private Color _color;
        private float _maxValue { get; }
        private float _changeRate { get; }
        private float _value { get; set; }

        public Attribute(Vector2 position, Texture2D texture, Color color, float maxValue, float changeRate = 0F)
        {
            _position = position;
            _texture = texture;
            _color = color;
            _maxValue = maxValue;
            _value = maxValue;
            _changeRate = changeRate;
        }

        public Attribute(Vector2 position, Texture2D texture, Color color, float maxValue, float value, float changeRate = 0F)
        {
            _position = position;
            _texture = texture;
            _color = color;
            _maxValue = maxValue;
            _value = value;
            _changeRate = changeRate;
        }



        public float ChangeValue(float value)
        {
            _value += value;
            if (_value > _maxValue)
            {
                _value = _maxValue;
            }
            else if (_value < 0)
            {
                _value = 0;
            }

            return _value;
        }

        public bool IsEmpty()
        {
            return _value <= 0;
        }

        public void Update(GameTime gameTime)
        {
            ChangeValue(_changeRate * gameTime.GetElapsedSeconds());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // TODO: Add an attribute texture, align the rectangles with the new texture
            spriteBatch.FillRectangle(_position, new Vector2(100, 10), Color.Black);
            spriteBatch.FillRectangle(_position, new Vector2(100 * (_value / _maxValue), 10), _color);
        }
    }
}