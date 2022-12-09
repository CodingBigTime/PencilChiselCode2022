using Microsoft.Xna.Framework;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source.Objects;

public class Twig : Pickupable
{
    public Twig(IngameState state, Vector2 position, Vector2 scale, float rotation = 0)
        : base(
            state,
            PickupableTypes.Twig,
            state.Game.TextureMap["twigs"],
            state.Game.SoundMap["pickup_branches"],
            position,
            scale,
            rotation
        ) { }

    public override void OnPickup()
    {
        base.OnPickup();
        MarkForRemoval();
    }
}
