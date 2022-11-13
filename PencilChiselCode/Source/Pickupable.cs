using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source;

public class Pickupable
{
    public Texture2D Texture { get; set; }
    public Vector2 Size => new(Texture.Width, Texture.Height);
    public Vector2 Position;
    public float Rotation;
    public bool IsConsumable;
    private Vector2 _scale;
    public SoundEffect PickupSound { get; set; }
    public PickupableTypes Type { get; set; }

    public Pickupable(PickupableTypes type, Texture2D texture, SoundEffect pickupSound, Vector2 position,
        Vector2 scale, float rotation = 0F)
    {
        IsConsumable = true;
        Texture = texture;
        Position = position;
        Rotation = rotation;
        _scale = scale;
        Type = type;
        PickupSound = pickupSound;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Size / 2, _scale, SpriteEffects.None, 0);
    }

    public void Update(GameTime gameTime)
    {
        Rotation = 180F;
    }
}