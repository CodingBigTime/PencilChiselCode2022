using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source.Objects;

public abstract class GroundEntity
{
    protected readonly IngameState State;
    protected Bonfire Game => State.Game;
    public Texture2D Texture { get; set; }
    public Vector2 Size => new(Texture.Width, Texture.Height);
    public virtual Vector2 Position { get; set; }
    public float Rotation { get; set; }
    public virtual Vector2 Scale { get; set; }
    protected bool MarkedForRemoval;

    public virtual bool ShouldRemove() => MarkedForRemoval;

    public void MarkForRemoval() => MarkedForRemoval = true;

    public GroundEntity(
        IngameState state,
        Texture2D texture,
        Vector2 position,
        Vector2 scale,
        float rotation
    )
    {
        State = state;
        Texture = texture;
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }

    public virtual void Cleanup() { }

    public virtual void Update(GameTime gameTime) { }

    public virtual void Draw(SpriteBatch spriteBatch) =>
        spriteBatch.Draw(
            Texture,
            Position,
            null,
            Color.White,
            Rotation,
            Size / 2,
            Scale,
            SpriteEffects.None,
            0
        );

    public bool Intersects(Vector2 position, Vector2 size) =>
        Utils.Intersects(
            new Rectangle(Position.ToPoint(), Size.ToPoint()),
            new Rectangle(position.ToPoint(), size.ToPoint())
        );

    public static IComparer<GroundEntity> YComparer { get; } =
        Comparer<GroundEntity>.Create((x, y) => x.Position.Y.CompareTo(y.Position.Y));
}
