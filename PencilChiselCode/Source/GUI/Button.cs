using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public abstract class Button : IUiElement
{
    public abstract Vector2 Size();

    public abstract void Draw(SpriteBatch spriteBatch, Box parent);

    public abstract void Update(GameTime gameTime, Box parent);
    public abstract void Click();
}
