using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class UiTextureElement : UiElement
{
    public override Vector2 Size() => Texture.Bounds.Size.ToVector2();

    public Vector2 Offset { get; set; }
    public RelativeBox SourceRectangleBox { get; set; }
    public Texture2D Texture { get; set; }

    public Color Color { get; set; } = Color.White;

    public UiTextureElement(Texture2D texture)
    {
        Texture = texture;
    }

    public override void Draw(SpriteBatch spriteBatch, AbsoluteBox parent)
    {
        Rectangle? sourceRectangle = null;
        if (SourceRectangleBox is not null)
        {
            var absoluteSourceRectangleBox = parent.AbsoluteFrom(SourceRectangleBox);
            sourceRectangle = new Rectangle(
                (int)(absoluteSourceRectangleBox.Position.X - parent.PaddedPosition.X),
                (int)(absoluteSourceRectangleBox.Position.Y - parent.PaddedPosition.Y),
                (int)(absoluteSourceRectangleBox.Size.X * Size().X / parent.PaddedSize.X),
                (int)(absoluteSourceRectangleBox.Size.Y * Size().Y / parent.PaddedSize.Y)
            );
        }
        spriteBatch.Draw(
            Texture,
            parent.PaddedPosition + Offset,
            sourceRectangle,
            Color,
            0F,
            Vector2.Zero,
            parent.PaddedSize / Size(),
            SpriteEffects.None,
            0F
        );
    }
}
