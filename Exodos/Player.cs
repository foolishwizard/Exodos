using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

// Represents the player's ship in the scene.
namespace ExodosGame {
    public class Player : Entity {
        Vector2 shotSpawnOffset = new Vector2(96, 52);
        int timeToShot;

        AnimatedSprite exhaust;
        Vector2 exhaustOffset;

        GameScene scene;

        // When the player is dead or respawning, they can't be hit.
        enum State { Alive, Dead, WaitingForSpawn, Invulnerable };
        State state;
        public bool Dead { get { return (state == State.Dead || state == State.WaitingForSpawn); } }
        public bool Invulnerable { get { return state == State.Invulnerable; } }
        int invulnerableTimer;
        const int invulnerableTime = 3000; // How long the player is invulnerable after respawning.
        Sprite invulnerableSprite;
        Sprite normalSprite;
        VisualEffect explosion;
        VisualEffect implosion;

        // Manages weapons.
        enum Weapon { Peashooter, Rapidfire, Doubleshot, Ryno };
        Weapon weapon;

        public Player(int x, int y, GameScene scene) {
            normalSprite = new Sprite(Renderer.Instance.player, 0, 0, 128, 128);
            visual = normalSprite;
            position = new Vector2(x, y);
            speed = 400;
            timeToShot = 0;

            // Create the vertices that makes up the player's collider.
            List<Vector2> verts = new List<Vector2>();
            verts.Add(new Vector2(6, 15));
            verts.Add(new Vector2(23, 33));
            verts.Add(new Vector2(40, 33));
            verts.Add(new Vector2(41, 31));
            verts.Add(new Vector2(60, 31));
            verts.Add(new Vector2(74, 36));
            verts.Add(new Vector2(93, 53));
            verts.Add(new Vector2(106, 58));
            verts.Add(new Vector2(115, 66));
            verts.Add(new Vector2(111, 73));
            verts.Add(new Vector2(67, 77));
            verts.Add(new Vector2(57, 84));
            verts.Add(new Vector2(50, 84));
            verts.Add(new Vector2(44, 97));
            verts.Add(new Vector2(33, 97));
            verts.Add(new Vector2(5, 113));
            // Assign it.
            collider = new PolygonCollider(position.X, position.Y, verts);

            exhaust = new AnimatedSprite(Renderer.Instance.exhaust, 2, 8, 16, 1000, true);
            exhaustOffset = new Vector2(-exhaust.Width + 20, exhaust.Height / 2 + 5);
            invulnerableSprite = new Sprite(Renderer.Instance.player, 0, 0, 128, 128, Color.Red);
            state = State.Alive;

            weapon = Weapon.Peashooter;

            this.scene = scene;
        }

        public new void Update(GameTime gameTime) {
            switch(state) {
                case State.Dead:
                    if (explosion.Finished) StartRespawn();
                    return;
                case State.WaitingForSpawn:
                    if (implosion.Finished) Respawn();
                    return;
                case State.Invulnerable:
                    invulnerableTimer += gameTime.ElapsedGameTime.Milliseconds;
                    if (invulnerableTimer >= invulnerableTime) FinishRespawn();
                    break;
            }

            visual.Update(gameTime);
            exhaust.Update(gameTime);

            float deltaTime = gameTime.ElapsedGameTime.Milliseconds / 1000f; // In seconds

            // The if clause makes sure we don't loop back to positive values after not shooting for long.
            if (timeToShot > 0) timeToShot -= gameTime.ElapsedGameTime.Milliseconds;

            Vector2 inputDirection = new Vector2();
            if(Keyboard.GetState().IsKeyDown(Keys.Right)) {
                inputDirection.X += 1;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Left)) {
                inputDirection.X -= 1;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Up)) {
                inputDirection.Y -= 1;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.Down)) {
                inputDirection.Y += 1;
            }
            if(Keyboard.GetState().IsKeyDown(Keys.S)) {
                Shoot();
            }

            direction = inputDirection;
            Vector2 oldPosition = position;
            if(direction.X != 0 && direction.Y != 0) direction.Normalize();
            position += direction * speed * deltaTime;
            if (position.X + Width / 2 <= 0 || position.X + Width / 2 >= Exodos.Instance.ScreenWidth ||
                position.Y + Height / 2 <= 0 || position.Y + Height / 2 >= Exodos.Instance.ScreenHeight) {
                position = oldPosition;
            }

            collider.UpdatePosition(position);
        }

        public override void Render(SpriteBatch spriteBatch) {
            base.Render(spriteBatch);
            exhaust.Draw(spriteBatch, position + exhaustOffset);
        }

        public void Die() {
            state = State.Dead;
            explosion = new PlayerExplosionEffect(position.X, position.Y);
            scene.effects.Add(explosion);
            scene.camera.Shake(1000, 60, 16);
            AudioSystem.Instance.PlaySound("explosion_large2");
        }

        void StartRespawn() {
            state = State.WaitingForSpawn;
            implosion = new ImplosionEffect(exhaust.Width, Exodos.Instance.ScreenHalfHeight - Height / 2);            
            scene.effects.Add(implosion);
            AudioSystem.Instance.PlaySound("respawn");
        }

        void Respawn() {
            state = State.Invulnerable;
            invulnerableTimer = 0;
            position = new Vector2(exhaust.Width, Exodos.Instance.ScreenHalfHeight - Height / 2);
            visual = invulnerableSprite;
        }

        void FinishRespawn() {
            state = State.Alive;
            visual = normalSprite;
        }

        void Shoot() {
            switch(weapon) {
                case Weapon.Peashooter:
                    Peashooter();
                    break;
                case Weapon.Rapidfire:
                    Rapidfire();
                    break;
                case Weapon.Doubleshot:
                    Doubleshot();
                    break;
                case Weapon.Ryno:
                    Ryno();
                    break;
            }
        }

        void Peashooter() {
            const int peashooterInterval = 300;
            if(timeToShot <= 0) {
                // Spawn a bullet.
                Bullet b = new Bullet(position.X + shotSpawnOffset.X, position.Y + shotSpawnOffset.Y, 1, 0, 800);
                scene.playerBullets.Add(b);
                // Reset time.
                timeToShot = peashooterInterval;
                // Play a sound.
                AudioSystem.Instance.PlaySoundWithPitch("shot", 0.84f);
            }
        }

        void Rapidfire() {
            const int rapidFireInterval = 100;
            if (timeToShot <= 0) {
                // Spawn a bullet.
                Bullet b = new Bullet(position.X + shotSpawnOffset.X, position.Y + shotSpawnOffset.Y, 1, 0, 800);
                scene.playerBullets.Add(b);
                // Reset time.
                timeToShot = rapidFireInterval;
                // Play sound.
                AudioSystem.Instance.PlaySoundWithPitch("shot", 0.64f);
            }
        }

        void Doubleshot() {
            const int doubleShotInterval = 150;
            if(timeToShot <= 0) {
                // Calculate where to spawn the bullets.
                Vector2 spawn1 = shotSpawnOffset;
                Vector2 spawn2 = shotSpawnOffset;
                spawn1.Y -= 10;
                spawn2.Y += 10;
                Bullet b = new Bullet(position.X + spawn1.X, position.Y + spawn1.Y, 1, 0, 800);
                scene.playerBullets.Add(b);
                b = new Bullet(position.X + spawn2.X, position.Y + spawn2.Y, 1, 0, 800);
                scene.playerBullets.Add(b);
                // Reset time.
                timeToShot = doubleShotInterval;
                // Play the sound.
                AudioSystem.Instance.PlaySoundWithPitch("shot", 0.0f);
            }
        }

        // Shout out to Ratchet & Clank!
        void Ryno() {
            const int bfgInterval = 100;
            if(timeToShot <= 0) {
                // Calculate where to spawn bullets.
                Vector2 spawn1 = shotSpawnOffset;
                Vector2 spawn2 = shotSpawnOffset;
                spawn1.Y -= 10;
                spawn2.Y += 10;

                Bullet b = new Bullet(position.X + spawn1.X, position.Y + spawn1.Y, 1, 0, 800);
                scene.playerBullets.Add(b);

                b = new Bullet(position.X + spawn2.X, position.Y + spawn2.Y, 1, 0, 800);
                scene.playerBullets.Add(b);

                // Shoot at 30-degree angles up and down.
                Vector2 vec = new Vector2(1, (1f / 1.73f));
                vec.Normalize();
                b = new Bullet(position.X + spawn1.X, position.Y + spawn1.Y, vec.X, vec.Y, 800);
                scene.playerBullets.Add(b);
                b = new Bullet(position.X + spawn2.X, position.Y + spawn2.Y, vec.X, -vec.Y, 800);
                scene.playerBullets.Add(b);

                // Reset time.
                timeToShot = bfgInterval;

                // Play sound. Pitch it down so we know it's a powerful weapon.
                AudioSystem.Instance.PlaySoundWithPitch("shot", -0.5f);
            }
        }

        public void UpgradeWeapon() {
            switch(weapon) {
                case Weapon.Peashooter:
                    weapon = Weapon.Rapidfire;
                    break;
                case Weapon.Rapidfire:
                    weapon = Weapon.Doubleshot;
                    break;
                case Weapon.Doubleshot:
                    weapon = Weapon.Ryno;
                    break;
                case Weapon.Ryno:
                    break;
            }
        }
    }
}
