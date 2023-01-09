using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class RelativeBox : Box
{
    public ScalarVector2 Position { get; set; }
    public ScalarVector2 Size { get; set; }
    public ScalarVector2 Padding { get; set; } = 0;
    public Index PreviousBoxIndex = ^1;

    public RelativeBox(Bonfire game) : base(game)
    {
        Position = 0;
        Size = 0F;
    }

    public RelativeBox(Bonfire game, ScalarVector2 position, ScalarVector2 size) : base(game)
    {
        Position = position;
        Size = size;
    }

    public RelativeBox(RelativeBox other) : base(other.Game)
    {
        Position = other.Position;
        Size = other.Size;
        IsVisible = other.IsVisible;
        DrawableElement = other.DrawableElement;
        Children = other.Children;
        BoxAlignment = other.BoxAlignment;
        SelfAlignment = other.SelfAlignment;
        Padding = other.Padding;
    }

    public RelativeBox WithChild(RelativeBox child)
    {
        AddChild(child);
        return this;
    }

    public RelativeBox WithChild(params RelativeBox[] children)
    {
        AddChild(children);
        return this;
    }

    public AbsoluteBox Draw(
        SpriteBatch spriteBatch,
        AbsoluteBox parent,
        AbsoluteBox previous = null
    )
    {
        var absoluteSelf = parent.AbsoluteFrom(this, previous);
        if (IsVisible() || Game.DebugMode == 2)
            absoluteSelf.Draw(spriteBatch);
        return absoluteSelf;
    }

    public virtual AbsoluteBox Update(
        GameTime gameTime,
        AbsoluteBox parent,
        AbsoluteBox previous = null
    )
    {
        var absoluteSelf = parent.AbsoluteFrom(this, previous);
        if (IsVisible())
            absoluteSelf.Update(gameTime);
        return absoluteSelf;
    }
}
