using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended;
using Penumbra;
using System;
using MonoGame.Extended.Screens;

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
        private AnimatedSprite _animatedSprite;
        private readonly float _maxScale = 300F;
        private ParticleGenerator _particleGenerator;

        public CampFire(Game1 game, Vector2 position)
        {
            _game = game;
            Position = position;
            _animatedSprite = new AnimatedSprite(_game.SpriteSheetMap["fire"]);
            _animatedSprite.Play("burn");
            var attributeTexture = _game.TextureMap["attribute_bar"];
            var fireplaceAttributeTexture = _game.TextureMap["fireplace_bar"];
            _attribute = new Attribute(
                Position + new Vector2(_animatedSprite.TextureRegion.Width,
                    _animatedSprite.TextureRegion.Height * 3 + attributeTexture.Height),
                0.5F,
                attributeTexture,
                fireplaceAttributeTexture,
                attributeTexture.Bounds.Center.ToVector2() + new Vector2(30, 5),
                100F,
                -5F
            );
            // _game.Penumbra.Lights.Add(PointLight);
            _game.Penumbra.Lights.Add(PointLight);
            _particleGenerator = new ParticleGenerator(
                (() => new Particle(
                    Utils.RANDOM.NextSingle(1, 2),
                    Position + Vector2.UnitY * Utils.RANDOM.NextSingle(0, -10) + Vector2.UnitX * Utils.RANDOM.NextSingle(-5, 5),
                    Vector2.UnitY * Utils.RANDOM.NextSingle(0, -10) - Vector2.UnitX * Utils.RANDOM.NextSingle(-5, 5),
                    ((time) => time),
                    ((time) => IsLow() ? Color.Black : Color.Red)
                )),
                5F
            );
        }

        public bool IsLit()
        {
            return !_attribute.IsEmpty();
        }

        public bool IsLow()
        {
            return _attribute.Percent() < 0.1;
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

        public bool IsInRange(Vector2 sourcePosition)
        {
            var distance = Vector2.Distance(Position, sourcePosition);
            return distance < Scale();
        }

        public void FeedFire(float amount)
        {
            _game.SoundMap["fuel_fire"].Play();
            _attribute.Value += amount;
        }

        public void Update(GameTime gameTime)
        {
            _animatedSprite.Update(gameTime);
            _attribute.Update(gameTime);
            PointLight.Scale = new(_maxScale * (float) Math.Sqrt(_attribute.Percent()));
            _particleGenerator.Update(gameTime, !IsLow());
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
            _particleGenerator.Draw(spriteBatch);
        }

        public void DrawUI(SpriteBatch spriteBatch)
        {
            _attribute.Draw(spriteBatch);
        }
    }
}