using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    // Basic class representing an entity that exists in the scene.
    // Can be rendered, updated, and can collide with other entities.
    public abstract class Entity {
        // Variables that descrive the Entity in the scene.
        protected VisualComponent visual;
        public Collider collider;

        public int Width { get { return visual.Width; } }
        public int Height { get { return visual.Height; } }

        public Vector2 position;
        public Vector2 direction;
        public float speed; // Pixels/second.

        public bool markedForDeletion { get; protected set; }

        public virtual void Update(GameTime gameTime) {
            visual.Update(gameTime);
            position += direction * speed * (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            collider.UpdatePosition(position);
        }

        public virtual void Render(SpriteBatch spriteBatch) {
            visual.Draw(spriteBatch, position);
        }

    }
}
