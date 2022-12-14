using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace PencilChiselCode.Source.GUI;

public class AbsoluteBox : Box
{
    public Vector2 Position { get; set; }
    public Vector2 Size { get; set; }
    public Vector2 PaddedPosition { get; set; }
    public Vector2 PaddedSize { get; set; }

    public AbsoluteBox(Bonfire game, Vector2 position, Vector2 size) : base(game)
    {
        Position = position;
        Size = size;
        PaddedPosition = position;
        PaddedSize = size;
    }

    public AbsoluteBox(Bonfire game, ScalarVector2 position, ScalarVector2 size)
        : this(game, position.ToAbsoluteVector2Safe(), size.ToAbsoluteVector2Safe()) { }

    public AbsoluteBox(AbsoluteBox other) : base(other.Game)
    {
        Position = other.Position;
        Size = other.Size;
        IsVisible = other.IsVisible;
        DrawableElement = other.DrawableElement;
        Children = other.Children;
    }

    public AbsoluteBox AbsoluteFrom(RelativeBox child, AbsoluteBox previous = null)
    {
        var parentSizeX = new Pixels((int)PaddedSize.X);
        var parentSizeY = new Pixels((int)PaddedSize.Y);
        Pixels elementSizeX = new((int)child.DrawableElementSize.X);
        Pixels elementSizeY = new((int)child.DrawableElementSize.Y);
        Ratio elementSizeXRatio = elementSizeX / elementSizeY;
        Ratio elementSizeYRatio = elementSizeY / elementSizeX;

        var absoluteChild = new AbsoluteBox(
            Game,
            child.Position switch
            {
                { X: Pixels x, Y: Pixels y } => (x, y),
                { X: Percent x, Y: Percent y } => (parentSizeX * x, parentSizeY * y),
                { X: Percent x, Y: Pixels y } => (parentSizeX * x, y),
                { X: Pixels x, Y: Percent y } => (x, parentSizeY * y),
                { X: Ratio x, Y: Pixels y } => (y * x, y),
                { X: Pixels x, Y: Ratio y } => (x, x * y),
                { X: Ratio x, Y: Percent y } => (y * parentSizeY * x, y * parentSizeY),
                { X: Percent x, Y: Ratio y } => (x * parentSizeX, x * parentSizeX * y),
                { X: Ratio, Y: Ratio }
                    => throw new InvalidOperationException("Cannot have 2 ratios"),
                _
                    => throw new NotImplementedException(
                        $"Position scalar combination {child.Position} not implemented"
                    )
            },
            child.Size switch
            {
                { X: Pixels x, Y: Pixels y } => (x, y),
                { X: Percent x, Y: Percent y } => (parentSizeX * x, parentSizeY * y),
                { X: Percent x, Y: Pixels y } => (parentSizeX * x, y),
                { X: Pixels x, Y: Percent y } => (x, parentSizeY * y),
                { X: Ratio x, Y: Pixels y } => (y * x, y),
                { X: Pixels x, Y: Ratio y } => (x, x * y),
                { X: Ratio x, Y: Percent y } => (y * parentSizeY * x, y * parentSizeY),
                { X: Percent x, Y: Ratio y } => (x * parentSizeX, x * parentSizeX * y),
                { X: FitElement x, Y: FitElement y } => (x * elementSizeX, y * elementSizeY),
                { X: FitElement x, Y: Pixels y } => (y * elementSizeXRatio * x, y),
                { X: Pixels x, Y: FitElement y } => (x, x * elementSizeYRatio * y),
                { X: FitElement(1F) x, Y: Percent y }
                    => (parentSizeY * y * elementSizeXRatio, parentSizeY * y),

                { X: FitElement, Y: Percent y }
                    => throw new InvalidOperationException(
                        "FitElement must have a value of 1F when paired with Percent"
                    ),
                { X: Percent x, Y: FitElement(1F) y }
                    => (parentSizeX * x, parentSizeX * x * elementSizeYRatio),
                { X: Percent x, Y: FitElement y }
                    => throw new InvalidOperationException(
                        "FitElement must have a value of 1F when paired with Percent"
                    ),
                { X: FitElement x, Y: Ratio y } => (x * elementSizeX, x * elementSizeX * y),
                { X: Ratio x, Y: FitElement y } => (y * elementSizeY * x, y * elementSizeY),
                { X: Ratio, Y: Ratio }
                    => throw new InvalidOperationException("Cannot have 2 ratios"),
                _
                    => throw new NotImplementedException(
                        $"Size scalar combination {child.Size} not implemented"
                    )
            }
        )
        {
            IsVisible = () => child.IsVisible() && IsVisible(),
            DrawableElement = child.DrawableElement,
            Children = child.Children
        };
        if (
            child.BoxAlignment
                is Alignments.TopLeftOfPrevious
                    or Alignments.TopCenterOfPrevious
                    or Alignments.TopRightOfPrevious
                    or Alignments.MiddleLeftOfPrevious
                    or Alignments.MiddleCenterOfPrevious
                    or Alignments.MiddleRightOfPrevious
                    or Alignments.BottomLeftOfPrevious
                    or Alignments.BottomCenterOfPrevious
                    or Alignments.BottomRightOfPrevious
            && previous is null
        )
        {
            throw new Exception("BoxAlignment is set relative to previous but previous is null");
        }

        absoluteChild.Position = child.BoxAlignment switch
        {
            Alignments.TopLeft => PaddedPosition + absoluteChild.Position,
            Alignments.TopCenter
                => PaddedPosition
                    + new Vector2(
                        PaddedSize.X / 2 + absoluteChild.Position.X,
                        absoluteChild.Position.Y
                    ),
            Alignments.TopRight
                => PaddedPosition
                    + new Vector2(
                        PaddedSize.X + absoluteChild.Position.X,
                        absoluteChild.Position.Y
                    ),
            Alignments.MiddleLeft
                => PaddedPosition
                    + new Vector2(
                        absoluteChild.Position.X,
                        PaddedSize.Y / 2 + absoluteChild.Position.Y
                    ),
            Alignments.MiddleCenter
                => PaddedPosition
                    + new Vector2(
                        PaddedSize.X / 2 + absoluteChild.Position.X,
                        PaddedSize.Y / 2 + absoluteChild.Position.Y
                    ),
            Alignments.MiddleRight
                => PaddedPosition
                    + new Vector2(
                        PaddedSize.X + absoluteChild.Position.X,
                        PaddedSize.Y / 2 + absoluteChild.Position.Y
                    ),
            Alignments.BottomLeft
                => PaddedPosition
                    + new Vector2(
                        absoluteChild.Position.X,
                        PaddedSize.Y + absoluteChild.Position.Y
                    ),
            Alignments.BottomCenter
                => PaddedPosition
                    + new Vector2(
                        PaddedSize.X / 2 + absoluteChild.Position.X,
                        PaddedSize.Y + absoluteChild.Position.Y
                    ),
            Alignments.BottomRight
                => PaddedPosition
                    + new Vector2(
                        PaddedSize.X + absoluteChild.Position.X,
                        PaddedSize.Y + absoluteChild.Position.Y
                    ),
            Alignments.TopLeftOfPrevious => previous.Position + absoluteChild.Position,
            Alignments.TopCenterOfPrevious
                => previous.Position + absoluteChild.Position + new Vector2(previous.Size.X / 2, 0),
            Alignments.TopRightOfPrevious
                => previous.Position + absoluteChild.Position + new Vector2(previous.Size.X, 0),
            Alignments.MiddleLeftOfPrevious
                => previous.Position + absoluteChild.Position + new Vector2(0, previous.Size.Y / 2),
            Alignments.MiddleCenterOfPrevious
                => previous.Position
                    + absoluteChild.Position
                    + new Vector2(previous.Size.X / 2, previous.Size.Y / 2),
            Alignments.MiddleRightOfPrevious
                => previous.Position
                    + absoluteChild.Position
                    + new Vector2(previous.Size.X, previous.Size.Y / 2),
            Alignments.BottomLeftOfPrevious
                => previous.Position + absoluteChild.Position + new Vector2(0, previous.Size.Y),
            Alignments.BottomCenterOfPrevious
                => previous.Position
                    + absoluteChild.Position
                    + new Vector2(previous.Size.X / 2, previous.Size.Y),
            Alignments.BottomRightOfPrevious
                => previous.Position
                    + absoluteChild.Position
                    + new Vector2(previous.Size.X, previous.Size.Y),
            _ => absoluteChild.Position
        };
        absoluteChild.Position += child.SelfAlignment switch
        {
            Alignments.TopLeft => Vector2.Zero,
            Alignments.TopCenter => new Vector2(-absoluteChild.Size.X / 2, 0),
            Alignments.TopRight => new Vector2(-absoluteChild.Size.X, 0),
            Alignments.MiddleLeft => new Vector2(0, -absoluteChild.Size.Y / 2),
            Alignments.MiddleCenter
                => new Vector2(-absoluteChild.Size.X / 2, -absoluteChild.Size.Y / 2),
            Alignments.MiddleRight => new Vector2(-absoluteChild.Size.X, -absoluteChild.Size.Y / 2),
            Alignments.BottomLeft => new Vector2(0, -absoluteChild.Size.Y),
            Alignments.BottomCenter
                => new Vector2(-absoluteChild.Size.X / 2, -absoluteChild.Size.Y),
            Alignments.BottomRight => new Vector2(-absoluteChild.Size.X, -absoluteChild.Size.Y),
            Alignments.TopLeftOfPrevious
            or Alignments.TopCenterOfPrevious
            or Alignments.TopRightOfPrevious
            or Alignments.MiddleLeftOfPrevious
            or Alignments.MiddleCenterOfPrevious
            or Alignments.MiddleRightOfPrevious
            or Alignments.BottomLeftOfPrevious
            or Alignments.BottomCenterOfPrevious
            or Alignments.BottomRightOfPrevious
                => throw new InvalidOperationException(
                    "SelfAlignment isn't meant to use relative to previous alignments"
                ),
            _ => Vector2.Zero
        };

        absoluteChild.PaddedSize = new ScalarVector2(absoluteChild.Size) - child.Padding;
        absoluteChild.PaddedPosition =
            absoluteChild.Position + (absoluteChild.Size - absoluteChild.PaddedSize) / 2;
        return absoluteChild;
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
            List<AbsoluteBox> previousBoxes = new();
            foreach (var relativeChild in Children)
            {
                previousBoxes.Add(
                    relativeChild.Draw(
                        spriteBatch,
                        this,
                        previousBoxes.ElementAtOrDefault(relativeChild.PreviousBoxIndex)
                    )
                );
            }
        }

        if (Game.DebugMode != 2 && (Game.DebugMode != 1 || !visible))
            return;

        if (visible)
        {
            spriteBatch.DrawRectangle(PaddedPosition, PaddedSize, PaddingDebugColor);
        }

        spriteBatch.DrawRectangle(Position, Size, visible ? DebugColor : InvisibleDebugColor);
    }

    public void Update(GameTime gameTime)
    {
        if (!IsVisible())
            return;

        DrawableElement?.Update(gameTime, this);
        List<AbsoluteBox> previousBoxes = new();
        foreach (var relativeChild in Children)
        {
            previousBoxes.Add(
                relativeChild.Update(
                    gameTime,
                    this,
                    previousBoxes.ElementAtOrDefault(relativeChild.PreviousBoxIndex)
                )
            );
        }
    }
}
