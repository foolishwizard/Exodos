using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    class OnOffSwitch {
        Vector2 position;
        public bool On { get; private set; }
        Sprite onSprite;
        Sprite offSprite;

        public delegate void actionType();
        actionType action;

        public OnOffSwitch(int xPosition, int yPosition, bool on, actionType action) {
            position = new Vector2(xPosition, yPosition);
            On = on;
            onSprite = new Sprite(Renderer.Instance.switch_on, 0, 25, 128, 80);
            offSprite = new Sprite(Renderer.Instance.switch_off, 0, 25, 128, 80);
            this.action = action;
        }

        public void Draw(SpriteBatch spriteBatch) {
            if (On) onSprite.Draw(spriteBatch, position);
            else offSprite.Draw(spriteBatch, position);
        }

        public void Toggle() {
            On = !On;
            action();
        }
    }
}
