using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace PencilChiselCode.Source;

public class IngameState : GameScreen
{
    private static readonly Color BgColor = Color.Green;
    private new Game1 Game => (Game1)base.Game;
    private Player Player;

    public IngameState(Game game) : base(game)
    {
    }

    public List<Pickupable> Pickupables { get; } = new();


    public override void LoadContent()
    {
        base.LoadContent();
        Pickupables.Add(new Pickupable(PickupableTypes.Twig, Game1.Instance.TextureMap["twigs"], new Vector2(300, 300),
            0.5F));
        Player = new Player(Game.TextureMap["player"], new Vector2(150, 150));
    }

    public override void Update(GameTime gameTime)
    {
        Player.Update(this, gameTime);
        Pickupables.ForEach(pickupable => pickupable.Update(gameTime));
    }

    public override void Draw(GameTime gameTime)
    {
        Game.GraphicsDevice.Clear(BgColor);
        Game.SpriteBatch.Begin();
        Pickupables.ForEach(pickupable => pickupable.Draw(Game1.Instance.SpriteBatch));
        Player.Draw(Game1.Instance.SpriteBatch);
        Game.SpriteBatch.End();
    }
}