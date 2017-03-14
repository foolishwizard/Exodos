using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    class ValueChanger {
        public Vector2 position { get; private set; }
        public int Value { get; private set; }
        int max;
        int min;
        SpriteFont font;

        // Delegate type for the action performed by the changer.
        public delegate void actionType();
        actionType action;

        public ValueChanger(int xPosition, int yPosition, int startValue, int maxValue, int minValue, actionType action) {
            position = new Vector2(xPosition, yPosition);
            Value = startValue;
            max = maxValue;
            min = minValue;
            font = Renderer.Instance.menu_option;
            this.action = action;
        }

        public void Draw(SpriteBatch spriteBatch) {
            if(Value < 10) {
                spriteBatch.DrawString(font, "  " + Value, position, Color.White);
            } else if(Value < 100) {
                spriteBatch.DrawString(font, " " + Value, position, Color.White);
            } else {
                spriteBatch.DrawString(font, "" + Value, position, Color.White);
            }
        }

        public void Decrement() {
            Value--;
            Value = MathHelper.Clamp(Value, min, max);
            action();
        }

        public void Increment() {
            Value++;
            Value = MathHelper.Clamp(Value, min, max);
            action();
        }
    }
}
