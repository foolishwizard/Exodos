using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExodosGame {
    public class Exodos : Game {
        // A lot of classes want access to this one. Let's make it globally accessible.
        private static Exodos instance;
        public static Exodos Instance {
            get {
                return instance;
            }
        }

        GraphicsDeviceManager graphics;
        public Random random;
        Renderer renderer;
        AudioSystem audioSystem;

        public Scene CurrentScene;
        public Dictionary<string, Scene> ActiveScenes { get; private set; }

        // It's easiest to load it in here.
        public Highscore HScore;

        // For easy access.
        public int ScreenWidth { get { return GraphicsDevice.Viewport.Width; } }
        public int ScreenHeight { get { return GraphicsDevice.Viewport.Height; } }
        public int ScreenHalfWidth { get { return GraphicsDevice.Viewport.Width / 2; } }
        public int ScreenHalfHeight { get { return GraphicsDevice.Viewport.Height / 2; } }

        public Exodos() {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            Content.RootDirectory = "Content";

            random = new Random();

            instance = this;
        }

        protected override void Initialize() {
            Content.RootDirectory = "Content";
            renderer = new Renderer();
            renderer.Initialize();
            audioSystem = new AudioSystem(100, 100);

            ActiveScenes = new Dictionary<string, Scene>();

            base.Initialize();
        }

        protected override void LoadContent() {
            // TODO: use this.Content to load your game content here
            string soundEffectsPath = "C:\\Users\\Public\\sound_effects.txt";
            string musicPath = "C:\\Users\\Public\\music.txt";
            string settingsPath = "C:\\Users\\Public\\settings.txt";
            audioSystem.LoadContent(soundEffectsPath, musicPath, settingsPath);

            string highScorePath = "C:\\Users\\Public\\highScores.txt";

            if(File.Exists(highScorePath)) {
                FileStream file = null;
                try {
                    XmlSerializer serializer = new XmlSerializer(typeof(Highscore));
                    file = File.OpenRead(highScorePath);
                    HScore = (Highscore)serializer.Deserialize(file);
                    file.Close();
                } catch (Exception e) {
                    Console.WriteLine("Could not load high score file: " + e);
                    if (file != null) file.Close();
                    HScore = new Highscore();
                }
            } else {
                Console.WriteLine("High score file not found! Starting with a blank slate.");
                HScore = new Highscore();
            }

            CurrentScene = new MainMenuScene();
            ActiveScenes.Add("main_menu", CurrentScene);
        }

        protected override void UnloadContent() {
            AudioSystem.Instance.SaveSettings();

            // Save high score data to an XML file for easy deserialization.
            FileStream file = null;
            try {
                file = new FileStream("C:\\Users\\Public\\highScores.txt", FileMode.Create, FileAccess.Write);
                XmlSerializer serializer = new XmlSerializer(typeof(Highscore));
                serializer.Serialize(file, HScore);
                file.Close();
            } catch (Exception e) {
                Console.WriteLine("Could not save high score: " + e);
                if (file != null) file.Close();
            }
        }

        protected override void Update(GameTime gameTime) {
            float deltaTime = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            CurrentScene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            renderer.Render();

            base.Draw(gameTime);
        }

        public void ChangeScene(string sceneName, Scene caller, bool restart = false) {
            if(restart) {
                switch(sceneName) {
                    case "game":
                        CurrentScene = new GameScene();
                        ActiveScenes.Remove("game");
                        ActiveScenes.Add("game", CurrentScene);
                        break;
                    case "pause":
                        CurrentScene = new PauseScene();
                        ActiveScenes.Remove("pause");
                        ActiveScenes.Add("pause", CurrentScene);
                        break;
                    case "game_over":
                        CurrentScene = new GameOverScene();
                        ActiveScenes.Remove("game_over");
                        ActiveScenes.Add("game_over", CurrentScene);
                        break;
                    case "highscore":
                        CurrentScene = new HighscoreScene();
                        ActiveScenes.Remove("highscore");
                        ActiveScenes.Add("highscore", CurrentScene);
                        break;
                }
            }
            if(ActiveScenes.ContainsKey(sceneName)) {
                CurrentScene = ActiveScenes[sceneName];
            } else {
                switch(sceneName) {
                    case "options":
                        CurrentScene = new OptionsScene(caller);
                        ActiveScenes.Add("options", CurrentScene);
                        break;
                    case "game":
                        CurrentScene = new GameScene();
                        ActiveScenes.Add("game", CurrentScene);
                        break;
                    case "pause":
                        CurrentScene = new PauseScene();
                        ActiveScenes.Add("pause", CurrentScene);
                        break;
                    case "credits":
                        CurrentScene = new CreditsScene();
                        ActiveScenes.Add("credits", CurrentScene);
                        break;
                    case "highscore":
                        CurrentScene = new HighscoreScene();
                        ActiveScenes.Add("highscore", CurrentScene);
                        break;
                }
            }
            CurrentScene.SetPreviousScene(caller);
        }
    }
}
