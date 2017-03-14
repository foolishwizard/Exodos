using Microsoft.Xna.Framework;

namespace ExodosGame {
    class BlueHitEffect : VisualEffect {
        public BlueHitEffect(int xPosition, int yPosition) {
            position = new Vector2(xPosition, yPosition);
            animation = new AnimatedSprite(Renderer.Instance.blueHit, 2, 4, 5, 500, false);
        }
    }
}
