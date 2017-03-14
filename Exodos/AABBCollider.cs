using Microsoft.Xna.Framework;

namespace ExodosGame {
    class AABBCollider : Collider {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public AABBCollider(int xPosition, int yPosition, int width, int height) {
            position = new Vector2(xPosition, yPosition);
            Width = width;
            Height = height;
        }

        public override void UpdatePosition(Vector2 newPosition) {
            Vector2 direction = newPosition - position;
            position += direction;
        }
    }
}
