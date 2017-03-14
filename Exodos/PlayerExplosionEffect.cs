using Microsoft.Xna.Framework;

namespace ExodosGame {
    public class PlayerExplosionEffect : VisualEffect {
        public PlayerExplosionEffect(float xPosition, float yPosition) {
            position = new Vector2(xPosition, yPosition);
            direction = new Vector2(-1, 0);
            speed = 400;
            animation = new AnimatedSprite(Renderer.Instance.explosion_player_2, 4, 8, 32, 1000, false);
            position.X -= animation.Width / 4;
            position.Y -= animation.Height / 4;
        }
    }
}