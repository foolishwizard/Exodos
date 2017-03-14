using System;
using Microsoft.Xna.Framework;

namespace ExodosGame {
    public class EnemySpawner {
        // Current difficulty level. Changes with time (and score?).
        enum Level { One, Two, Three, Four };
        Level lvl;

        // Timers for spawning enemies.
        int timeToSpawn;
        int timeToSpawnTurret;
        int turretSpawnInterval;
        bool turretsActive = false;
        int turretsNumber = 0;

        GameScene scene;

        int difficultyChange = 0;

        public EnemySpawner(GameScene scene, int initialDelay) {
            this.scene = scene;
            timeToSpawn = initialDelay;
            timeToSpawnTurret = Exodos.Instance.random.Next(15000, 20000);
            lvl = Level.One;
        }

        public void Update(GameTime gameTime) {
            int ms = gameTime.ElapsedGameTime.Milliseconds;
            timeToSpawn -= ms;

            if(lvl != Level.Four) {
                difficultyChange += ms;
                if(difficultyChange > 15000) {
                    lvl++;
                    difficultyChange = 0;
                    turretsActive = false;
                }
            }

            if (timeToSpawn <= 0) {
                Spawn();
            }

            if(turretsActive) {
                timeToSpawnTurret -= ms;
                if (timeToSpawnTurret <= 0) {
                    SpawnTurret(turretsNumber);
                    timeToSpawnTurret = turretSpawnInterval;
                }
            }
        }

        void Spawn() {
            switch(lvl) {
                case Level.One:
                    LevelOne();
                    break;
                case Level.Two:
                    LevelTwo();
                    break;
                case Level.Three:
                    LevelThree();
                    break;
                case Level.Four:
                    LevelFour();
                    break;
            }
        }

        void LevelOne() {
            // Spawn two columns of basic enemies every three seconds.
            timeToSpawn = 3000;
            SpawnBasicColumns(2, 5);
        }

        void LevelTwo() {
            // Randomize whether to spawn basic or bug enemies. Spawn every three seconds.
            timeToSpawn = 3000;
            if (Exodos.Instance.random.Next(0, 3) == 2) SpawnBugColumns(2, 3);
            else SpawnBasicColumns(3, 5);
        }

        void LevelThree() {
            // Start spawning turrets every eight seconds.
            timeToSpawn = 2000;
            if (Exodos.Instance.random.Next(0, 3) == 2) SpawnBugColumns(2, 3);
            else SpawnBasicColumns(3, 5);
            if(!turretsActive) {
                turretsActive = true;
                turretSpawnInterval = 10000;
                timeToSpawnTurret = turretSpawnInterval;
                turretsNumber = 1;
            }
        }

        void LevelFour() {
            // Spawn two turrets at a time.
            timeToSpawn = 2000;
            if (Exodos.Instance.random.Next(0, 3) == 2) SpawnBugColumns(3, 5);
            else SpawnBasicColumns(3, 6);
            if(!turretsActive) {
                turretSpawnInterval = 10000;
                timeToSpawnTurret = turretSpawnInterval;
                turretsNumber = 2;
            }
        }

        // Spawns columns of perColumn basic enemies.
        void SpawnBasicColumns(int columns, int perColumn) {
            const int xDistance = 64;
            Random r = Exodos.Instance.random;
            for(int i = 0; i < columns; i++) {
                // Where to - vertically - spawn the column.
                int y = r.Next(0, Exodos.Instance.ScreenHeight - 128);
                for(int j = 0; j < perColumn; j++) {
                    BasicEnemy b = new BasicEnemy(Exodos.Instance.ScreenWidth + 100 + j * xDistance, y);
                    scene.enemies.Add(b);
                }
            }
        }

        void SpawnBugColumns(int columns, int perColumn) {
            const int xDistance = 64;
            Random r = Exodos.Instance.random;
            for (int i = 0; i < columns; i++) {
                int y = r.Next(0, Exodos.Instance.ScreenHeight - 128);
                for(int j = 0; j < perColumn; j++) {
                    BugEnemy b = new BugEnemy(Exodos.Instance.ScreenWidth + 100 + j * xDistance, y);
                    scene.enemies.Add(b);
                }
            }
        }

        void SpawnTurret(int count) {
            Random r = Exodos.Instance.random;
            
            for(int i = 0; i < count; i++) {
                int xDestination = r.Next(Exodos.Instance.ScreenHalfWidth, Exodos.Instance.ScreenWidth - 128);
                int yDestination = r.Next(128, Exodos.Instance.ScreenHeight - Exodos.Instance.ScreenHalfHeight / 2);
                int xStart = r.Next(Exodos.Instance.ScreenHalfWidth + Exodos.Instance.ScreenHalfWidth / 2, Exodos.Instance.ScreenWidth + Exodos.Instance.ScreenHalfWidth);
                int yStart = r.Next(-500, Exodos.Instance.ScreenHeight + 500);
                int sceneTime = r.Next(6000, 12000);
                TurretEnemy te = new TurretEnemy(xStart, yStart, xDestination, yDestination, sceneTime, scene);
                scene.enemies.Add(te);
            }
        }
    }
}