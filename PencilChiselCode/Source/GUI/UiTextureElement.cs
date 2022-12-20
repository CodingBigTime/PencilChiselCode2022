using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class UiTextureElement : UiElement
{
    public override Vector2 Size() => new(Texture.Width, Texture.Height);

    public Texture2D Texture { get; protected set; }

    public Color Color { get; set; } = Color.White;

    public UiTextureElement(Texture2D texture) => Texture = texture;

    public override void Draw(SpriteBatch spriteBatch, AbsoluteBox parent)
    {
        spriteBatch.Draw(
            Texture,
            parent.Position,
            null,
            Color,
            0F,
            Vector2.Zero,
            parent.Size / Size(),
            SpriteEffects.None,
            0F
        );
    }
}
