using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source.GUI;

public class Slider : UiElement
{
    private UiTextureElement _sliderTexture;
    private UiTextureElement _filledSliderTexture;
    private UiTextureElement _sliderButtonTexture;
    private float _value;
    private float _minValue;
    private float _maxValue;

    public float Value
    {
        get => _minValue + _value * (_maxValue - _minValue);
        set
        {
            _value = MathHelper.Clamp(value, _minValue, _maxValue);
            _sliderButtonTexture.Offset = new Vector2(
                _value * _sliderTexture.Size().X - _sliderButtonTexture.Size().X / 2F,
                _sliderButtonTexture.Offset.Y
            );
            _filledSliderTexture.SourceRectangle = new Rectangle(
                0,
                0,
                (int)(_sliderTexture.Size().X * _value),
                (int)_sliderTexture.Size().Y
            );
            _onValueChanged?.Invoke(Value);
        }
    }

    private readonly Action<float> _onValueChanged;
    public bool IsDragging { get; private set; }

    public Slider(
        UiTextureElement sliderTexture,
        UiTextureElement filledSliderTexture,
        UiTextureElement sliderButtonTexture,
        Action<float> onValueChanged,
        float initialValue = 0F,
        float minValue = 0F,
        float maxValue = 1F
    )
    {
        _sliderTexture = sliderTexture;
        _filledSliderTexture = filledSliderTexture;
        _sliderButtonTexture = sliderButtonTexture;
        _onValueChanged = onValueChanged;
        _minValue = minValue;
        _maxValue = maxValue;
        _sliderTexture.ScaleWithParent = false;
        _filledSliderTexture.ScaleWithParent = false;
        _sliderButtonTexture.ScaleWithParent = false;
        _filledSliderTexture.Offset = _sliderTexture.Offset = new Vector2(
            0,
            _sliderButtonTexture.Texture.Height / 2F - _sliderTexture.Texture.Height / 2F
        );
        Value = initialValue;
    }

    public override Vector2 Size() =>
        new(
            Math.Max(_sliderTexture.Size().X, _sliderButtonTexture.Size().X),
            Math.Max(_sliderTexture.Size().Y, _sliderButtonTexture.Size().Y)
        );

    public override void Draw(SpriteBatch spriteBatch, AbsoluteBox parent)
    {
        _sliderTexture.Draw(spriteBatch, parent);
        _filledSliderTexture.Draw(spriteBatch, parent);
        _sliderButtonTexture.Draw(spriteBatch, parent);
    }

    public override void Update(GameTime gameTime, AbsoluteBox parent)
    {
        var mouseState = Mouse.GetState();
        var mousePosition = mouseState.Position.ToVector2();

        var sliderButtonRect = new Rectangle(parent.Position.ToPoint(), Size().ToPoint());

        if (
            parent.Game.MouseValues.JustPressed(MouseButton.Left)
            && sliderButtonRect.Contains(mousePosition)
        )
        {
            IsDragging = true;
        }
        else if (parent.Game.MouseValues.JustReleased(MouseButton.Left))
        {
            IsDragging = false;
        }

        if (!IsDragging)
            return;
        Value = (mousePosition.X - parent.Position.X) / _sliderTexture.Size().X;
    }
}
