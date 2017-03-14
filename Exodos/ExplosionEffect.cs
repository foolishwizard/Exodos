using Microsoft.Xna.Framework;

namespace ExodosGame {
    class ExplosionEffect : VisualEffect {
        public ExplosionEffect(float xPosition, float yPosition, int speed) {
            position = new Vector2(xPosition, yPosition);
            animation = new AnimatedSprite(Renderer.Instance.explosion, 4, 8, 28, 1000, false);
            direction = new Vector2(-1, 0);
            this.speed = speed;
        }
    }
}
