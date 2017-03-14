using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExodosGame {
    public class PauseScene : Scene {
        string title = "Paused";

        SpriteFont titleFont;
        SpriteFont optionFont;

        Camera2D camera;

        MenuOption resume;
        MenuOption restart;
        MenuOption options;
        MenuOption main;
        MenuOption quit;

        MenuCursor cursor;
        bool escPressed;

        public PauseScene() {
            titleFont = Renderer.Instance.menu_title;
            optionFont = Renderer.Instance.menu_option;
            camera = new Camera2D(0, 0, 0, 0, 0);

            int screenHalfWidth = Exodos.Instance.ScreenHalfWidth;

            resume = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Resume").X / 2, 240, "Resume", optionFont);
            restart = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Restart").X / 2, 320, "Restart", optionFont);
            options = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Options").X / 2, 400, "Options", optionFont);
            main = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Main menu").X / 2, 480, "Main menu", optionFont);
            quit = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Quit").X / 2, 560, "Quit", optionFont);

            resume.SetTransitions(quit, restart);
            restart.SetTransitions(resume, options);
            options.SetTransitions(restart, main);
            main.SetTransitions(options, quit);
            quit.SetTransitions(main, resume);

            cursor = new MenuCursor(resume);
        }

        public override void Update(GameTime gameTime) {
            camera.Update(gameTime);

            switch(cursor.ProcessInput()) {
                case 0:
                    break;
                case 1:
                    AcceptButtonPress();
                    break;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) escPressed = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Escape)) {
                bool wasPressed = escPressed;
                escPressed = false;
                if (wasPressed) BackButtonPress();
            }
        }

        public override void Render(SpriteBatch spriteBatch) {
            cursor.Draw(spriteBatch);

            spriteBatch.DrawString(titleFont, title, new Vector2(Exodos.Instance.GraphicsDevice.Viewport.Width / 2 - titleFont.MeasureString(title).X / 2, 20), Color.White);

            resume.Draw(spriteBatch);
            restart.Draw(spriteBatch);
            options.Draw(spriteBatch);
            main.Draw(spriteBatch);
            quit.Draw(spriteBatch);

            spriteBatch.DrawString(optionFont, "Space - accept, Esc - back", new Vector2(
                Exodos.Instance.ScreenWidth - optionFont.MeasureString("Space - accept, Esc - back").X - 10, Exodos.Instance.ScreenHeight - optionFont.MeasureString("Space - accept, Esc - back").Y), Color.White);
        }

        void AcceptButtonPress() {
            AudioSystem.Instance.PlaySound("ui_accept");
            if (cursor.currentOption == resume) Resume();
            else if (cursor.currentOption == restart) Restart();
            else if (cursor.currentOption == options) Options();
            else if (cursor.currentOption == main) Main();
            else if (cursor.currentOption == quit) Quit();
        }

        void BackButtonPress() {
            AudioSystem.Instance.PlaySound("ui_back");
            Resume();
        }

        void Resume() {
            // Resume game.
            Exodos.Instance.ChangeScene("game", this);
            AudioSystem.Instance.ResumeMusic();
        }

        void Restart() {
            Exodos.Instance.ChangeScene("game", this, true);
        }

        void Options() {
            // Go to options screen.
            Exodos.Instance.ChangeScene("options", this);
        }

        void Main() {
            // Go to main menu.
            Exodos.Instance.ChangeScene("main_menu", this);
        }

        void Quit() {
            Exodos.Instance.Exit();
        }

        public override Matrix GetCameraViewMatrix() {
            return camera.GetViewMatrix();
        }

    }
}