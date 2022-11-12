using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;

namespace PencilChiselCode.Source;

public class IngameState : GameScreen
{
    private static readonly Color BgColor = Color.Green;
    private new Game1 Game => (Game1)base.Game;

    public IngameState(Game1 game) : base(game)
    {
    }

    public List<Pickupable> Pickupables { get; } = new();


    public override void LoadContent()
    {
        base.LoadContent();
        Pickupables.Add(new Pickupable(PickupableTypes.Twig, Game1.Instance.TextureMap["twigs"], new Vector2(300, 300), 0.5F));
    }

    public override void Update(GameTime gameTime)
    {
        Game1.Instance.Player.Update(this, gameTime);
        Pickupables.ForEach(pickupable => pickupable.Update(gameTime));
    }

    public override void Draw(GameTime gameTime)
    {
        Game1.Instance.GraphicsDevice.Clear(BgColor);
        Game1.Instance.SpriteBatch.Begin();
        Pickupables.ForEach(pickupable => pickupable.Draw(Game1.Instance.SpriteBatch));
        Game1.Instance.Player.Draw(Game1.Instance.SpriteBatch);
        Game1.Instance.SpriteBatch.End();
    }
}