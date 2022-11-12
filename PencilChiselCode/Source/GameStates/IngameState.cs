using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Screens;

namespace PencilChiselCode.Source;

public class IngameState : GameScreen
{
    private static readonly Color BgColor = Color.Green;
    private new Game1 Game => (Game1)base.Game;
    private Player Player;
    private bool _showDebug = false;
    private HashSet<Keys> _previousPressedKeys = new();

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
        var keyState = Keyboard.GetState();

        if (keyState.IsKeyDown(Keys.Z) && !_previousPressedKeys.Contains(Keys.Z))
        {
            _showDebug = !_showDebug;
        }

        _previousPressedKeys.Clear();
        _previousPressedKeys.UnionWith(keyState.GetPressedKeys());
    }

    public override void Draw(GameTime gameTime)
    {
        Game.GraphicsDevice.Clear(BgColor);
        Game.SpriteBatch.Begin();
        Pickupables.ForEach(pickupable => pickupable.Draw(Game1.Instance.SpriteBatch));
        Player.Draw(Game1.Instance.SpriteBatch);
        if (_showDebug)
        {
            Game.SpriteBatch.DrawString(Game.BitmapFont, $"FPS: {1 / gameTime.ElapsedGameTime.TotalSeconds}",
                new Vector2(16, 16), Color.Black);
        }

        Game.SpriteBatch.End();
    }
}