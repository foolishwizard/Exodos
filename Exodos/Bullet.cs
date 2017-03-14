using Microsoft.Xna.Framework;

namespace ExodosGame {
    public class Bullet : Entity {
        public Bullet(float xPosition, float yPosition, float xDirection, float yDirection, float speed) {
            position = new Vector2(xPosition, yPosition);
            direction = new Vector2(xDirection, yDirection);
            this.speed = speed;
            visual = new Sprite(Renderer.Instance.bullets, 32, 0, 32, 32);
            collider = new CircleCollider(position.X, position.Y, 26, 15, 11);
        }
    }
}
