using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tweening;
using PencilChiselCode.Source.GameStates;
using Penumbra;

namespace PencilChiselCode.Source.Objects;

public class Tree : GroundEntity
{
    private Vector2 _position;

    public override Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            PointLight.Position = new Vector2(value.X, value.Y);
            Hull.Position = new Vector2(value.X, value.Y + Size.Y * 0.6F);
        }
    }

    private Color _color = Color.White;
    private readonly Tweener _lightSizeTweener;
    private const float RenderOffset = 32;

    public override bool ShouldRemove() =>
        base.ShouldRemove() || Game.Camera.Position.X > Position.X + Size.X * Scale.X + RenderOffset;

    public Tree(
        IngameState state,
        Texture2D texture,
        Vector2 position,
        Vector2 scale,
        Color glow,
        float rotation = 0F
    ) : base(
        state,
        texture,
        position,
        scale,
        rotation
    )
    {
        Game.Penumbra.Lights.Add(PointLight);
        PointLight.Color = glow;
        _lightSizeTweener = new Tweener();
        _lightSizeTweener
            .TweenTo(
                target: PointLight,
                expression: pointLight => pointLight.Scale,
                toValue: new Vector2(196F),
                duration: 10F,
                delay: 0F
            )
            .RepeatForever(repeatDelay: 2f)
            .AutoReverse()
            .Easing(EasingFunctions.SineInOut);
        var repeat = Utils.RANDOM.Next(24);
        for (var i = 0; i < repeat; ++i)
        {
            _lightSizeTweener.Update(1F);
        }
    }

    public Tree(
        IngameState state,
        Texture2D texture,
        Vector2 position,
        Vector2 scale,
        float rotation = 0F
    ) : base(
        state,
        texture,
        position,
        scale,
        rotation
    )
    {
        Game.Penumbra.Hulls.Add(Hull);
    }

    public override void Cleanup()
    {
        lock (Game.Penumbra)
        {
            Game.Penumbra.Lights.Remove(PointLight);
            Game.Penumbra.Hulls.Remove(Hull);
        }
    }

    ~Tree() => Cleanup();

    public Light PointLight { get; } =
        new PointLight
        {
            Scale = new(64F),
            Radius = 1,
            ShadowType = ShadowType.Occluded
        };

    // TODO: Make the shape more dynamic
    public Hull Hull { get; } = Hull.CreateRectangle(scale: new(8, 16));

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (ShouldRemove()) return;

        spriteBatch.Draw(
            Texture,
            Position,
            null,
            _color,
            Rotation,
            Size / 2,
            Scale,
            SpriteEffects.None,
            0
        );
    }

    public override void Update(GameTime gameTime)
    {
        if (ShouldRemove())
        {
            Cleanup();
            return;
        }

        var playerPosition = State.Player.Position;
        _lightSizeTweener?.Update(gameTime.GetElapsedSeconds());
        _color.A = Size.Y > 32
                   && playerPosition.Y < _position.Y + 32
                   && playerPosition.Y > _position.Y - 64
                   && Math.Abs(playerPosition.X - _position.X) < 32
            ? (byte)150
            : (byte)255;
    }
}