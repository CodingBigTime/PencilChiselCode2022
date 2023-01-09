using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace PencilChiselCode.Source.GUI;

public class Box
{
    public readonly Bonfire Game;
    public List<RelativeBox> Children { get; protected set; } = new();
    public UiElement DrawableElement;
    public virtual Vector2 DrawableElementSize =>
        IsVisible() ? (DrawableElement?.Size() ?? Vector2.Zero) : Vector2.Zero;
    public Alignments BoxAlignment { get; set; } = Alignments.TopLeft;
    public Alignments SelfAlignment { get; set; } = Alignments.TopLeft;
    public Func<bool> IsVisible { get; set; } = () => true;
    public Color DebugColor = Color.LightGreen;
    public Color PaddingDebugColor = Color.LightCoral;
    public Color InvisibleDebugColor = Color.OrangeRed;

    public Box(Bonfire game)
    {
        Game = game;
    }

    public void AddChild(RelativeBox child) => Children.Add(child);

    public void AddChild(params RelativeBox[] children) => Children.AddRange(children);
}
