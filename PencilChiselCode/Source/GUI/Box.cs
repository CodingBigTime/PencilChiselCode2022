using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace PencilChiselCode.Source.GUI;

public class Box
{
    public readonly List<Box> Children = new();
    public IUiElement DrawableElement;
    public Vector2 Position { get; set; }
    public Func<Vector2> Size { get; set; }
    public Vector2 Scale { get; set; } = new(1F);
    public Vector2 AdjustedSize => Size() * Scale;
    public bool IsPositionAbsolute { get; set; }
    public bool IsSizeAbsolute { get; set; }
    public Alignments BoxAlignment { get; set; } = Alignments.TopLeft;
    public Alignments SelfAlignment { get; set; } = Alignments.TopLeft;
    public Func<bool> IsVisible { get; set; } = () => true;
    public Color DebugColor = Color.LightGreen;
    private readonly Bonfire _game;

    public Box(Bonfire game, Vector2 position, Func<Vector2> size)
    {
        _game = game;
        Position = position;
        Size = size;
    }
    public Box(Bonfire game, Vector2 position, Vector2 size)
    {
        _game = game;
        Position = position;
        Size = () => size;
    }

    public Box(Box parent, Box child)
    {
        _game = child._game;
        Position = child.Position * (child.IsPositionAbsolute ? new(1F) : parent.AdjustedSize);
        Size = child.IsSizeAbsolute ? child.Size : () => parent.Size() * child.Size();
        Children = child.Children;
        DrawableElement = child.DrawableElement;
        IsPositionAbsolute = child.IsPositionAbsolute;
        IsSizeAbsolute = child.IsSizeAbsolute;
        BoxAlignment = child.BoxAlignment;
        SelfAlignment = child.SelfAlignment;
        IsVisible = child.IsVisible;
        Scale = child.Scale;
    }

    public void AddChild(Box child) => Children.Add(child);

    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsVisible())
        {
            DrawableElement?.Draw(spriteBatch, this);
            Children.ForEach(box => box.Draw(spriteBatch, this));
        }

        if (_game.DebugMode)
        {
            spriteBatch.DrawRectangle(Position, AdjustedSize, DebugColor);
        }
    }

    public void Update(GameTime gameTime)
    {
        if (!IsVisible())
            return;
        DrawableElement?.Update(gameTime, this);
        Children.ForEach(box => box.Update(gameTime, this));
    }

    public void Draw(SpriteBatch spriteBatch, Box parent)
    {
        if (!IsVisible())
            return;
        GetAbsolute(parent).Draw(spriteBatch);
    }

    public void Update(GameTime gameTime, Box parent)
    {
        if (!IsVisible())
            return;
        GetAbsolute(parent).Update(gameTime);
    }

    public Box GetAbsolute(Box parent)
    {
        var abs = new Box(parent, this);
        abs.Position = BoxAlignment switch
        {
            Alignments.TopLeft => abs.Position,
            Alignments.TopCenter
                => parent.Position
                    + new Vector2(parent.AdjustedSize.X / 2 - abs.Position.X, abs.Position.Y),
            Alignments.TopRight
                => parent.Position
                    + new Vector2(parent.AdjustedSize.X - abs.Position.X, abs.Position.Y),
            Alignments.MiddleLeft
                => parent.Position
                    + new Vector2(abs.Position.X, parent.AdjustedSize.Y / 2 - abs.Position.Y),
            Alignments.MiddleCenter
                => parent.Position
                    + new Vector2(
                        parent.AdjustedSize.X / 2 - abs.Position.X,
                        parent.AdjustedSize.Y / 2 - abs.Position.Y
                    ),
            Alignments.MiddleRight
                => abs.Position
                    + new Vector2(
                        parent.AdjustedSize.X - abs.Position.X,
                        parent.AdjustedSize.Y / 2 - abs.Position.Y
                    ),
            Alignments.BottomLeft
                => abs.Position + new Vector2(0, parent.AdjustedSize.Y - abs.Position.Y),
            Alignments.BottomCenter
                => parent.Position
                    + new Vector2(
                        parent.AdjustedSize.X / 2 - abs.Position.X,
                        parent.AdjustedSize.Y - abs.Position.Y
                    ),
            Alignments.BottomRight
                => abs.Position
                    + new Vector2(
                        parent.AdjustedSize.X - abs.Position.X,
                        parent.AdjustedSize.Y - abs.Position.Y
                    ),
            _ => abs.Position
        };
        abs.Position += SelfAlignment switch
        {
            Alignments.TopLeft => Vector2.Zero,
            Alignments.TopCenter => new Vector2(-abs.AdjustedSize.X / 2, 0),
            Alignments.TopRight => new Vector2(-abs.AdjustedSize.X, 0),
            Alignments.MiddleLeft => new Vector2(0, -abs.AdjustedSize.Y / 2),
            Alignments.MiddleCenter
                => new Vector2(-abs.AdjustedSize.X / 2, -abs.AdjustedSize.Y / 2),
            Alignments.MiddleRight => new Vector2(-abs.AdjustedSize.X, -abs.AdjustedSize.Y / 2),
            Alignments.BottomLeft => new Vector2(0, abs.AdjustedSize.Y),
            Alignments.BottomCenter => new Vector2(-abs.AdjustedSize.X / 2, -abs.AdjustedSize.Y),
            Alignments.BottomRight => new Vector2(-abs.AdjustedSize.X, abs.AdjustedSize.Y),
            _ => Vector2.Zero
        };
        return abs;
    }
}
