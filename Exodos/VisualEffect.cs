using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// A simple wrapper for the AnimatedSprite class that also holds a position.
// Extend this to create different visual effects, like explosions.
namespace ExodosGame {
    public abstract class VisualEffect {
        protected AnimatedSprite animation;
        public Vector2 position;
        protected Vector2 direction;
        protected int speed;

        public float Width { get { return animation.Width; } }
        public float Height { get { return animation.Height; } }

        public bool Finished { get { return animation.Finished; } }
        
        public virtual void Update(GameTime gameTime) {
            animation.Update(gameTime);
            position += direction * speed * gameTime.ElapsedGameTime.Milliseconds / 1000f;
        }

        public void Render(SpriteBatch spriteBatch) { animation.Draw(spriteBatch, position); }
    }
}
