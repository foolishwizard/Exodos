using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    public class Fireball : Entity {
        public Fireball(float xPosition, float yPosition, float xDirection, float yDirection, float speed) {
            position = new Vector2(xPosition, yPosition);
            direction = new Vector2(xDirection, yDirection);
            this.speed = speed;
            visual = new Sprite(Renderer.Instance.bullets, 32, 32, 32, 32);
            collider = new CircleCollider(position.X, position.Y, 1, 6, 18);
        }

        public override void Render(SpriteBatch spriteBatch) {
            visual.Draw(spriteBatch, position, true);
        }
    }
}