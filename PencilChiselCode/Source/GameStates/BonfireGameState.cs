using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace PencilChiselCode.Source.GameStates;

public abstract class BonfireGameState : GameScreen
{
    public new readonly Bonfire Game;
    public Color BgColor { get; set; }

    protected BonfireGameState(Game game) : base(game)
    {
        Game = (Bonfire)game;
    }
}