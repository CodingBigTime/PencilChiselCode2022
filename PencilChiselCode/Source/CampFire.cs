using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;

namespace PencilChiselCode.Source
{
    public class CampFire
    {
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                PointLight.Position = new Vector2(value.X, value.Y);
            }
        }

        private Vector2 _position;
        private Game1 _game;
        private Attribute _attribute;
        private readonly float _maxScale = 300F;

        public CampFire(Game1 game, Vector2 position)
        {
            _game = game;
            Position = position;
            _attribute = new Attribute(
                Position + Vector2.UnitY * 32 - Vector2.UnitX * 15,
                new(30, 5),
                Color.Red,
                100F,
                -5F
            );
        }

        public bool Lit()
        {
            return !_attribute.IsEmpty();
        }

        public Light PointLight { get; } = new PointLight
        {
            Color = Color.White,
            ShadowType = ShadowType.Occluded
        };

        public float Scale()
        {
            return _maxScale * _attribute.Percent();
        }

        public bool IsInRange(Vector2 sourcePosition)
        {
            var distance = Vector2.Distance(Position, sourcePosition);
            return distance < Scale();
        }

        public void Update(GameTime gameTime)
        {
            _attribute.Update(gameTime);
            PointLight.Scale = new(_maxScale * _attribute.Percent());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var texture = _game.TextureMap["fire_01"];
            var textureScale = _attribute.Percent() * 4F;
            spriteBatch.Draw(
                texture: texture,
                position: Position - new Vector2(texture.Width, texture.Height) * textureScale / 2,
                sourceRectangle: null,
                color: Color.White,
                rotation: 0,
                origin: Vector2.Zero,
                scale: textureScale,
                effects: SpriteEffects.None,
                layerDepth: 0
            );
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            _attribute.Draw(spriteBatch);
        }
    }
}