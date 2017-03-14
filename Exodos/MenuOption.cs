using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Represents an option in the menu. Has an on-screen position and can be drawn.
namespace ExodosGame {
    public class MenuOption {
        public Vector2 position;
        string text;
        SpriteFont font;

        public int Size { get { return (int)font.MeasureString(text).X; } }

        public MenuOption up { get; private set; }
        public MenuOption down { get; private set; }

        public MenuOption(int xPosition, int yPosition, string text, SpriteFont font) {
            position = new Vector2(xPosition, yPosition);
            this.text = text;
            this.font = font;
        }

        public void SetTransitions(MenuOption up, MenuOption down) {
            this.up = up;
            this.down = down;
        }

        public void Draw(SpriteBatch spriteBatch) {
            spriteBatch.DrawString(font, text, position, Color.White);
        }
    }
}