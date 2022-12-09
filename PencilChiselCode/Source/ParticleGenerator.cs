using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source;

public class ParticleGenerator
{
    private readonly List<Particle> _particles;
    private readonly Func<Particle> _particleGenerator;
    private float _frequency;

    public ParticleGenerator(Func<Particle> particleGenerator, float frequency)
    {
        _particles = new();
        _particleGenerator = particleGenerator;
        _frequency = frequency;
    }

    public void Update(GameTime gameTime, bool addNew)
    {
        if (
            addNew && Utils.RANDOM.NextDouble() < _frequency * gameTime.ElapsedGameTime.TotalSeconds
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
