using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

// Governs the rules of the game.
namespace ExodosGame {
    public class Gameplay {
        GameScene scene;

        Random random;

        // Description of the player's current state.
        public long Score { get; private set; } // Points earned by the player.
        public long HighScore { get; private set; }
        public short Lives { get; private set; }
        public short Bombs { get; private set; }
        public short WeaponLevel { get; private set; }

        public int WeaponExp { get; private set; }
        public const int weaponLevelUp = 100;

        bool bombPressed;

        int gameOverTime = 1000;

        public Gameplay(GameScene gameScene) {
            scene = gameScene;

            // @todo: Use the random generator from the game class.
            random = new Random();

            Score = 0;
            HighScore = Exodos.Instance.HScore.GetTop();
            Lives = 3;
            Bombs = 3;
            WeaponLevel = 1;
            WeaponExp = 75;
        }

        // Values should be updated with time and some event need to happen at specific moments, so: Update function.
        public void Update(GameTime gameTime) {
            if(Keyboard.GetState().IsKeyDown(Keys.D1)) {
                BasicEnemy e = new BasicEnemy((int)scene.camera.position.X + 1280, random.Next(0, 660));
                scene.enemies.Add(e);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D)) bombPressed = true;
            else if(bombPressed && Keyboard.GetState().IsKeyUp(Keys.D)) {
                bombPressed = false;
                if(Bombs > 0) {
                    Bombs--;
                    scene.bombs.Add(new Bomb((int)scene.player.position.X, (int)scene.player.position.Y));
                }
            }

            if(Lives < 0 && scene.player.Dead) {
                gameOverTime -= gameTime.ElapsedGameTime.Milliseconds;
                if (gameOverTime <= 0) GameOver();
            }
        }

        public void OnEnemyDeath(Enemy enemy) {
            Score += enemy.Value;
        }

        public void GameOver() {
            // Go to game over scene, take care of the score and stuff.
            Exodos.Instance.ChangeScene("game_over", scene, true);
            Exodos.Instance.HScore.AddScore(Score);
        }

        public void OneUp() {
            Lives++;
        }

        public void GetBomb() {
            Bombs++;
        }

        public void GetWeapon() {
            if(WeaponLevel >= 4) {
                Score += 100;
                return;
            }
            WeaponExp += 25;
            if(WeaponExp == weaponLevelUp) {
                WeaponLevel++;
                WeaponExp = 0;
                scene.player.UpgradeWeapon();
            }
        }

        public void OnPlayerDeath() {
            Lives--;
        }
    }
}
