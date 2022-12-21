using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PencilChiselCode.Source.GUI;

public class RootBox : AbsoluteBox
{
    public RootBox(Bonfire game) : base(game, 0, game.GetWindowDimensions()) { }

    new public void Update(GameTime gameTime)
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
