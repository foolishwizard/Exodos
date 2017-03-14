using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    public class AnimatedSprite : VisualComponent {
        Texture2D texture;

        // Duration of the entire animation and of the currently rendered frame. Both in milliseconds.
        int duration;
        int currentFrameDuration;
        int frames;

        int rows;
        int columns;
        int currentFrame;

        public bool Looping { get; set; }
        public bool Finished { get; private set; }

        public override int Width { get { return texture.Width / columns; } }
        public override int Height { get { return texture.Height / rows; } }

        public AnimatedSprite(Texture2D texture, int rows, int columns, int frames, int duration, bool looping) {
            this.texture = texture;
            this.rows = rows;
            this.columns = columns;
            this.frames = frames; // In case the sprite atlas has empty frames.
            this.duration = duration;
            Looping = looping;
            Finished = false;
            currentFrame = 0;
            currentFrameDuration = 0;
        }

        public override void Update(GameTime gameTime) {
            if (Finished) return;

            int ms = gameTime.ElapsedGameTime.Milliseconds;
            currentFrameDuration += ms;
            if(currentFrameDuration >= duration / frames) {
                currentFrame++;
                currentFrameDuration = 0;
                if (Looping) {
                    currentFrame %= frames;
                } else if(currentFrame == frames) {
                    Finished = true;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 position, bool flipped = false) {
            int c = currentFrame / columns;
            int r = currentFrame - (c * columns);
            spriteBatch.Draw(texture, position, new Rectangle(r * Width, c * Height, Width, Height), Color.White);
        }
    }
}