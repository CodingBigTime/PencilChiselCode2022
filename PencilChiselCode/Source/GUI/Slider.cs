using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source.GUI;

public class Slider : RelativeBox
{
    private readonly UiTextureElement _sliderTexture;
    private readonly UiTextureElement _filledSliderTexture;
    private readonly UiTextureElement _sliderButtonTexture;
    private readonly RelativeBox _sliderButtonBox;
    private float _value;
    private readonly float _minValue;
    private readonly float _maxValue;

    public float Value
    {
        get => _minValue + _value * (_maxValue - _minValue);
        set
        {
            _value = Math.Clamp(value, 0F, 1F);
            _sliderButtonBox.Position = (_value, 0);
            _filledSliderTexture.SourceRectangleBox = new(null, 0F, (_value, 1F));
            _onValueChanged?.Invoke(Value);
        }
    }

    private readonly Action<float> _onValueChanged;
    public bool IsDragging { get; private set; }

    public Slider(
        Bonfire game,
        ScalarVector2 position,
        ScalarVector2 size,
        UiTextureElement sliderTexture,
        UiTextureElement filledSliderTexture,
        UiTextureElement sliderButtonTexture,
        Action<float> onValueChanged,
        float initialValue = 0F,
        float minValue = 0F,
        float maxValue = 1F
    ) : base(game, position, size)
    {
        _sliderTexture = sliderTexture;
        _filledSliderTexture = filledSliderTexture;
        _sliderButtonTexture = sliderButtonTexture;
        _onValueChanged = onValueChanged;
        _minValue = minValue;
        _maxValue = maxValue;
        var sliderTextureRatio = new Ratio(
            (float)_sliderTexture.Texture.Height / _sliderTexture.Texture.Width
        );
        var sliderButtonTextureRatio = new Ratio(
            (float)_sliderButtonTexture.Texture.Width / _sliderButtonTexture.Texture.Height
        );

        var sliderBox = new RelativeBox(game, 0, (1F, sliderTextureRatio))
        {
            DrawableElement = _sliderTexture,
            BoxAlignment = Alignments.MiddleCenter,
            SelfAlignment = Alignments.MiddleCenter,
        };
        var filledSliderBox = new RelativeBox(game, 0, (1F, sliderTextureRatio))
        {
            DrawableElement = _filledSliderTexture,
            BoxAlignment = Alignments.MiddleCenter,
            SelfAlignment = Alignments.MiddleCenter,
        };
        _sliderButtonBox = new RelativeBox(game, 0, (sliderButtonTextureRatio, 1F))
        {
            DrawableElement = _sliderButtonTexture,
            BoxAlignment = Alignments.MiddleLeft,
            SelfAlignment = Alignments.MiddleCenter,
        };
        AddChild(sliderBox, filledSliderBox, _sliderButtonBox);
        Value = initialValue;
    }

    public override Vector2 DrawableElementSize =>
        new(
            _sliderTexture.Size().X,
            Math.Max(_sliderTexture.Size().Y, _sliderButtonTexture.Size().Y)
        );

    public override AbsoluteBox Update(
        GameTime gameTime,
        AbsoluteBox parent,
        AbsoluteBox previous = null
    )
    {
        var absoluteSelf = parent.AbsoluteFrom(this, previous);
        var absoluteSliderButtonBox = absoluteSelf.AbsoluteFrom(_sliderButtonBox);
        var mouseState = Mouse.GetState();
        var mousePosition = mouseState.Position.ToVector2();

        var sliderButtonRect = new Rectangle(
            new Vector2(
                absoluteSelf.Position.X - absoluteSliderButtonBox.Size.X / 2,
                absoluteSelf.Position.Y
            ).ToPoint(),
            new Vector2(
                absoluteSelf.Size.X + absoluteSliderButtonBox.Size.X,
                absoluteSelf.Size.Y
            ).ToPoint()
        );

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

        if (IsDragging)
            Value = (mousePosition.X - absoluteSelf.Position.X) / absoluteSelf.Size.X;

        absoluteSelf.Update(gameTime);
        return absoluteSelf;
    }
}
