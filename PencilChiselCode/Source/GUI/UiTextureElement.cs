using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class UiTextureElement : IUiElement
{
    public Vector2 Size() => new(_texture.Width, _texture.Height);

    private readonly Texture2D _texture;

    public UiTextureElement(Texture2D texture) => _texture = texture;

    public void Draw(SpriteBatch spriteBatch, Box parent) =>
        spriteBatch.Draw(
            _texture,
            parent.Position,
            null,
            Color.White,
            0F,
            Vector2.Zero,
            parent.Scale,
            SpriteEffects.None,
            0F
        );

    public void Update(GameTime gameTime, Box parent) { }
}
