using Microsoft.Xna.Framework;

namespace ExodosGame {
    class BombPowerup : Powerup {
        public BombPowerup(int xPosition, int yPosition) {
            position = new Vector2(xPosition, yPosition);
            direction = new Vector2(-1, 0);
            speed = 200;

            visual = new AnimatedSprite(Renderer.Instance.powerup_bomb, 1, 8, 8, 1000, true);

            collider = new CircleCollider(position.X, position.Y, 32, 32, 32);
        }

        public override void Collect(Gameplay gameplay) {
            AudioSystem.Instance.PlaySound("powerup_bomb");
            gameplay.GetBomb();
        }
    }
}
