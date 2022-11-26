using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source
{
    public class ParticleGenerator
    {
        private HashSet<Particle> _particles;
        private Func<Particle> _particleGenerator;
        private float _frequency;


        public ParticleGenerator(Func<Particle> particleGenerator, float frequency)
        {
            _particles = new();
            _particleGenerator = particleGenerator;
            _frequency = frequency;
        }

        public void Update(GameTime gameTime, bool addNew)
        {
            if (addNew && Utils.RANDOM.NextDouble() < _frequency * gameTime.ElapsedGameTime.TotalSeconds)
            {
                _particles.Add(_particleGenerator());
            }
            _particles.ToList().ForEach(particle => particle.Update(gameTime));
            _particles.RemoveWhere(particle => particle.IsExpired());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _particles.ToList().ForEach(particle => particle.Draw(spriteBatch));
        }
    }
}