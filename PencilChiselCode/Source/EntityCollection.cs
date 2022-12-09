using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PencilChiselCode.Source.Objects;

namespace PencilChiselCode.Source
{
    public class EntityCollection<T>: List<T>
        where T : GroundEntity
    {
        new public void Add(T entity)
        {
            var binarySearchIndex = BinarySearch(entity, GroundEntity.YComparer);
            if (binarySearchIndex < 0)
            {
                binarySearchIndex = ~binarySearchIndex;
            }
            // O(n) insertion, not worth the effort to optimize
            Insert(binarySearchIndex, entity);
        }

        new public void Remove(T entity)
        {
            foreach (var e in this.Where(e => e == entity))
            {
                e.Cleanup();
            }
            RemoveAll(e => e == entity);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var entity in this)
            {
                entity.Update(gameTime);
            }
            RemoveDead();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var entity in this)
            {
                entity.Draw(spriteBatch);
            }
        }

        public void RemoveDead()
        {
            foreach (var entity in this.Where(entity => entity.ShouldRemove())) {
                entity.Cleanup();
            }
            RemoveAll(entity => entity.ShouldRemove());
        }

        public void ClearAll()
        {
            foreach (var entity in this)
            {
                entity.Cleanup();
            }
            Clear();
        }
    }
}