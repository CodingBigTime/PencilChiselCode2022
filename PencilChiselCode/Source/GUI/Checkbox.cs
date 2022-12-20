using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class Checkbox : UiElement
{
    public bool IsChecked { get; private set; }
    protected readonly Action<bool> Action;
    private readonly UiElement _uncheckedElement;
    private readonly UiElement _checkedElement;
    private readonly SoundEffect _enabledSound;
    private readonly SoundEffect _disabledSound;
    public UiElement CheckboxElement => IsChecked ? _checkedElement : _uncheckedElement;

    public override Vector2 Size() => CheckboxElement.Size();

    public Checkbox(
        Texture2D checkedTexture,
        Texture2D uncheckedTexture,
        SoundEffect enabledSound,
        SoundEffect disableSound,
        Action<bool> action,
        bool isChecked = false
    )
        : this(
            new UiTextureElement(checkedTexture),
            new UiTextureElement(uncheckedTexture),
            enabledSound,
            disableSound,
            action,
            isChecked
        ) { }

    public Checkbox(
        UiElement checkedElement,
        UiElement uncheckedElement,
        SoundEffect enabledSound,
        SoundEffect disableSound,
        Action<bool> action,
        bool isChecked = false
    )
    {
        IsChecked = isChecked;
        Action = action;
        _enabledSound = enabledSound;
        _disabledSound = disableSound;
        _checkedElement = checkedElement;
        _uncheckedElement = uncheckedElement;
    }

    public override void Draw(SpriteBatch spriteBatch, AbsoluteBox parent) =>
        CheckboxElement.Draw(spriteBatch, parent);

    public override void Update(GameTime gameTime, AbsoluteBox parent)
    {
        base.Update(gameTime, parent);
        var mouse = parent.Game.MouseValues;
        var region = new Rectangle(parent.Position.ToPoint(), parent.Size.ToPoint());
        var isMouseInside = Utils.IsPointInRectangle(
            mouse.CurrentState.Position.ToVector2(),
            region
        );

        if (!isMouseInside)
            return;
        if (mouse.JustPressed(MouseButton.Left))
        {
            OnClick(parent, MouseButton.Left);
        }
    }

    public override void OnClick(AbsoluteBox parent, MouseButton button)
    {
        IsChecked = !IsChecked;
        if (IsChecked)
            _enabledSound.Play();
        else
            _disabledSound.Play();

        Action.Invoke(IsChecked);
    }
}
