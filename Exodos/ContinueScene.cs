using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    class ContinueScene : Scene {

        Camera2D camera;

        public ContinueScene() {
            camera = new Camera2D(0, 0, 0, 0, 0);
        }

        public override void Update(GameTime gameTime) {
            if(Keyboard.GetState().IsKeyDown(Keys.Space)) {
                Exodos.Instance.CurrentScene = new GameScene();
            }
        }

        public override void Render(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(Renderer.Instance.regularFont, "Game Over!\nContinue?", new Vector2(), Color.Cyan);
        }

        
        public override Matrix GetCameraViewMatrix() {
            return camera.GetViewMatrix();
        }
    }
}
