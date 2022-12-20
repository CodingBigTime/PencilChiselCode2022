using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace PencilChiselCode.Source.GUI;

public class AbsoluteBox : Box
{
    public Vector2 Position { get; set; }
    private readonly Func<Vector2> _size;
    public Vector2 Size => _size();

    public AbsoluteBox(Bonfire game, Vector2 position, Func<Vector2> size) : base(game)
    {
        Position = position;
        _size = size;
    }

    public AbsoluteBox(Bonfire game, Vector2 position, Vector2 size)
        : this(game, position, () => size) { }

    public AbsoluteBox AbsoluteFrom(RelativeBox relative)
    {
        var absolute = new AbsoluteBox(
            relative.Game,
            relative.Position.RelativeTo(Size),
            relative.Size.RelativeTo(Size)
        )
        {
            IsVisible = () => relative.IsVisible() && IsVisible(),
            DrawableElement = relative.DrawableElement,
            Children = relative.Children
        };
        absolute.Position = relative.BoxAlignment switch
        {
            Alignments.TopLeft => Position + absolute.Position,
            Alignments.TopCenter
                => Position + new Vector2(Size.X / 2 + absolute.Position.X, absolute.Position.Y),
            Alignments.TopRight
                => Position + new Vector2(Size.X + absolute.Position.X, absolute.Position.Y),
            Alignments.MiddleLeft
                => Position + new Vector2(absolute.Position.X, Size.Y / 2 + absolute.Position.Y),
            Alignments.MiddleCenter
                => Position
                    + new Vector2(
                        Size.X / 2 + absolute.Position.X,
                        Size.Y / 2 + absolute.Position.Y
                    ),
            Alignments.MiddleRight
                => Position
                    + new Vector2(Size.X + absolute.Position.X, Size.Y / 2 + absolute.Position.Y),
            Alignments.BottomLeft
                => Position + new Vector2(absolute.Position.X, Size.Y + absolute.Position.Y),
            Alignments.BottomCenter
                => Position
                    + new Vector2(Size.X / 2 + absolute.Position.X, Size.Y + absolute.Position.Y),
            Alignments.BottomRight
                => Position
                    + new Vector2(Size.X + absolute.Position.X, Size.Y + absolute.Position.Y),
            _ => absolute.Position
        };
        absolute.Position += relative.SelfAlignment switch
        {
            Alignments.TopLeft => Vector2.Zero,
            Alignments.TopCenter => new Vector2(-absolute.Size.X / 2, 0),
            Alignments.TopRight => new Vector2(-absolute.Size.X, 0),
            Alignments.MiddleLeft => new Vector2(0, -absolute.Size.Y / 2),
            Alignments.MiddleCenter => new Vector2(-absolute.Size.X / 2, -absolute.Size.Y / 2),
            Alignments.MiddleRight => new Vector2(-absolute.Size.X, -absolute.Size.Y / 2),
            Alignments.BottomLeft => new Vector2(0, -absolute.Size.Y),
            Alignments.BottomCenter => new Vector2(-absolute.Size.X / 2, -absolute.Size.Y),
            Alignments.BottomRight => new Vector2(-absolute.Size.X, -absolute.Size.Y),
            _ => Vector2.Zero
        };
        return absolute;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var visible = IsVisible();
        if (visible)
        {
            DrawableElement?.Draw(spriteBatch, this);
        }

        if (visible || Game.DebugMode == 2)
        {
            Children.ForEach(box => box.Draw(spriteBatch, this));
        }

        if (Game.DebugMode == 2 || (Game.DebugMode == 1 && visible))
        {
            spriteBatch.DrawRectangle(Position, Size, visible ? DebugColor : InvisibleDebugColor);
        }
    }

    public void Update(GameTime gameTime)
    {
        if (!IsVisible())
            return;
        DrawableElement?.Update(gameTime, this);
        Children.ForEach(box => box.Update(gameTime, this));
    }
}
