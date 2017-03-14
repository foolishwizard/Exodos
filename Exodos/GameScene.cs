using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

// The scene in which the game is being played.
// Has a Layer for any kind of Entity that may appear and an instance
// of a Gameplay class that governs the game rules.
namespace ExodosGame {
    public class GameScene : Scene {
        public Player player;

        // "Layers" of the game. Layers interact only with certain other layers.
        public List<Enemy> enemies;
        public List<Bullet> playerBullets;
        public List<Entity> enemyBullets;
        public List<VisualEffect> effects;
        public List<Powerup> powerups;
        public List<Bomb> bombs;

        PowerupSpawner powerupSpawner;
        EnemySpawner enemySpawner;

        public Camera2D camera;

        Gameplay gameplay;

        GameUI ui;

        bool escPressed;

        // Timers for various scripted events. All in milliseconds.
        int startEnemiesTimer = 3000;
        bool enemiesSpawning = false;

        public GameScene() {
            enemies = new List<Enemy>();
            playerBullets = new List<Bullet>();
            enemyBullets = new List<Entity>();
            effects = new List<VisualEffect>();
            powerups = new List<Powerup>();
            bombs = new List<Bomb>();

            powerupSpawner = new PowerupSpawner(this, 10000);

            enemySpawner = new EnemySpawner(this, 1000);

            //camera = new Camera2D(128, 0, 1, 0, 200);
            camera = new Camera2D(0, 0, 0, 0, 0);
            player = new Player(50, Exodos.Instance.ScreenHalfHeight, this);
            player.position.Y -= player.Height / 2;

            gameplay = new Gameplay(this);

            ui = new GameUI(gameplay);

            AudioSystem.Instance.PlayMusicTrack("killers", true);

            // Spawn one of each powerup type to let the player know how they work.
            powerups.Add(new WeaponPowerup(Exodos.Instance.ScreenWidth, Exodos.Instance.ScreenHalfHeight));
            powerups.Add(new LifePowerup(Exodos.Instance.ScreenWidth + 200, Exodos.Instance.ScreenHalfHeight));
            powerups.Add(new BombPowerup(Exodos.Instance.ScreenWidth + 400, Exodos.Instance.ScreenHalfHeight));
            for (int i = 0; i < 3; i++) {
                powerups[i].position.Y -= powerups[i].Height / 2;
            }
        }

        public override void Update(GameTime gameTime) {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) escPressed = true;
            if (escPressed && Keyboard.GetState().IsKeyUp(Keys.Escape)) {
                escPressed = false;
                AudioSystem.Instance.PauseMusic();
                Exodos.Instance.ChangeScene("pause", this);
                return;
            }

            if(!enemiesSpawning) {
                startEnemiesTimer -= gameTime.ElapsedGameTime.Milliseconds;
                // Start spawning enemies.
                enemiesSpawning = true;
            }

            gameplay.Update(gameTime);
            camera.Update(gameTime);
            ui.Update(gameTime);
            player.Update(gameTime);
            powerupSpawner.Update(gameTime);
            enemySpawner.Update(gameTime);

            foreach(Enemy e in enemies) {
                e.Update(gameTime);
            }
            
            foreach(Bullet b in playerBullets) {
                b.Update(gameTime);
            }

            foreach(Entity b in enemyBullets) {
                b.Update(gameTime);
            }

            foreach(VisualEffect ve in effects) {
                ve.Update(gameTime);
            }

            foreach(Powerup p in powerups) {
                p.Update(gameTime);
            }

            foreach(Bomb b in bombs) {
                b.Update(gameTime);
            }

            RemoveInvisible();

            DoCollisionChecking();

            CleanupEntities();
        }

        public override void Render(SpriteBatch spriteBatch) {
            foreach (Enemy e in enemies) {
                e.Render(spriteBatch);
            }

            foreach(Bullet b in playerBullets) {
                b.Render(spriteBatch);
            }

            foreach (var ve in effects) {
                ve.Render(spriteBatch);
            }

            foreach (Entity b in enemyBullets) {
                b.Render(spriteBatch);
            }

            foreach (var p in powerups) {
                p.Render(spriteBatch);
            }

            if(!player.Dead) player.Render(spriteBatch);
        }

        public override void RenderUI(SpriteBatch spriteBatch) {
            ui.Render(spriteBatch);
        }

        private void DoCollisionChecking() {
            // Check the collisions between the enemies and player bullets.
            for(int i = 0; i < enemies.Count; i++) {
                for(int j = 0; j < playerBullets.Count; j++) {
                    if (enemies.Count == 0 || playerBullets.Count == 0) break;
                    if (Collisions.CheckCollision(enemies[i], playerBullets[j])) {
                        playerBullets.RemoveAt(j);
                        if (j != 0) j--;
                        if(enemies[i].OnHit(this)) {
                            gameplay.OnEnemyDeath(enemies[i]);
                            enemies.RemoveAt(i);
                            if (i != 0) i--;
                        }
                    }
                }
            }

            // Enemies and any deployed bombs.
            for(int i = 0; i < bombs.Count; i++) {
                for(int j = 0; j < enemies.Count; j++) {
                    if (Collisions.CheckCollision(enemies[j], bombs[i])) {
                        enemies[j].OnHit(this, enemies[j].LethalDamage);
                        gameplay.OnEnemyDeath(enemies[j]);
                        enemies.RemoveAt(j);
                        if (j != 0) --j;
                    }
                }
            }

            CheckPlayerCollisions();
        }

        void CheckPlayerCollisions() {
            if (player.Dead) return;
            // Player and powerups.
            for (int i = 0; i < powerups.Count; i++) {
                if (Collisions.CheckCollision(powerups[i], player)) {
                    powerups[i].Collect(gameplay);
                    powerups.RemoveAt(i);
                    gameplay.OnEnemyDeath(new BasicEnemy(0, 0));
                    if (i != 0) i--;
                }
            }

            if (player.Invulnerable) return;
            // Check for collision between the player and enemies.
            for (int i = 0; i < enemies.Count; i++) {
                if (Collisions.CheckCollision(player, enemies[i])) {
                    enemies[i].OnHit(this, enemies[i].LethalDamage);
                    enemies.RemoveAt(i);
                    if (i != 0) i--;
                    if(!player.Dead) {
                        player.Die();
                        gameplay.OnPlayerDeath();
                        break;
                    }
                }
            }

            // Check for collision between the player and enemy bullets.
            for(int i = 0; i < enemyBullets.Count; i++) {
                if(Collisions.CheckCollision(player, enemyBullets[i])) {
                    enemyBullets.RemoveAt(i);
                    if (i != 0) i--;
                    if(!player.Dead) {
                        player.Die();
                        gameplay.OnPlayerDeath();
                        break;
                    }
                }
            }
        }

        // Removes all the objects that are invisible (and no longer needed) from the scene.
        public void RemoveInvisible() {
            // Player bullets that have exited the scene on the right or through the top/bottom.
            for(int i = 0; i < playerBullets.Count; i++) {
                if((playerBullets[i].position.X >= camera.position.X + Exodos.Instance.GraphicsDevice.Viewport.Width) ||
                    (playerBullets[i].position.Y >= Exodos.Instance.GraphicsDevice.Viewport.Height) ||
                    (playerBullets[i].position.Y + playerBullets[i].Height <= 0)) {
                    playerBullets.RemoveAt(i);
                    i--;
                }
            }

            // Enemies that have exited the screen on the left.
            for(int i = 0; i < enemies.Count; i++) {
                if(enemies[i].position.X + enemies[i].Width <= camera.position.X) {
                    enemies.RemoveAt(i);
                    i--;
                }
            }

            // Visual effects that have finished.
            for(int i = 0; i < effects.Count; i++) {
                if(effects[i].Finished) {
                    effects.RemoveAt(i);
                    if (i != 0) i--;
                }
            }
        }

        void CleanupEntities() {
            for(int i = 0; i < bombs.Count; i++) {
                if(bombs[i].markedForDeletion) {
                    bombs.RemoveAt(i);
                    if (i != 0) i--;
                }
            }
        }

        public override Matrix GetCameraViewMatrix() {
            return camera.GetViewMatrix();
        }
    }
}