using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source
{
    public class Icon
    {
        private Vector2 _position;
        private Texture2D _texture;
        private float _scale;
        private Vector2 _size;

        public Icon(Vector2 position, Texture2D texture, float scale = 1F)
        {
            _scale = scale;
            _position = position;
            _texture = texture;
            _size = new Vector2(texture.Width * _scale, texture.Height * _scale);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, Color.White, 0F, Vector2.Zero, _scale, SpriteEffects.None, 0F);
        }
    }
}