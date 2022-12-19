using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using PencilChiselCode.Source.GameStates;
using PencilChiselCode.Source.Objects;
using Penumbra;

namespace PencilChiselCode.Source;

public class CampFire : GroundEntity
{
    public override Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            PointLight.Position = new Vector2(value.X, value.Y);
        }
    }

    private Vector2 _position;
    private readonly Attribute _attribute;
    private readonly AnimatedSprite _animatedSprite;
    private readonly float _maxScale = 300F;
    private readonly ParticleGenerator _particleGenerator;
    public const uint TwigCost = 10U;

    public CampFire(IngameState state, Vector2 position) : base(state, null, position, new(1F), 0F)
    {
        Position = position;
        _animatedSprite = new AnimatedSprite(Game.SpriteSheetMap["fire"]);
        _animatedSprite.Play("burn");
        var attributeTexture = Game.TextureMap["attribute_bar"];
        var fireplaceAttributeTexture = Game.TextureMap["fireplace_bar"];
        _attribute = Attribute.Builder
            .WithPosition(
                Position
                    + new Vector2(
                        _animatedSprite.TextureRegion.Width,
                        _animatedSprite.TextureRegion.Height * 3 + attributeTexture.Height
                    )
            )
            .WithTextures(attributeTexture, fireplaceAttributeTexture)
            .WithScale(0.5F)
            .WithOffset(attributeTexture.Bounds.Center.ToVector2() + new Vector2(30, 5))
            .WithChangeRate(-10F)
            .Build();
        Game.Penumbra.Lights.Add(PointLight);
        _particleGenerator = new ParticleGenerator(
            () =>
                new Particle(
                    Utils.Random.NextSingle(1, 4),
                    Position
                        + Vector2.UnitY * Utils.Random.NextSingle(0, -10)
                        + Vector2.UnitX * Utils.Random.NextSingle(-5, 5),
                    Vector2.UnitY * Utils.Random.NextSingle(0, -10)
                        - Vector2.UnitX * Utils.Random.NextSingle(-5, 5),
                    time => time,
                    _ => IsLow() ? Color.Black : Color.Red
                ),
            5F
        );
    }

    public override void Cleanup()
    {
        lock (Game.Penumbra)
        {
            Game.Penumbra.Lights.Remove(PointLight);
        }
    }

    public bool IsLit() => !_attribute.IsEmpty();

    public override bool ShouldRemove() => base.ShouldRemove() || !IsLit();

    public bool IsLow() => _attribute.Percent() < 0.1;

    public Light PointLight { get; } =
        new PointLight { Color = Color.DarkOrange, ShadowType = ShadowType.Occluded };

    public override Vector2 Scale => new(_maxScale * _attribute.Percent());

    public bool IsInRange(Vector2 sourcePosition) =>
        Vector2.Distance(Position, sourcePosition) < Scale.X;

    public void FeedFire(float amount)
    {
        Game.SoundMap["fuel_fire"].Play();
        _attribute.Value += amount;
    }

    public override void Update(GameTime gameTime)
    {
        if (ShouldRemove())
            return;

        _animatedSprite.Update(gameTime);
        _attribute.Update(gameTime);
        PointLight.Scale = new(_maxScale * (float)Math.Sqrt(_attribute.Percent()));
        _particleGenerator.Update(gameTime, !IsLow());
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (ShouldRemove())
            return;
        var textureScale = _attribute.Percent() * 4F;
        _animatedSprite.Draw(spriteBatch, Position, 0F, new(textureScale));
        _particleGenerator.Draw(spriteBatch);
    }

    public void DrawUI(SpriteBatch spriteBatch)
    {
        if (ShouldRemove())
            return;
        _attribute.Draw(spriteBatch);
    }
}
