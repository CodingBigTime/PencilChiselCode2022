using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source;

public class Player
{
    public Vector2 Size;
    public Vector2 Position;
    private const float sqrt1_2 = 0.70710678118654752440084436210485F;
    private const float PI = (float)Math.PI;
    private readonly float _scale = 2F;
    private Game1 _game;
    private Vector2 _speed;
    private readonly float _maxSpeed = 80F;
    private readonly float _acceleration = 16F;
    private readonly float _friction = 0.95F;
    private uint _twigs = 0;

    public Player(Game1 game, Vector2 position)
    {
        _game = game;
        Position = position;
        var texturePlayerDown = _game.TextureMap["player_down"];
        Size = new(texturePlayerDown.Width * _scale, texturePlayerDown.Height * _scale);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        float angle = (float)Math.Atan2(_speed.Y, _speed.X);
        var (texture, spriteEffect) = angle switch {
            >= -PI / 4 and <= PI / 4 => (_game.TextureMap["player_left"], SpriteEffects.FlipHorizontally),
            > PI / 4 and < 3 * PI / 4 => (_game.TextureMap["player_down"], SpriteEffects.None),
            >= 3 * PI / 4 or <= -3 * PI / 4 => (_game.TextureMap["player_left"], SpriteEffects.None),
            > -3 * PI / 4 and < -PI / 4 => (_game.TextureMap["player_up"], SpriteEffects.None),
            _ => (_game.TextureMap["player_down"], SpriteEffects.None)
        };
        spriteBatch.Draw(
            texture: texture, 
            position: Position - new Vector2(Size.X / 2F, Size.Y / 2),
            sourceRectangle: null,
            color: Color.White,
            rotation: 0,
            origin: new(0, 0),
            scale: _scale,
            effects: spriteEffect,
            layerDepth: 0
        );
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