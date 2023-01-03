using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class UiTextureElement : UiElement
{
    public override Vector2 Size() => Texture.Bounds.Size.ToVector2();
    public Vector2 Offset { get; set; }
    public Rectangle? SourceRectangle { get; set; }
    public bool ScaleWithParent { get; set; } = true;

    public Texture2D Texture { get; protected set; }

    public Color Color { get; set; } = Color.White;

    public UiTextureElement(Texture2D texture) => Texture = texture;

    public override void Draw(SpriteBatch spriteBatch, AbsoluteBox parent)
    {
        spriteBatch.Draw(
            Texture,
            parent.PaddedPosition + Offset,
            SourceRectangle,
            Color,
            0F,
            Vector2.Zero,
            ScaleWithParent ? parent.PaddedSize / Size() : new Vector2(1F),
            SpriteEffects.None,
            0F
        );
    }
}