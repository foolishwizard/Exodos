using System;
using Microsoft.Xna.Framework;

namespace ExodosGame {
    public class BugEnemy : Enemy {
        public override int HP { get { return 2; } }
        public override int LethalDamage { get { return currentHP; } }
        public override int Value { get { return 15; } }

        // How long should the enemy go up or down before changing direction.
        int directionTimer;
        const int directionTime = 1000;

        public BugEnemy(int xPosition, int yPosition) {
            position = new Vector2(xPosition, yPosition);
            currentHP = HP;
            speed = 150;
            visual = new AnimatedSprite(Renderer.Instance.bug, 4, 4, 16, 1000, true);
            collider = new AABBCollider((int)position.X + 6, (int)position.Y + 2, 52, 60);
            if (Exodos.Instance.random.Next(0, 2) == 1) {
                direction = new Vector2(-1, 1);
            } else {
                direction = new Vector2(1, 1);
            }
            direction.Normalize();
            directionTimer = directionTime;
        }

        public override void Update(GameTime gameTime) {
            visual.Update(gameTime);

            position += direction * speed * (gameTime.ElapsedGameTime.Milliseconds / 1000f);

            directionTimer -= gameTime.ElapsedGameTime.Milliseconds;
            if(directionTimer <= 0 || position.Y <= 0 || (position.Y + Height >= Exodos.Instance.ScreenHeight)) {
                direction.Y = -direction.Y;
                directionTimer = directionTime;
            }

            if (position.X + Width < 0) markedForDeletion = true;

            collider.UpdatePosition(position);
        }

        public override bool OnHit(GameScene scene, int damage = 1) {
            currentHP -= damage;
            AudioSystem.Instance.PlaySound("hit");
            if (currentHP <= 0) {
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