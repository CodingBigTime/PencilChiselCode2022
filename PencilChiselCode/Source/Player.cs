using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source;

public class Player
{
    public Texture2D Texture { get; set; }
    public Vector2 Size => new(Texture.Width, Texture.Height);
    public Vector2 Position;
    private const float sqrt1_2 = 0.70710678118654752440084436210485F;
    private Game1 _game;
    private Vector2 _speed;
    private readonly float _maxSpeed = 80F;
    private readonly float _acceleration = 16F;
    private readonly float _friction = 0.95F;
    private uint _twigs = 0;

    public Player(Game1 game, Texture2D texture, Vector2 position)
    {
        _game = game;
        Texture = texture;
        Position = position;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position - new Vector2(Size.X / 2F, Size.Y / 2), Color.White);
    }

    public void Update(IngameState state, GameTime gameTime)
    {
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var keyState = Keyboard.GetState();
        var left = keyState.IsKeyDown(Keys.A) || keyState.IsKeyDown(Keys.Left);
        var up = keyState.IsKeyDown(Keys.W) || keyState.IsKeyDown(Keys.Up);
        var down = keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down);
        var right = keyState.IsKeyDown(Keys.D) || keyState.IsKeyDown(Keys.Right);
        var mx = Convert.ToSingle(right) - Convert.ToSingle(left);
        var my = Convert.ToSingle(down) - Convert.ToSingle(up);

        if (mx != 0 && my != 0)
        {
            // Normalize the diagonal movement
            mx *= sqrt1_2;
            my *= sqrt1_2;
        }

        var ax = _acceleration * mx;
        var ay = _acceleration * my;

        _speed += new Vector2(ax, ay);
        _speed *= _friction;
        _speed = Utils.Clamp(_speed, -_maxSpeed, _maxSpeed);
        Position.X += _speed.X * delta;
        Position.Y += _speed.Y * delta;

        if (Position.X >= _game.Camera.Center.X + _game.Width / 2F - Size.X / 2F)
        {
            _speed.X = 0;
            Position.X = Math.Min(Position.X, _game.Camera.Center.X + _game.Width / 2F - Size.X / 2F);
        }

        if (Position.Y <= Size.Y / 2F || Position.Y >= _game.Height - Size.Y / 2F)
        {
            _speed.Y = 0;
            Position.Y = Math.Clamp(Position.Y, Size.Y / 2F, _game.Height - Size.Y / 2F);
        }


        if (keyState.IsKeyDown(Keys.E))
        {
            var pickupable = state.Pickupables.Find(pickupable =>
                Utils.CreateCircle(Position, Size.GetAverageSize()).Expand(8)
                    .Intersects(Utils.CreateCircle(pickupable.Position, pickupable.Size.GetAverageSize()).Expand(8)));
            if (pickupable == null) return;
            switch (pickupable.Type)
            {
                case PickupableTypes.Twig:
                    ++_twigs;
                    break;
            }

            state.Pickupables.Remove(pickupable);
        }
    }
}