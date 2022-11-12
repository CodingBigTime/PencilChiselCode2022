using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source
{
    public class AttributeGroup
    {
        private List<Attribute> _attributes { get; }
        
        public AttributeGroup(List<Attribute> attributes)
        {
            _attributes = attributes;
        }

        public bool IsEmptyAny()
        {
            return _attributes.Any(attribute => attribute.IsEmpty());
        }

        public void Update(GameTime gameTime)
        {
            _attributes.ForEach(attribute => attribute.Update(gameTime));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _attributes.ForEach(attribute => attribute.Draw(spriteBatch));
        }
    }
}
