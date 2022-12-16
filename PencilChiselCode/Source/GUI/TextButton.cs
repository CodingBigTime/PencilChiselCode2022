using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source.GUI;

public class TextButton : Button
{
    public override Vector2 Size() => TextElement.Size();

    private UiTextElement TextElement { get; set; }
    private UiTextElement _normalTextElement;
    private UiTextElement _hoveredTextElement;
    private UiTextElement _pressedTextElement;
    private readonly SoundEffect _pressSound;
    private readonly SoundEffect _releaseSound;
    private readonly Action _action;

    public TextButton(
        UiTextElement normal,
        UiTextElement hovered,
        UiTextElement pressed,
        SoundEffect pressSound,
        SoundEffect releaseSound,
        Action action
    )
    {
        _pressSound = pressSound;
        _releaseSound = releaseSound;
        _normalTextElement = normal;
        _hoveredTextElement = hovered;
        _pressedTextElement = pressed;
        TextElement = normal;

        _action = action;
    }

    public override void Draw(SpriteBatch spriteBatch, Box parent) =>
        TextElement.Draw(spriteBatch, parent);

    public override void Update(GameTime gameTime, Box parent)
    {
        var mouseState = Mouse.GetState();
        var inside = Utils.IsPointInRectangle(
            mouseState.Position.ToVector2(),
            new Rectangle(parent.Position.ToPoint(), parent.Size().ToPoint())
        );
        var released = mouseState.LeftButton == ButtonState.Released;

        if (inside)
        {
            if (released)
            {
                if (TextElement == _pressedTextElement)
                {
                    _releaseSound.Play();
                    Click();
                }

                TextElement = _hoveredTextElement;
            }
            else if (TextElement == _hoveredTextElement)
            {
                _pressSound.Play();
                TextElement = _pressedTextElement;
            }
        }
        else
        {
            if (TextElement == _pressedTextElement)
            {
                _releaseSound.Play();
            }

            TextElement = _normalTextElement;
        }
    }

    public override void Click() => _action.Invoke();
}
