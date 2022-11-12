using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Screens;
using MonoGame.Extended;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source;

public class IngameState : GameScreen
{
    private static readonly Color _bgColor = Color.Green;
    private static float _cameraSpeed = 10.0F;
    private Game1 _game => (Game1)base.Game;
    private Player _player;

    public IngameState(Game game) : base(game)
    {
    }

    public List<Pickupable> Pickupables { get; } = new();


    public override void LoadContent()
    {
        base.LoadContent();
        Pickupables.Add(new Pickupable(PickupableTypes.Twig, Game1.Instance.TextureMap["twigs"], new Vector2(300, 300),
            0.5F));
        _player = new Player(_game, new Vector2(150, 150));
    }

    public override void Update(GameTime gameTime)
    {
        _game.Camera.Move(Vector2.UnitX * _cameraSpeed * gameTime.GetElapsedSeconds());
        _player.Update(this, gameTime);
        Pickupables.ForEach(pickupable => pickupable.Update(gameTime));
    }

    public override void Draw(GameTime gameTime)
    {
        var transformMatrix = _game.Camera.GetViewMatrix();
        _game.GraphicsDevice.Clear(_bgColor);
        _game.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);
        Pickupables.ForEach(pickupable => pickupable.Draw(Game1.Instance.SpriteBatch));
        _player.Draw(Game1.Instance.SpriteBatch);
        _game.SpriteBatch.End();
    }
}