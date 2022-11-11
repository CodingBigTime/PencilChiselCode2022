using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source;

public interface IDrawable
{
    public Vector2 Size { get; set; }
    public Vector2 Position { get; set; }
    public void Draw(SpriteBatch spriteBatch);
}