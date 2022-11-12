using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source;

public class Pickupable
{
    public Texture2D Texture { get; set; }
    public Vector2 Size => new(Texture.Width, Texture.Height);
    public Vector2 Position;
    public float Rotation;
    public PickupableTypes Type { get; set; }

    public Pickupable(PickupableTypes type, Texture2D texture, Vector2 position, float rotation)
    {
        Texture = texture;
        Position = position;
        Rotation = rotation;
        Type = type;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Size / 2, 1, SpriteEffects.None, 0);
    }

    public void Update(GameTime gameTime)
    {
        Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds;
    }
}