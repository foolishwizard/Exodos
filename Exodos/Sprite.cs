using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    public class Sprite : VisualComponent {
        protected Texture2D texture;
        protected Rectangle sourceRectangle;

        public override int Width { get { return sourceRectangle.Width; } }
        public override int Height { get { return sourceRectangle.Height; } }

        public Sprite(Texture2D texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight) {
            this.texture = texture;
            color = Color.White;
            sourceRectangle = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
        }

        public Sprite(Texture2D texture, int sourceX, int sourceY, int sourceWidth, int sourceHeight, Color color) {
            this.texture = texture;
            this.color = color;
            sourceRectangle = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, bool flipped = false) {
            if(flipped) {
                spriteBatch.Draw(
                    texture,
                    position,
                    sourceRectangle,
                    color,
                    0,
                    new Vector2(),
                    1,
                    SpriteEffects.FlipHorizontally,
                    0);
            }
            else {
                spriteBatch.Draw(texture, position, sourceRectangle, color);
            }
        }
    }
}
