using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public abstract class UiElement
{
    public abstract Vector2 Size();
    public abstract void Draw(SpriteBatch spriteBatch, AbsoluteBox parent);

    public virtual void Update(GameTime gameTime, AbsoluteBox parent) { }

    public virtual void OnClick(AbsoluteBox parent, MouseButton button) { }

    public virtual void OnRelease(AbsoluteBox parent, MouseButton button) { }

    public virtual void OnHovered(AbsoluteBox parent) { }

    public virtual void OnUnhovered(AbsoluteBox parent) { }
}
