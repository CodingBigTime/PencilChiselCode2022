using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace PencilChiselCode.Source;

public class Particle
{
    private float _startTime;
    private readonly float _duration;
    private Vector2 _position;
    private Vector2 _velocity;
    private readonly Func<float, float> _scaleFunction;
    private readonly Func<float, Color> _colorFunction;
    private float _scale;
    private Color _color;
    private bool _expired;

    public Particle(float duration, Vector2 startPosition, Vector2 velocity, Func<float, float> scaleFunction,
        Func<float, Color> colorFunction)
    {
        _duration = duration;
        _position = startPosition;
        _scaleFunction = scaleFunction;
        _velocity = velocity;
        _colorFunction = colorFunction;
        _scale = _scaleFunction(0);
        _color = _colorFunction(0);
        _expired = false;
        _startTime = -1;
    }

    public bool IsExpired() => _expired;

    public void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_startTime < 0)
        {
            _startTime = (float)gameTime.TotalGameTime.TotalSeconds;
        }
        else if (gameTime.TotalGameTime.TotalSeconds - _startTime > _duration)
        {
            _expired = true;
        }

        var elapsedTime = (float)gameTime.TotalGameTime.TotalSeconds - _startTime;

        if (IsExpired()) return;

        _position += _velocity * deltaTime;
        _scale = _scaleFunction(elapsedTime);
        _color = _colorFunction(elapsedTime);
        _color.A = (byte)(Byte.MaxValue *
                          (1 - Math.Pow((gameTime.TotalGameTime.TotalSeconds - _startTime) / _duration, 2)));
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsExpired()) return;
        var size = new Vector2(10, 10) * _scale;
        spriteBatch.FillRectangle(_position - size / 2, size, _color);
    }
}