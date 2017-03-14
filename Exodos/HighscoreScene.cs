using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    class HighscoreScene : Scene {
        Camera2D camera;

        string hstext = "Highscore";
        string instructiontext = "Esc - back";
        SpriteFont title;
        SpriteFont text;

        Highscore highScore;

        bool escPressed;

        public HighscoreScene() {
            highScore = Exodos.Instance.HScore;
            camera = new Camera2D(0, 0, 0, 0, 0);
            title = Renderer.Instance.menu_title;
            text = Renderer.Instance.menu_option;
        }

        public override void Update(GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) escPressed = true;
            if(escPressed && Keyboard.GetState().IsKeyUp(Keys.Escape)) {
                escPressed = false;
                AudioSystem.Instance.PlaySound("ui_back");
                Main();
            }
        }

        public override void Render(SpriteBatch spriteBatch) {
            int screenHalfWidth = Exodos.Instance.ScreenHalfWidth;
            int screenHalfHeight = Exodos.Instance.ScreenHalfHeight;

            spriteBatch.DrawString(title, hstext, new Vector2(screenHalfWidth - title.MeasureString(hstext).X / 2, 20), Color.White);
            spriteBatch.DrawString(text, instructiontext, new Vector2(Exodos.Instance.ScreenWidth - text.MeasureString(instructiontext).X - 10, Exodos.Instance.ScreenHeight - text.MeasureString(instructiontext).Y), Color.White);

            // Print high score list.
            short j = 1;
            for(int i = highScore.scores.Count - 1; i >= 0; --i) {
                string txt = j + ".   " + highScore.scores[i];
                Vector2 txtSize = text.MeasureString(txt);
                spriteBatch.DrawString(text, txt, new Vector2(screenHalfWidth - txtSize.X / 2, 240 + txtSize.Y * (j - 1)), Color.White);
                ++j;
            }
        }

        void Main() {
            Exodos.Instance.ChangeScene("main_menu", this);
        }
        
        public override Matrix GetCameraViewMatrix() {
            return camera.GetViewMatrix();
        }
    }
}
