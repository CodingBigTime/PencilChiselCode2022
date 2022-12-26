using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace PencilChiselCode.Source.GUI;

public class UiTextElement : UiElement
{
    public Func<string> GetText { get; set; }
    public Color Color { get; set; }
    public Color? OutlineColor { get; set; }
    public BitmapFont Font { get; set; }

    public override Vector2 Size() => Font.MeasureString(GetText());

    public UiTextElement(
        BitmapFont font,
        Func<string> textFunc,
        Color? color = null,
        Color? outlineColor = null
    )
    {
        Font = font;
        GetText = textFunc;
        Color = color ?? Color.White;
        OutlineColor = outlineColor ?? Color.Black;
    }

    public override void Draw(SpriteBatch spriteBatch, AbsoluteBox parent)
    {
        if (OutlineColor.HasValue)
        {
            spriteBatch.DrawOutlinedString(
                Font,
                GetText(),
                parent.PaddedPosition,
                Color,
                OutlineColor.Value
            );
        }
        else
        {
            spriteBatch.DrawString(
                Font,
                GetText(),
                parent.PaddedPosition,
                Color,
                0F,
                Vector2.One * 200,
                1F,
                SpriteEffects.None,
                0F
            );
        }
    }
}
