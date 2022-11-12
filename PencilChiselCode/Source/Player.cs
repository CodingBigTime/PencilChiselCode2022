using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PencilChiselCode.Source;

public class Player
{
    public Texture2D Texture { get; set; }
    public Vector2 Size => new(Texture.Width, Texture.Height);
    public Vector2 Position;
    private Vector2 _speed;
    private readonly float _maxSpeed = 255F;
    private readonly float _acceleration = 33F;
    private readonly float _friction = 0.95F;
    private uint _twigs = 0;

    public Player(Texture2D texture, Vector2 position)
    {
        Texture = texture;
        Position = position;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color.White);
    }

    public void Update(GameTime gameTime)
    {
        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
        var keyState = Keyboard.GetState();
        var a = keyState.IsKeyDown(Keys.A);
        var w = keyState.IsKeyDown(Keys.W);
        var s = keyState.IsKeyDown(Keys.S);
        var d = keyState.IsKeyDown(Keys.D);
        var mx = Convert.ToSingle(d) - Convert.ToSingle(a);
        var my = Convert.ToSingle(s) - Convert.ToSingle(w);

        if (mx != 0 && my != 0)
        {
            // Normalize the diagonal movement
            mx *= 0.70710678118654752440084436210485F;
            my *= 0.70710678118654752440084436210485F;
        }

        var ax = _acceleration * mx;
        var ay = _acceleration * my;

        _speed += new Vector2(ax, ay);
        _speed *= _friction;
        _speed = Utils.Clamp(_speed, -_maxSpeed, _maxSpeed);
        Position.X += _speed.X * delta;
        Position.Y += _speed.Y * delta;
    }
}