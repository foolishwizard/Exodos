// Base class for powerups. Extends Entity with a function that represents
// the powerup's behavior.
namespace ExodosGame {
    public abstract class Powerup : Entity {
        public abstract void Collect(Gameplay gameplay);
    }
}
