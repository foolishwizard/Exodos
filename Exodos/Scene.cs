using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Represents a single scene in the game. Extend this to create different kinds of scenes
// (Game, Main Menu, Options Menu, others).
// Can be rendered and updated.
// Switch between the currently updated and rendered scene in the main game class to change scenes.
namespace ExodosGame {
    public abstract class Scene {
        protected Scene previousScene;
        public abstract void Update(GameTime gameTime);
        public abstract void Render(SpriteBatch spriteBatch);
        public virtual void RenderUI(SpriteBatch spriteBatch) {}
        public abstract Matrix GetCameraViewMatrix();
        public virtual void SetPreviousScene(Scene previousScene) {
            this.previousScene = previousScene;
        }
    }
}
