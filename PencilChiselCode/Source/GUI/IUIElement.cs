using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public abstract class UiElement
{
    public abstract Vector2 Size();
    public abstract void Draw(SpriteBatch spriteBatch, Box parent);

    public virtual void Update(GameTime gameTime, Box parent) { }

    public virtual void OnClick(Box parent, MouseButton button) { }

    public virtual void OnRelease(Box parent, MouseButton button) { }

    public virtual void OnHovered(Box parent) { }

    public virtual void OnUnhovered(Box parent) { }
}
