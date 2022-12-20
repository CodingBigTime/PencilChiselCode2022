using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace PencilChiselCode.Source.GUI;

public abstract class Button : UiElement
{
    public bool IsHighlighted { get; private set; }
    public bool IsPressed { get; protected set; }
    protected readonly SoundEffect PressSound;
    protected readonly SoundEffect ReleaseSound;
    protected readonly Action Action;

    protected Button(Action action, SoundEffect pressSound, SoundEffect releaseSound)
    {
        Action = action;
        PressSound = pressSound;
        ReleaseSound = releaseSound;
    }

    public override void Update(GameTime gameTime, AbsoluteBox parent)
    {
        var mouse = parent.Game.MouseValues;
        var region = new Rectangle(parent.Position.ToPoint(), parent.Size.ToPoint());
        IsHighlighted = Utils.IsPointInRectangle(mouse.CurrentState.Position.ToVector2(), region);
        if (mouse.JustExited(region))
        {
            OnUnhovered(parent);
        }
        else if (mouse.JustEntered(region))
        {
            OnHovered(parent);
        }

        if (mouse.JustPressed(MouseButton.Left))
        {
            OnClick(parent, MouseButton.Left);
        }
        else if (mouse.JustReleased(MouseButton.Left))
        {
            OnRelease(parent, MouseButton.Left);
        }
    }

    public override void OnUnhovered(AbsoluteBox parent)
    {
        if (IsPressed)
            ReleaseSound.Play();
        IsPressed = false;
    }

    public override void OnClick(AbsoluteBox parent, MouseButton button)
    {
        if (button != MouseButton.Left || !IsHighlighted)
            return;
        PressSound.Play();
        IsPressed = true;
    }

    public override void OnRelease(AbsoluteBox parent, MouseButton button)
    {
        if (button != MouseButton.Left || !IsPressed || !IsHighlighted)
            return;
        ReleaseSound.Play();
        Click();
        IsPressed = false;
    }

    public void Click() => Action.Invoke();
}
