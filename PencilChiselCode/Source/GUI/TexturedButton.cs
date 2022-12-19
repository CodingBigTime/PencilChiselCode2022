using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class TexturedButton : Button
{
    private Texture2D Texture =>
        IsHighlighted switch
        {
            true => IsPressed ? _pressedTexture : _hoveredTexture,
            _ => _normalTexture
        };

    public override Vector2 Size() => new(Texture.Width, Texture.Height);

    private readonly Texture2D _normalTexture;
    private readonly Texture2D _hoveredTexture;
    private readonly Texture2D _pressedTexture;

    public TexturedButton(
        Texture2D normal,
        Texture2D hovered,
        Texture2D pressed,
        SoundEffect pressSound,
        SoundEffect releaseSound,
        Action action
    ) : base(action, pressSound, releaseSound)
    {
        _normalTexture = normal;
        _hoveredTexture = hovered;
        _pressedTexture = pressed;
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
}
