using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    public class FillUpBar {
        Vector2 position;

        Sprite bar;
        Texture2D fill;

        int fillValue;
        const int maxValue = 100;

        public FillUpBar(int xPosition, int yPosition) {
            position = new Vector2(xPosition, yPosition);
            bar = new Sprite(Renderer.Instance.fill_bar, 0, 0, Renderer.Instance.fill_bar.Width, Renderer.Instance.fill_bar.Height);
            fill = Renderer.Instance.bar_fill;

            fillValue = 50;
        }

        public void UpdateValue(int xp) {
            fillValue = xp;
            if (fillValue > maxValue) fillValue -= maxValue;
        }

        public void Draw(SpriteBatch spriteBatch) {
            bar.Draw(spriteBatch, position);
            int barLength = bar.Width;
            spriteBatch.Draw(fill, new Rectangle((int)position.X, (int)position.Y, (int)((float)fillValue / maxValue * barLength), 100), Color.White);
        }
    }
}