using Microsoft.Xna.Framework;

namespace ExodosGame {
    class CircleCollider : Collider {
        public int Radius { get; set; } // In screen pixels, so int.
        public Vector2 center { get; private set; } // Offset of the circle's center from the position.

        public CircleCollider(float xPosition, float yPosition, float xCenter, float yCenter, int radius) {
            position = new Vector2(xPosition, yPosition);
            center = new Vector2(xCenter, yCenter);
            Radius = radius;
        }

        public override void UpdatePosition(Vector2 newPosition) {
            position = newPosition;
        }
    }
}
