using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source;

public class Pickupable
{
    public Texture2D Texture { get; set; }
    public Vector2 Size => new(Texture.Width, Texture.Height);
    public Vector2 Position;
    public float Rotation;
    public Pickupable(Texture2D texture, Vector2 position, float rotation)
    {
        Texture = texture;
        Position = position;
        Rotation = rotation;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Size / 2, 1, SpriteEffects.None, 0);
    }

    public void Update(GameTime gameTime)
    {
        Rotation += 5*(float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}