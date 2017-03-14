using Microsoft.Xna.Framework;

namespace ExodosGame {
    // Bomb that can be used by the player to clear the screen of enemies.
    // Implemented as an entity with a huge circle collider to represent
    // its blas radius.
    public class Bomb : Entity {
        int lifems = 2000;

        public Bomb(int xPosition, int yPosition) {
            position = new Vector2(xPosition, yPosition);

            visual = null;

            collider = new CircleCollider(position.X, position.Y, 0, 0, 128);

            AudioSystem.Instance.PlaySound("ui_back");
        }

        public new void Update(GameTime gameTime) {
            int ms = gameTime.ElapsedGameTime.Milliseconds;
            lifems -= ms;
            if (lifems <= 0) markedForDeletion = true;
            ((CircleCollider)collider).Radius += (int)(750 * (ms / 1000f));
        }
    }
}