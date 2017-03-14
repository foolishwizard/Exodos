using System;
using Microsoft.Xna.Framework;

namespace ExodosGame {
    public class PowerupSpawner {
        int timeToSpawn;
        GameScene scene;

        public PowerupSpawner(GameScene scene, int initialDelay) {
            this.scene = scene;
            timeToSpawn = initialDelay;
        }

        public void Update(GameTime gameTime) {
            timeToSpawn -= gameTime.ElapsedGameTime.Milliseconds;
            if (timeToSpawn <= 0) {
                SpawnPowerup();
            }
        }

        void SpawnPowerup() {
            Random r = Exodos.Instance.random;

            // Randomize the powerups spawn position.
            int x = Exodos.Instance.ScreenWidth + 100;
            int y = r.Next(0, Exodos.Instance.ScreenHeight - 64);
            // Decide which powerup to spawn.
            Powerup p;
            int val = r.Next(0, 7);
            if (val == 0) {
                p = new LifePowerup(x, y);
            } else if (val < 3) {
                p = new BombPowerup(x, y);
            } else {
                p = new WeaponPowerup(x, y);
            }
            scene.powerups.Add(p);

            // Randomize when to spawn a powerup again.
            timeToSpawn = r.Next(5000, 7000);
        }
    }
}