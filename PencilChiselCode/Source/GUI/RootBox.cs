using Microsoft.Xna.Framework;

namespace PencilChiselCode.Source.GUI;

public class RootBox : AbsoluteBox
{
    public RootBox(Bonfire game) : base(game, 0, game.GetWindowDimensions()) { }

    public new void Update(GameTime gameTime)
    {
        Size = Game.GetWindowDimensions();
        if (!IsVisible())
            return;
        DrawableElement?.Update(gameTime, this);
        AbsoluteBox previousChild = null;
        foreach (var box in Children)
        {
            previousChild = box.Update(gameTime, this, previousChild);
        }
    }
}
