using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    class OptionsScene : Scene {
        string title = "Options";

        SpriteFont titleFont;
        SpriteFont optionFont;

        Camera2D camera;

        MenuOption toggleMusic;
        MenuOption toggleSounds;
        MenuOption musicVolume;
        MenuOption soundVolume;
        MenuOption screenShake;

        OnOffSwitch switchMusic;
        OnOffSwitch switchSound;

        ValueChanger changerMusic;
        ValueChanger changerSound;

        MenuCursor cursor;
        bool escPressed;

        public OptionsScene(Scene previousScene) {
            this.previousScene = previousScene;
            titleFont = Renderer.Instance.menu_title;
            optionFont = Renderer.Instance.menu_option;
            camera = new Camera2D(0, 0, 0, 0, 0);

            int screenHalfWidth = Exodos.Instance.ScreenHalfWidth;

            toggleMusic = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Music on/off").X / 2 - 80, 240, "Music on/off", optionFont);
            toggleSounds = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Sounds on/off").X / 2 - 80, 320, "Sounds on/off", optionFont);
            musicVolume = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Music volume").X / 2 - (int)optionFont.MeasureString("999").X / 2, 400, "Music volume", optionFont);
            soundVolume = new MenuOption((int)musicVolume.position.X, 480, "Sound volume", optionFont);
            screenShake = new MenuOption(screenHalfWidth - (int)optionFont.MeasureString("Screen shake power").X / 2, 560, "Screen shake power", optionFont);

            toggleMusic.SetTransitions(soundVolume, toggleSounds);
            toggleSounds.SetTransitions(toggleMusic, musicVolume);
            musicVolume.SetTransitions(toggleSounds, soundVolume);
            soundVolume.SetTransitions(musicVolume, toggleMusic);
            screenShake.SetTransitions(soundVolume, toggleMusic);

            cursor = new MenuCursor(toggleMusic);

            // Sound- and music-related.
            int musicVol = (int)(AudioSystem.Instance.musicVolume * 100);
            int soundVol = (int)(AudioSystem.Instance.soundVolume * 100);
            bool musicMuted = AudioSystem.Instance.musicMuted;
            bool soundMuted = AudioSystem.Instance.soundMuted;

            switchMusic = new OnOffSwitch(screenHalfWidth + 176, 230, !musicMuted, () => { AudioSystem.Instance.ToggleMusic(); });
            switchSound = new OnOffSwitch(screenHalfWidth + 176, 310, !soundMuted,
                () => {
                    AudioSystem.Instance.ToggleSound();
                    AudioSystem.Instance.PlaySound("ui_accept");
                }
                );

            changerMusic = new ValueChanger(screenHalfWidth + (int)optionFont.MeasureString("999").X + 40, 400, musicVol, 100, 0,
                () => {
                    AudioSystem.Instance.SetMusicVolume(changerMusic.Value / 100f);
                });
            changerSound = new ValueChanger((int)changerMusic.position.X, 480, soundVol, 100, 0,
                () => {
                    AudioSystem.Instance.SetSoundVolume(changerSound.Value / 100f);
                });
        }

        public override void Update(GameTime gameTime) {
            camera.Update(gameTime);

            cursor.Update(gameTime);

            // Take care of the input.
            switch(cursor.ProcessInput()) {
                case 0:
                    break;
                case 1:
                    AcceptButtonPress();
                    break;
                case 2:
                    DecrementButtonPress();
                    break;
                case 3:
                    IncrementButtonPress();
                    break;

            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) escPressed = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Escape)) {
                bool wasPressed = escPressed;
                escPressed = false;
                if (wasPressed) BackButtonPress();
            }
        }

        public override void Render(SpriteBatch spriteBatch) {
            cursor.Draw(spriteBatch);
            spriteBatch.DrawString(titleFont, title, new Vector2(
                Exodos.Instance.ScreenHalfWidth - titleFont.MeasureString(title).X / 2, 20), Color.White);

            toggleMusic.Draw(spriteBatch);
            toggleSounds.Draw(spriteBatch);
            musicVolume.Draw(spriteBatch);
            soundVolume.Draw(spriteBatch);

            switchMusic.Draw(spriteBatch);
            switchSound.Draw(spriteBatch);

            changerMusic.Draw(spriteBatch);
            changerSound.Draw(spriteBatch);

            spriteBatch.DrawString(optionFont, "Space - accept, Esc - back", new Vector2(
                Exodos.Instance.ScreenWidth - optionFont.MeasureString("Space - accept, Esc - back").X - 10, Exodos.Instance.ScreenHeight - optionFont.MeasureString("Space - accept, Esc - back").Y), Color.White);
        }

        void ToggleMusic() {
            switchMusic.Toggle();
        }

        void ToggleSounds() {
            switchSound.Toggle();
        }

        void MusicVolume() { }

        void SoundVolume() { }

        void ScreenShake() { }

        void AcceptButtonPress() {
            AudioSystem.Instance.PlaySound("ui_accept");
            if (cursor.currentOption == toggleMusic) ToggleMusic();
            else if (cursor.currentOption == toggleSounds) ToggleSounds();
            else if (cursor.currentOption == musicVolume) MusicVolume();
            else if (cursor.currentOption == soundVolume) SoundVolume();
            else if (cursor.currentOption == screenShake) ScreenShake();
        }

        void BackButtonPress() {
            AudioSystem.Instance.PlaySound("ui_back");
            Exodos.Instance.CurrentScene = previousScene;
        }

        void DecrementButtonPress() {
            if (cursor.currentOption == musicVolume) changerMusic.Decrement();
            else if (cursor.currentOption == soundVolume) changerSound.Decrement();
        }

        void IncrementButtonPress() {
            if (cursor.currentOption == musicVolume) changerMusic.Increment();
            else if (cursor.currentOption == soundVolume) changerSound.Increment();
        }

        public override Matrix GetCameraViewMatrix() {
            return camera.GetViewMatrix();
        }
    }
}
