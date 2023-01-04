#nullable enable
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class Button : UiElement
{
    public bool IsHighlighted { get; private set; }
    public bool IsPressed { get; protected set; }
    protected readonly SoundEffect? PressSound;
    protected readonly SoundEffect? ReleaseSound;
    protected readonly Action? Action;
    private readonly UiElement _normalElement;
    private readonly UiElement _hoveredElement;
    private readonly UiElement _pressedElement;

    public UiElement ButtonElement =>
        IsHighlighted switch
        {
            true => IsPressed ? _pressedElement : _hoveredElement,
            _ => _normalElement
        };

    public Button(
        UiElement normalElement,
        UiElement hoveredElement,
        UiElement pressedElement,
        SoundEffect? pressSound = null,
        SoundEffect? releaseSound = null,
        Action? action = null
    )
    {
        Action = action;
        PressSound = pressSound;
        ReleaseSound = releaseSound;
        _normalElement = normalElement;
        _hoveredElement = hoveredElement;
        _pressedElement = pressedElement;
    }

    public Button(
        Texture2D normalTexture,
        Texture2D hoveredTexture,
        Texture2D pressedTexture,
        SoundEffect? pressSound = null,
        SoundEffect? releaseSound = null,
        Action? action = null
    )
        : this(
            new UiTextureElement(normalTexture),
            new UiTextureElement(hoveredTexture),
            new UiTextureElement(pressedTexture),
            pressSound,
            releaseSound,
            action
        ) { }

    public override Vector2 Size() => ButtonElement.Size();

    public override void Draw(SpriteBatch spriteBatch, AbsoluteBox parent) =>
        ButtonElement.Draw(spriteBatch, parent);

    public override void Update(GameTime gameTime, AbsoluteBox parent)
    {
        ButtonElement.Update(gameTime, parent);
        var mouse = parent.Game.MouseValues;
        var region = new Rectangle(parent.PaddedPosition.ToPoint(), parent.PaddedSize.ToPoint());
        IsHighlighted = Utils.IsPointInRectangle(mouse.CurrentState.Position.ToVector2(), region);
        if (mouse.JustExited(region))
        {
            OnUnhovered(parent);
        }
        else if (mouse.JustEntered(region))
        {
            OnHovered(parent);
        }

        switch (IsHighlighted)
        {
            case true when mouse.JustPressed(MouseButton.Left):
                OnClick(parent, MouseButton.Left);
                break;
            case true when mouse.JustReleased(MouseButton.Left):
                OnRelease(parent, MouseButton.Left);
                break;
        }
    }

    public override void OnUnhovered(AbsoluteBox parent)
    {
        if (IsPressed)
            ReleaseSound?.Play();
        IsPressed = false;
    }

    public override void OnClick(AbsoluteBox parent, MouseButton button)
    {
        if (button != MouseButton.Left || !IsHighlighted)
            return;
        PressSound?.Play();

        IsPressed = true;
    }

    public override void OnRelease(AbsoluteBox parent, MouseButton button)
    {
        if (button != MouseButton.Left || !IsPressed || !IsHighlighted)
            return;
        ReleaseSound?.Play();

        Click();
        IsPressed = false;
    }

    public void Click() => Action?.Invoke();
}
