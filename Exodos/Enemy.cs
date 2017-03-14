// Base class for all enemies.
// Extends an Entity with hit points and score value.
namespace ExodosGame {
    public abstract class Enemy : Entity {
        public abstract int HP { get; }
        public abstract int Value { get; }
        public abstract int LethalDamage { get; }
        protected int currentHP;

        // What happens when an enemy is hit.
        // Returns true if the hit was lethal.
        public abstract bool OnHit(GameScene scene, int damage = 1);
    }
}
