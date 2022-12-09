using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace PencilChiselCode.Source;

public class TextIcon
{
    public string Text;
    private Vector2 _position;
    private readonly BitmapFont _font;

    public TextIcon(string text, Vector2 position, BitmapFont font)
    {
        Text = text;
        _position = position;
        _font = font;
    }

    public void Draw(SpriteBatch spriteBatch) =>
        spriteBatch.DrawString(_font, Text, _position, Color.White);
}
