using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Sprites;
using Penumbra;

namespace PencilChiselCode.Source
{
    public class CampFire
    {
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                PointLight.Position = new Vector2(value.X, value.Y);
            }
        }
        Vector2 position;

        private Game1 _game;
        private Attribute _attribute;
        private AnimatedSprite _animatedSprite;
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
            _animatedSprite = new MonoGame.Extended.Sprites.AnimatedSprite(_game.SpriteSheetMap["fire"]);
            _animatedSprite.Play("burn");
        }

        public bool Lit()
        {
            return !_attribute.IsEmpty();
        }

        public Light PointLight { get; } = new PointLight
        {
            Color = Color.DarkOrange,
            ShadowType = ShadowType.Occluded
        };

        public float Scale()
        {
            return _maxScale * _attribute.Percent();
        }

        public bool isInRange(Vector2 sourcePosition)
        {
            var distance = Vector2.Distance(Position, sourcePosition);
            return distance < Scale();
        }

        public void Update(GameTime gameTime)
        {
            _animatedSprite.Update(gameTime);
            _attribute.Update(gameTime);
            PointLight.Scale = new(_maxScale * _attribute.Percent());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            float textureScale = _attribute.Percent() * 4F;
            _animatedSprite.Draw(
                spriteBatch,
                Position,
                0F,
                new(textureScale)
            );
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            _attribute.Draw(spriteBatch);
        }
    }
}