using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    public class GameOverScene : Scene {
        Camera2D camera;

        MenuOption retry;
        MenuOption main;

        MenuCursor cursor;

        string gotext = "Game Over";
        string yourScore = "Your score: ";

        SpriteFont title;
        SpriteFont text;
        SpriteFont scoreFont;

        // Reference to the Highscore object held by the game class.
        Highscore highScore;

        public GameOverScene() {
            highScore = Exodos.Instance.HScore;
            camera = new Camera2D(0, 0, 0, 0, 0);
            title = Renderer.Instance.menu_title;
            text = Renderer.Instance.menu_option;
            scoreFont = Renderer.Instance.menu_score;

            int screenHalfWidth = Exodos.Instance.ScreenHalfWidth;

            retry = new MenuOption(screenHalfWidth - (int) text.MeasureString("Retry").X / 2, 480, "Retry", text);
            main = new MenuOption(screenHalfWidth - (int) text.MeasureString("Main menu").X / 2, 560, "Main menu", text);

            retry.SetTransitions(main, main);
            main.SetTransitions(retry, retry);

            cursor = new MenuCursor(retry);
        }

        public override void Update(GameTime gameTime) {
            switch (cursor.ProcessInput()) {
                case 1:
                    AcceptButtonPress();
                    break;
            }
        }

        public override void Render(SpriteBatch spriteBatch) {
            int screenHalfWidth = Exodos.Instance.ScreenHalfWidth;
            int screenHalfHeight = Exodos.Instance.ScreenHalfHeight;

            retry.Draw(spriteBatch);
            main.Draw(spriteBatch);
            cursor.Draw(spriteBatch);

            spriteBatch.DrawString(title, gotext, new Vector2(Exodos.Instance.ScreenHalfWidth - title.MeasureString(gotext).X / 2, 20), Color.White);
            spriteBatch.DrawString(text, yourScore + highScore.GetNew(), new Vector2(Exodos.Instance.ScreenHalfWidth - text.MeasureString(yourScore + highScore.GetNew()).X / 2, 240), Color.White);

            // Print high score list.
            int j = 1;
            for(int i = highScore.scores.Count - 1; i >= 0; --i) {
                string txt = j + ".   " + highScore.scores[i];
                Vector2 txtSize = scoreFont.MeasureString(txt);
                spriteBatch.DrawString(scoreFont, txt, new Vector2(screenHalfWidth - txtSize.X / 2, 320 + txtSize.Y * (j - 1)), Color.White);
                ++j;
            }
            
            spriteBatch.DrawString(text, "Space - accept", new Vector2(Exodos.Instance.ScreenWidth - text.MeasureString("Space - accept").X - 10, Exodos.Instance.ScreenHeight - text.MeasureString("Space - accept").Y), Color.White);
        }

        void Retry() {
            Exodos.Instance.ChangeScene("game", this, true);
        }

        void Main() {
            Exodos.Instance.ChangeScene("main_menu", this, true);
        }

        void AcceptButtonPress() {
            AudioSystem.Instance.PlaySound("ui_accept");
            if (cursor.currentOption == retry) Retry();
            else if (cursor.currentOption == main) Main();
        }

        // Should I move this to the base class? It's always the same, yo.
        public override Matrix GetCameraViewMatrix() {
            return camera.GetViewMatrix();
        }
    }
}