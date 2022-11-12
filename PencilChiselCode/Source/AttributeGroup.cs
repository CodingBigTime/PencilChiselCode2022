using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PencilChiselCode.Source
{
    public class AttributeGroup
    {
        public List<Attribute> Attributes { get; }
        
        public AttributeGroup(List<Attribute> attributes)
        {
            Attributes = attributes;
        }

        public bool IsEmptyAny()
        {
            return Attributes.Any(attribute => attribute.IsEmpty());
        }

        public void Update(GameTime gameTime)
        {
            Attributes.ForEach(attribute => attribute.Update(gameTime));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Attributes.ForEach(attribute => attribute.Draw(spriteBatch));
        }
    }
}
