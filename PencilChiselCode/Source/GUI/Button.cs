using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class Button : UiElement
{
    public bool IsHighlighted { get; private set; }
    public bool IsPressed { get; protected set; }
    protected readonly SoundEffect PressSound;
    protected readonly SoundEffect ReleaseSound;
    protected readonly Action Action;
    private readonly UiElement _normalElement;
    private readonly UiElement _hoveredElement;
    private readonly UiElement _pressedElement;

    public UiElement ButtonElement =>
        IsHighlighted switch
        {
            true => IsPressed ? _pressedElement : _hoveredElement,
            _ => _normalElement
        };

    private Button(Action action, SoundEffect pressSound, SoundEffect releaseSound)
    {
        Action = action;
        PressSound = pressSound;
        ReleaseSound = releaseSound;
    }

    public Button(
        UiElement normalElement,
        UiElement hoveredElement,
        UiElement pressedElement,
        SoundEffect pressSound,
        SoundEffect releaseSound,
        Action action
    ) : this(action, pressSound, releaseSound)
    {
        _normalElement = normalElement;
        _hoveredElement = hoveredElement;
        _pressedElement = pressedElement;
    }

    public Button(
        Texture2D normalTexture,
        Texture2D hoveredTexture,
        Texture2D pressedTexture,
        SoundEffect pressSound,
        SoundEffect releaseSound,
        Action action
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

    public override void Draw(SpriteBatch spriteBatch, Box parent) =>
        ButtonElement.Draw(spriteBatch, parent);

    public override void Update(GameTime gameTime, Box parent)
    {
        ButtonElement.Update(gameTime, parent);
        var mouse = parent.Game.MouseValues;
        var region = new Rectangle(parent.Position.ToPoint(), parent.Size().ToPoint());
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

    public override void OnUnhovered(Box parent)
    {
        if (IsPressed)
            ReleaseSound.Play();
        IsPressed = false;
    }

    public override void OnClick(Box parent, MouseButton button)
    {
        if (button != MouseButton.Left || !IsHighlighted)
            return;
        PressSound.Play();
        IsPressed = true;
    }

    public override void OnRelease(Box parent, MouseButton button)
    {
        if (button != MouseButton.Left || !IsPressed || !IsHighlighted)
            return;
        ReleaseSound.Play();
        Click();
        IsPressed = false;
    }

    public virtual void Click() => Action.Invoke();
}
