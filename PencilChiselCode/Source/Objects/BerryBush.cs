using Microsoft.Xna.Framework;
using PencilChiselCode.Source.GameStates;

namespace PencilChiselCode.Source.Objects;

public class BerryBush : Pickupable
{
    public BerryBush(
        IngameState state,
        Vector2 position,
        Vector2 scale,
        float rotation = 0
    ) : base(
        state,
        PickupableTypes.Bush,
        state.Game.TextureMap["bush_berry"],
        state.Game.SoundMap["pickup_branches"],
        position,
        scale,
        rotation
    )
    {
    }

    public override void OnPickup()
    {
        base.OnPickup();
        Texture = Game.TextureMap["bush_empty"];
    }
}