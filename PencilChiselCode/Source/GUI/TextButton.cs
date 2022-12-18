using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class TextButton : Button
{
    public override Vector2 Size() => TextElement.Size();

    private UiTextElement TextElement =>
        IsHighlighted switch
        {
            true => IsPressed ? _pressedTextElement : _hoveredTextElement,
            _ => _normalTextElement
        };

    private readonly UiTextElement _normalTextElement;
    private readonly UiTextElement _hoveredTextElement;
    private readonly UiTextElement _pressedTextElement;

    public TextButton(
        UiTextElement normal,
        UiTextElement hovered,
        UiTextElement pressed,
        SoundEffect pressSound,
        SoundEffect releaseSound,
        Action action
    ) : base(action, pressSound, releaseSound)
    {
        _normalTextElement = normal;
        _hoveredTextElement = hovered;
        _pressedTextElement = pressed;
    }

    public override void Draw(SpriteBatch spriteBatch, Box parent) =>
        TextElement.Draw(spriteBatch, parent);
}
