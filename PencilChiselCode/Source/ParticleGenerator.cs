using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace PencilChiselCode.Source;

public class ParticleGenerator
{
    private readonly List<Particle> _particles;
    private readonly Func<Particle> _particleGenerator;
    private readonly Func<float> _frequencyFunction;

    public ParticleGenerator(Func<Particle> particleGenerator, float frequency)
        : this(particleGenerator, () => frequency) { }

    public ParticleGenerator(Func<Particle> particleGenerator, Func<float> frequency)
    {
        _particles = new();
        _particleGenerator = particleGenerator;
        _frequencyFunction = frequency;
    }

    public void Update(GameTime gameTime, bool addNew)
    {
        if (
            addNew
            && Utils.Random.NextDouble() < _frequencyFunction() * gameTime.GetElapsedSeconds()
        )
        {
            _particles.Add(_particleGenerator());
        }

        _particles.ForEach(particle => particle.Update(gameTime));
        _particles.RemoveAll(particle => particle.IsExpired());
    }

    public void Draw(SpriteBatch spriteBatch) =>
        _particles.ForEach(particle => particle.Draw(spriteBatch));
}
