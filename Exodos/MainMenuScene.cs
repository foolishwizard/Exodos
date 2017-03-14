using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

// Represents the main menu in the game.
namespace ExodosGame {
    class MainMenuScene : Scene {
        string title = "Exodos";

        SpriteFont titleFont;
        SpriteFont optionFont;

        Camera2D camera;

        MenuOption start;
        MenuOption highscore;
        MenuOption options;
        MenuOption credits;
        MenuOption quit;

        MenuCursor cursor;
        bool escPressed;

        OptionsScene optionsScene;

        public MainMenuScene() {
            titleFont = Renderer.Instance.menu_title;
            optionFont = Renderer.Instance.menu_option;
            camera = new Camera2D(0, 0, 0, 0, 0);

            int screenHalfWidth = Exodos.Instance.ScreenHalfWidth;

            start = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Start").X / 2, 240, "Start", optionFont);
            highscore = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Highscore").X / 2, 320, "Highscore", optionFont);
            options = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Options").X / 2, 400, "Options", optionFont);
            credits = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Credits").X / 2, 480, "Credits", optionFont);
            quit = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Quit").X / 2, 560, "Quit", optionFont);

            start.SetTransitions(quit, highscore);
            highscore.SetTransitions(start, options);
            options.SetTransitions(highscore, credits);
            credits.SetTransitions(options, quit);
            quit.SetTransitions(credits, start);

            cursor = new MenuCursor(start);

            AudioSystem.Instance.PlayMusicTrack("long_road_ahead", true);
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

            // Take care of the Escape button for going back to previous menu.
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) escPressed = true;
            if(Keyboard.GetState().IsKeyUp(Keys.Escape)) {
                bool wasPressed = escPressed;
                escPressed = false;
                if (wasPressed) BackButtonPress();
            }
        }

        public override void Render(SpriteBatch spriteBatch) {
            cursor.Draw(spriteBatch);
            spriteBatch.DrawString(titleFont, title, new Vector2(Exodos.Instance.GraphicsDevice.Viewport.Width / 2 - titleFont.MeasureString(title).X / 2, 20), Color.White);

            start.Draw(spriteBatch);
            highscore.Draw(spriteBatch);
            options.Draw(spriteBatch);
            credits.Draw(spriteBatch);
            quit.Draw(spriteBatch);

            spriteBatch.DrawString(optionFont, "Space - accept, Esc - quit", new Vector2(
                Exodos.Instance.GraphicsDevice.Viewport.Width - optionFont.MeasureString("Space - accept, Esc - quit").X - 10, Exodos.Instance.GraphicsDevice.Viewport.Height - optionFont.MeasureString("Space - quit").Y),
                Color.White);
        }

        void Start() {
            Exodos.Instance.ChangeScene("game", this, true);
        }

        void Highscore() {
            Exodos.Instance.ChangeScene("highscore", this);
        }

        void Options() {
            if(optionsScene == null) {
                optionsScene = new OptionsScene(this);
            } else {
                optionsScene.SetPreviousScene(this);
            }
            Exodos.Instance.CurrentScene = optionsScene;
        }

        void Credits() {
            Exodos.Instance.ChangeScene("credits", this);
        }

        void Quit() {
            Exodos.Instance.Exit();
        }

        void AcceptButtonPress() {
            AudioSystem.Instance.PlaySound("ui_accept");
            if (cursor.currentOption == start) Start();
            else if (cursor.currentOption == highscore) Highscore();
            else if (cursor.currentOption == options) Options();
            else if (cursor.currentOption == credits) Credits();
            else if (cursor.currentOption == quit) Quit();
        }

        void BackButtonPress() {
            AudioSystem.Instance.PlaySound("ui_back");
            Quit();
        }

        public override Matrix GetCameraViewMatrix() {
            return camera.GetViewMatrix();
        }
    }
}
