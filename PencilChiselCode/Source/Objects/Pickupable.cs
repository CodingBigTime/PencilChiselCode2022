using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source.Objects;

public abstract class Pickupable : GroundEntity
{
    public bool IsConsumable;
    public SoundEffect PickupSound { get; set; }
    public PickupableTypes Type { get; set; }
    private const float RenderOffset = 32;

    public override bool ShouldRemove() =>
        base.ShouldRemove()
        || Game.Camera.Position.X > Position.X + Size.X * Scale.X + RenderOffset;

    public Pickupable(
        IngameState state,
        PickupableTypes type,
        Texture2D texture,
        SoundEffect pickupSound,
        Vector2 position,
        Vector2 scale,
        float rotation = 0F
    ) : base(state, texture, position, scale, rotation)
    {
        IsConsumable = true;
        Type = type;
        PickupSound = pickupSound;
    }

    public virtual void OnPickup() => PickupSound.Play();
}
