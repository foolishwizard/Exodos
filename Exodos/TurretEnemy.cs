using Microsoft.Xna.Framework;

namespace ExodosGame {
    public class TurretEnemy : Enemy {
        public override int HP { get { return 10; } }
        public override int LethalDamage { get { return currentHP; } }
        public override int Value { get { return 50; } }

        // The enemy behaves as follows:
        // - enters the scene until it reaches a designated point
        // - it stays in the scene, shooting in the direction of the player, for a specified amount of time
        // - (if not killed) it leaves the scene in the same direction it entered.
        enum State { Entering, Shooting, Exiting }
        State state;

        // Point at which the enemy will stay on the screen.
        Vector2 shootingPoint;
        // Direction from the spawn point to the shooting designation. Will be used to later exit the scene.
        Vector2 entryVector;
        // Time, in milliseconds, for which the enemy will stay in the scene.
        int sceneTime;

        // Point from which the enemy will shoot bullets.
        static Vector2 bulletSpawnOffset = new Vector2(-12, 48);
        GameScene scene;
        const int shootingInterval = 500;
        int timeSinceShot;

        const float moveSpeed = 400;

        public TurretEnemy(int xPosition, int yPosition, int xDestination, int yDestination, int sceneTime, GameScene scene) {
            position = new Vector2(xPosition, yPosition);
            shootingPoint = new Vector2(xDestination, yDestination);
            direction = shootingPoint - position;
            direction.Normalize();
            entryVector = direction;
            this.sceneTime = sceneTime;
            state = State.Entering;

            this.scene = scene;
            timeSinceShot = 0;

            currentHP = HP;

            visual = new Sprite(Renderer.Instance.turret, 0, 0, 128, 128);

            collider = new AABBCollider((int)position.X + 8, (int)position.Y + 8, 112, 112);

            speed = moveSpeed;
        }

        public override void Update(GameTime gameTime) {
            int ms = gameTime.ElapsedGameTime.Milliseconds;

            position += direction * speed * ms / 1000f;

            collider.UpdatePosition(position);

            switch(state) {
                case State.Entering:
                    if(position.X < shootingPoint.X + 50 && position.X > shootingPoint.X - 50 &&
                        position.Y < shootingPoint.Y + 50 && position.Y > shootingPoint.Y -50) {
                        state = State.Shooting;
                        direction = new Vector2(1, 0);
                        speed = 0;
                    }
                    break;
                case State.Shooting:
                    timeSinceShot += ms;
                    sceneTime -= ms;
                    if (timeSinceShot >= shootingInterval) {
                        timeSinceShot = 0;
                        if(sceneTime <= 0) {
                            state = State.Exiting;
                            direction = -entryVector;
                            speed = moveSpeed;
                            break;
                        }
                        // Make sure the player is currently not dead.
                        else if (!scene.player.Dead) {
                            Vector2 playerPosition = scene.player.position;
                            playerPosition.X += scene.player.Width / 2;
                            playerPosition.Y += scene.player.Height / 2;
                            Vector2 shotDirection = playerPosition - (position + bulletSpawnOffset);
                            shotDirection.Normalize();
                            AudioSystem.Instance.PlaySound("turret_shot");
                            scene.enemyBullets.Add(new Fireball(position.X + bulletSpawnOffset.X, position.Y + bulletSpawnOffset.Y, shotDirection.X, shotDirection.Y, 300));
                        }
                    }
                    break;
                case State.Exiting:
                    if(position.X >= Exodos.Instance.ScreenWidth ||
                        position.Y >= Exodos.Instance.ScreenHeight ||
                        position.Y + Height <= 0) {
                        markedForDeletion = true;
                    }
                    break;
            }
        }

        public override bool OnHit(GameScene scene, int damage = 1) {
            currentHP -= damage;
            AudioSystem.Instance.PlaySound("hit");
            if(currentHP <= 0) {
                scene.camera.Shake(250, 30, 32);
                BigExplosionEffect splosion = new BigExplosionEffect(position.X, position.Y, (int)moveSpeed);
                splosion.position.X -= splosion.Width / 4;
                splosion.position.Y -= splosion.Height / 4;
                scene.effects.Add(splosion);
                AudioSystem.Instance.PlaySound("explosion_large2");
                return true;
            }
            scene.effects.Add(new BlueHitEffect((int)position.X + Width / 2, (int)position.Y + Height / 2));
            return false;
        }
    }
}