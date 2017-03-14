using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// The class used for rendering the UI during gameplay.
// All of the text and sprites are drawn with a separate sprite batch
// than the rest of the game so they're not affected by screen shake.
namespace ExodosGame {
    class GameUI {
        Gameplay gameplay;

        FillUpBar expBar;

        // The text to be rendered!
        string score = "Score: ";
        string highScore = "High score: ";
        string lives = "Lives: ";
        string bombs = "Bombs: ";
        string weaponLevel = "Weapon: ";
        string tutorial1 = "S - shoot, D - bomb";
        string tutorial2 = "Good luck!";

        int tutorial1ms = 2000;
        int tutorial2ms = 2000;

        SpriteFont font;

        // On-screen locations of the text.
        Vector2 scorePosition;
        Vector2 highScorePosition;
        Vector2 livesPosition;
        Vector2 bombsPosition;
        Vector2 weaponLevelPosition;
        Vector2 tutorial1Position;
        Vector2 tutorial2Position;

        public GameUI(Gameplay gameplay) {
            this.gameplay = gameplay;

            font = Renderer.Instance.ui;

            int screenWidth = Exodos.Instance.GraphicsDevice.Viewport.Width;
            int screenHeight = Exodos.Instance.GraphicsDevice.Viewport.Height;
            float textHeight = Renderer.Instance.regularFont.MeasureString("A").Y;
            float livesWidth = Renderer.Instance.regularFont.MeasureString(lives + "A").X;
            float bombsWidth = Renderer.Instance.regularFont.MeasureString(bombs + "AA").X;
            float weaponWidth = Renderer.Instance.regularFont.MeasureString(weaponLevel + "AA").X;

            scorePosition = new Vector2(10, 0);
            highScorePosition = new Vector2(screenWidth / 2, 0);
            livesPosition = new Vector2(10, screenHeight - textHeight);
            bombsPosition = new Vector2(livesPosition.X + livesWidth + 20, screenHeight - textHeight);
            weaponLevelPosition = new Vector2(bombsPosition.X + bombsWidth, screenHeight - textHeight);
            tutorial1Position = new Vector2(Exodos.Instance.ScreenHalfWidth - font.MeasureString(tutorial1).X / 2, Exodos.Instance.ScreenHalfHeight / 2 - font.MeasureString(tutorial1).Y / 2);
            tutorial2Position = new Vector2(Exodos.Instance.ScreenHalfWidth - font.MeasureString(tutorial2).X / 2, Exodos.Instance.ScreenHalfHeight / 2 - font.MeasureString(tutorial2).Y / 2);

            Vector2 expPosition = new Vector2(font.MeasureString(weaponLevel + "W").X + weaponLevelPosition.X + 20, weaponLevelPosition.Y);
            expBar = new FillUpBar((int)expPosition.X, (int)expPosition.Y);
        }

        public void Update(GameTime gameTime) {
            expBar.UpdateValue(gameplay.WeaponExp);
            int ms = gameTime.ElapsedGameTime.Milliseconds;
            if (tutorial1ms > 0) tutorial1ms -= ms;
            else if (tutorial2ms > 0) tutorial2ms -= ms;
        }

        public void Render(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(font, score + gameplay.Score, scorePosition, Color.White);
            spriteBatch.DrawString(font, highScore + gameplay.HighScore, highScorePosition, Color.White);
            spriteBatch.DrawString(font, lives + gameplay.Lives, livesPosition, Color.White);
            spriteBatch.DrawString(font, bombs + gameplay.Bombs, bombsPosition, Color.White);
            spriteBatch.DrawString(font, weaponLevel + gameplay.WeaponLevel, weaponLevelPosition, Color.White);
            expBar.Draw(spriteBatch);

            if(tutorial1ms > 0) {
                spriteBatch.DrawString(font, tutorial1, tutorial1Position, Color.White);
            } else if(tutorial2ms > 0) {
                spriteBatch.DrawString(font, tutorial2, tutorial2Position, Color.White);
            }
        }
    }
}
