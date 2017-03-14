using Microsoft.Xna.Framework;

namespace ExodosGame {
    class BigExplosionEffect : VisualEffect {
        public BigExplosionEffect(float xPosition, float yPosition, int speed) {
            position = new Vector2(xPosition, yPosition);
            animation = new AnimatedSprite(Renderer.Instance.explosion_large, 4, 8, 28, 1000, false);
            direction = new Vector2(-1, 0);
            this.speed = speed;
        }
    }
}