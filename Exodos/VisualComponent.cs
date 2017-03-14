using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Represents a component of an Entity that can be rendered on screen.
// Used for polymorhpism so we can have Entities with different types of sprites (animated, static).
namespace ExodosGame {
    public abstract class VisualComponent {
        abstract public int Width { get; }
        abstract public int Height { get; }
        protected Color color;
        public abstract void Draw(SpriteBatch spriteBatch, Vector2 position, bool flipped = false);
        public virtual void Update(GameTime gameTime) {}
    }
}
