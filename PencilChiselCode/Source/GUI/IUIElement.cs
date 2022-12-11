using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public interface IUiElement
{
    public Vector2 Size();
    void Draw(SpriteBatch spriteBatch, Box parent);
    void Update(GameTime gameTime, Box parent);
}
