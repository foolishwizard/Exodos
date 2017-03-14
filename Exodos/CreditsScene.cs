using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExodosGame {
    public class CreditsScene : Scene {
        string text1 = "Exodos";
        string text2 = "A game by Rafal \"Kradziej\" Rzeuski";
        string text3 = "All visual assets shamelessly stolen from:\nhttps://github.com/fabiensanglard/Prototype";
        string text4 = "\nLicensing information about fonts, music and sound\nshould be attached to this software in a separate file";

        SpriteFont title;
        SpriteFont text;

        Camera2D camera;

        bool escPressed;

        public CreditsScene() {
            camera = new Camera2D(0, 0, 0, 0, 0);

            title = Renderer.Instance.menu_title;
            text = Renderer.Instance.menu_score;
        }

        public override void Update(GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) escPressed = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Escape)) {
                bool wasPressed = escPressed;
                escPressed = false;
                if(wasPressed) {
                    AudioSystem.Instance.PlaySound("ui_back");
                    Exodos.Instance.ChangeScene("main_menu", this);
                }
            }
        }

        public override void Render(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(title, text1, new Vector2(10, 0), Color.White);
            spriteBatch.DrawString(text, text2, new Vector2(10, title.MeasureString(text1).Y + 10), Color.White);

            spriteBatch.DrawString(text, text4, new Vector2(10, Exodos.Instance.ScreenHeight - text.MeasureString(text4).Y - 10), Color.White);
            spriteBatch.DrawString(text, text3, new Vector2(10, Exodos.Instance.ScreenHeight - text.MeasureString(text4).Y -text.MeasureString(text3).Y - 10), Color.White);
        }

        public override Matrix GetCameraViewMatrix() {
            return camera.GetViewMatrix();
        }
    }
}