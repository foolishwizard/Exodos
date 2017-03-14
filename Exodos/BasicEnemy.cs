using System;
using Microsoft.Xna.Framework;

namespace ExodosGame {
    class BasicEnemy : Enemy {
        public override int HP { get { return 2; } }
        public override int Value { get { return 10; } }
        public override int LethalDamage { get { return currentHP; } }

        public BasicEnemy(int xPosition, int yPosition) {
            currentHP = HP;
            position = new Vector2(xPosition, yPosition);
            direction = new Vector2(-1, 0);
            speed = 200;
            visual = new AnimatedSprite(Renderer.Instance.enemy, 4, 4, 16, 1000, true);
            collider = new AABBCollider((int)position.X + 6, (int)position.Y + 7, 50, 50);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (position.X + Width <= 0) markedForDeletion = true;
        }

        public override bool OnHit(GameScene scene, int damage = 1) {
            currentHP -= damage;
            AudioSystem.Instance.PlaySound("hit");
            if(currentHP <= 0) {
                scene.camera.Shake(250, 15, 16);
                scene.effects.Add(new ExplosionEffect((int)position.X, (int)position.Y, (int)speed));
                AudioSystem.Instance.PlaySound("explosion_small");
                return true;
            }
            scene.effects.Add(new BlueHitEffect((int)position.X, (int)position.Y));
            return false;
        }
    }
}
