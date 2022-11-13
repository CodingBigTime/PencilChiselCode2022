using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;

namespace PencilChiselCode.Source;

public class GroundEntity
{
    public Texture2D Texture { get; set; }
    public Vector2 Size => new(Texture.Width, Texture.Height);

    public Vector2 Position
    {
        get => _position;
        set
        {
            _position = value;
            PointLight.Position = new Vector2(value.X, value.Y);
        }
    }

    private Vector2 _position;
    public float Rotation;
    private Vector2 _scale;
    private Game1 _game;
    private Color _glow;

    public GroundEntity(Game1 game, Texture2D texture, Vector2 position, Vector2 scale,
        Color glow, float rotation = 0F)
    {
        _game = game;
        Texture = texture;
        Position = position;
        Rotation = rotation;
        _scale = scale;
        _game.Penumbra.Lights.Add(PointLight);
        PointLight.Color = glow;
    }

    public GroundEntity(Game1 game, Texture2D texture, Vector2 position, Vector2 scale, float rotation = 0F)
    {
        _game = game;
        Texture = texture;
        Position = position;
        Rotation = rotation;
        _scale = scale;
    }

    public Light PointLight { get; } = new PointLight
    {
        Scale = new(128),
        Radius = 1,
        ShadowType = ShadowType.Occluded
    };

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, null, Color.White, Rotation, Size / 2, _scale, SpriteEffects.None, 0);
    }

    public void Update(GameTime gameTime)
    {
    }
}