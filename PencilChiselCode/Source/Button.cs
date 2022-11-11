using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source;

public class Button : IDrawable
{
    public Vector2 Size { get; set; } = new();
    public Vector2 Position { get; set; }
    private Texture2D _texture;
    private readonly Texture2D _normalTexture;
    private readonly Texture2D _hoveredTexture;
    private readonly Texture2D _pressedTexture;
    private readonly Action _action;

    public Button(Texture2D normal, Texture2D hovered, Texture2D pressed,Action action)
    {
        _normalTexture = normal;
        _hoveredTexture = hovered;
        _pressedTexture = pressed;
        _texture = normal;
        Size = new Vector2(_normalTexture.Width, _normalTexture.Height);
        Position = Utils.GetCenterStartCoords(Size, Game1.Instance.GetWindowDimensions());
        _action = action;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, Position, Color.White);
    }

    public void Update(GameTime gameTime)
    {
        var inside = Utils.IsPointInRectangle(Mouse.GetState().Position.ToVector2(),
            new Rectangle(Position.ToPoint(), Size.ToPoint()));
        var pressed = Mouse.GetState().LeftButton == ButtonState.Pressed;
        var released = Mouse.GetState().LeftButton == ButtonState.Released;

        var _pressSound = Game1.Instance.SoundMap["button_press"];
        var _releaseSound = Game1.Instance.SoundMap["button_release"];

        var mouseState = Mouse.GetState();
        if (inside)
        {
            if (mouseState.LeftButton == ButtonState.Released)
            {
                if (_texture == _pressedTexture)
                {
                    _releaseSound.Play();
                    Click();
                }

                _texture = _hoveredTexture;
            }
            else if (_texture == _hoveredTexture)
            {
                _pressSound.Play();
                _texture = _pressedTexture;
            }
        }
        else
        {
            if (_texture == _pressedTexture)
            {
                _releaseSound.Play();
            }

            _texture = _normalTexture;
        }
    }

    private void Click()
    {
        _action.Invoke();
    }
}