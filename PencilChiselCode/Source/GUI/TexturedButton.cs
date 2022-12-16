using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source.GUI;

public class TexturedButton : Button
{
    private Texture2D Texture { get; set; }

    public override Vector2 Size() => new(Texture.Width, Texture.Height);

    private readonly Texture2D _normalTexture;
    private readonly Texture2D _hoveredTexture;
    private readonly Texture2D _pressedTexture;
    private readonly SoundEffect _pressSound;
    private readonly SoundEffect _releaseSound;
    private readonly Action _action;

    public TexturedButton(
        Texture2D normal,
        Texture2D hovered,
        Texture2D pressed,
        SoundEffect pressSound,
        SoundEffect releaseSound,
        Action action
    )
    {
        _pressSound = pressSound;
        _releaseSound = releaseSound;
        _normalTexture = normal;
        _hoveredTexture = hovered;
        _pressedTexture = pressed;
        Texture = normal;
        _action = action;
    }

    public override void Draw(SpriteBatch spriteBatch, Box parent) =>
        spriteBatch.Draw(
            Texture,
            parent.Position,
            null,
            Color.White,
            0F,
            Vector2.Zero,
            parent.Scale,
            SpriteEffects.None,
            0F
        );

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
                if (Texture == _pressedTexture)
                {
                    _releaseSound.Play();
                    Click();
                }

                Texture = _hoveredTexture;
            }
            else if (Texture == _hoveredTexture)
            {
                _pressSound.Play();
                Texture = _pressedTexture;
            }
        }
        else
        {
            if (Texture == _pressedTexture)
            {
                _releaseSound.Play();
            }

            Texture = _normalTexture;
        }
    }

    public override void Click() => _action.Invoke();
}
