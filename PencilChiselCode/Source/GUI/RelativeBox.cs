using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source.GUI;

public class RelativeBox : Box
{
    public ScalarVector2 Position { get; set; }
    public ScalarVector2 Size { get; set; }

    public RelativeBox(Bonfire game, ScalarVector2 position, ScalarVector2 size) : base(game)
    {
        Position = position;
        Size = size;
    }

    public void Draw(SpriteBatch spriteBatch, AbsoluteBox parent)
    {
        if (!IsVisible() && Game.DebugMode != 2)
            return;
        parent.AbsoluteFrom(this).Draw(spriteBatch);
    }

    public void Update(GameTime gameTime, AbsoluteBox parent)
    {
        if (!IsVisible())
            return;
        parent.AbsoluteFrom(this).Update(gameTime);
    }
}
