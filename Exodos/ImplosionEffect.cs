using Microsoft.Xna.Framework;

namespace ExodosGame {
    public class ImplosionEffect : VisualEffect {
        public ImplosionEffect(float xPosition, float yPosition) {
            position = new Vector2(xPosition, yPosition);
            animation = new AnimatedSprite(Renderer.Instance.implosion, 4, 8, 32, 1500, false);
        }
    }
}