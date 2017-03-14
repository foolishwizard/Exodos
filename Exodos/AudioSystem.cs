using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace ExodosGame {
    class AudioSystem {
        // All of the sounds effects and their names.
        Dictionary<string, SoundEffect> soundEffects;
        // All of the music and song names.
        Dictionary<string, SoundEffect> musicTracks;
        // Currently playing music track.
        // We don't want to play more than one at a time.
        SoundEffectInstance currentTrack;
        string currentTrackName;

        // Volume.
        public bool soundMuted { get; private set; }
        public bool musicMuted { get; private set; }
        public float soundVolume { get; private set; }
        public float musicVolume { get; private set; }

        // For saving the settings.
        string settingsPath;

        // Catching exceptions or a lookup in the dictionary are too expensive.
        bool soundsLoaded;
        bool musicLoaded;

        private static AudioSystem _instance;
        public static AudioSystem Instance {
            get {
                if (_instance == null) {
                    _instance = new AudioSystem(100, 100);
                }
                return _instance;
            }
        }

        public AudioSystem(int musicVolume, int soundVolume) {
            _instance = this;
            soundEffects = new Dictionary<string, SoundEffect>();
            musicTracks = new Dictionary<string, SoundEffect>();
            this.musicVolume = musicVolume / 100f;
            this.soundVolume = soundVolume / 100f;
        }

        public void LoadContent(string soundEffectsPath, string musicPath, string settingsPath) {
            // Load the sound effects.
            if(File.Exists(soundEffectsPath)) {
                string[] effects = File.ReadAllLines(soundEffectsPath);
                foreach(string str in effects) {
                    string[] substrings = str.Split(' ');
                    if(substrings.Length != 2) {
                        // Invalid string! Let's try the next one.
                        continue;
                    }
                    // Let's load the sound!
                    SoundEffect sound = null;
                    try {
                        sound = Exodos.Instance.Content.Load<SoundEffect>(substrings[1]);
                    } catch(ContentLoadException e) {
                        Console.WriteLine(e);
                        continue;
                    }
                    soundEffects.Add(substrings[0], sound);
                }
                soundsLoaded = true;
            } else {
                Console.WriteLine("Couldn't find the sound effects file!");
                soundsLoaded = false;
            }

            // Load the music.
            if(File.Exists(musicPath)) {
                string[] tracks = File.ReadAllLines(musicPath);
                foreach(string str in tracks) {
                    string[] substrings = str.Split(' ');
                    if(substrings.Length != 2) {
                        continue;
                    }
                    SoundEffect track = null;
                    try {
                        track = Exodos.Instance.Content.Load<SoundEffect>(substrings[1]);
                    } catch(ContentLoadException e) {
                        Console.WriteLine(e);
                        continue;
                    }
                    musicTracks.Add(substrings[0], track);
                }
                musicLoaded = true;
            } else {
                Console.WriteLine("Couldn't find the music tracks file!");
                musicLoaded = false;
            }

            // Load the audio settings from an XML file.
            this.settingsPath = settingsPath;
            if (File.Exists(settingsPath)) {
                FileStream file = null;
                try { 
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    file = File.OpenRead(settingsPath);
                    //XmlReader reader = XmlReader.Create(file);
                    Settings settings = (Settings)serializer.Deserialize(file);
                    musicVolume = settings.musicVolume / 100f;
                    soundVolume = settings.soundVolume / 100f;
                    musicMuted = settings.musicMuted;
                    soundMuted = settings.soundMuted;
                    file.Close();
                } catch (Exception e) {
                    Console.WriteLine("Could not load audio settings from file: " + e);
                    if (file != null) file.Close();
                }
            }
        }

        public bool SaveSettings() {
            try {
                // FileMode.Create - if the file exists, truncate it to 0 bytes. If it doesn't, create new.
                // It's smart to read through all enumeration options.
                FileStream file = new FileStream(settingsPath, FileMode.Create, FileAccess.Write);
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                Settings settings = new Settings();
                settings.musicVolume = (int)(musicVolume * 100);
                settings.soundVolume = (int)(soundVolume * 100);
                settings.musicMuted = musicMuted;
                settings.soundMuted = soundMuted;
                serializer.Serialize(file, settings);
                file.Close();
                return true;
            } catch (Exception e) {
                Console.WriteLine("Could not save settings: " + e);
                return false;
            }
        }

        public void PlaySound(string soundName) {
            if (soundMuted) return;
            if (!soundsLoaded) return;
            SoundEffect sound = soundEffects[soundName];
            sound.Play(soundVolume, 0, 0);
        }

        public void PlaySoundWithPitch(string soundName, float pitch) {
            if (soundMuted) return;
            if (!soundsLoaded) return;
            SoundEffect sound = soundEffects[soundName];
            if (sound == null) {
                return;
            }
            sound.Play(1f, pitch, 0f);
        }

        public void Play3D(string soundName, Vector3 position) {
            if (soundMuted) return;
            if (!soundsLoaded) return;
            SoundEffect sound = soundEffects[soundName];
            if (sound == null) {
                return;
            }
            SoundEffectInstance instance = sound.CreateInstance();
            AudioEmitter emitter = new AudioEmitter();
            AudioListener listener = new AudioListener();
            emitter.Position = position;
            try {
                instance.Apply3D(listener, emitter);
            } catch (AccessViolationException e) {
                Console.WriteLine("3D positioning can only be applied to mono sounds! " + e);
            }
            instance.Play();

        }

        public void PlayMusicTrack(string trackName, bool looped) {
            if (musicMuted) return;
            if (!musicLoaded) return;
            if(currentTrackName == trackName) {
                currentTrack.Play();
                return;
            }

            if (!musicTracks.ContainsKey(trackName)) return;
            SoundEffect track = musicTracks[trackName];
            if (track == null) {
                return;
            }

            if (currentTrack != null) currentTrack.Stop(true);

            currentTrack = track.CreateInstance();
            currentTrack.IsLooped = looped;
            currentTrack.Play();
            currentTrackName = trackName;
        }

        public void PlayMusic(bool looped) {
            if (musicMuted) return;
            if(currentTrack != null) {
                currentTrack.IsLooped = looped;
                currentTrack.Resume();
            }
        }

        public void PauseMusic() {
            if(currentTrack != null) {
                currentTrack.Pause();
            }
        }

        public void ResumeMusic() {
            if(currentTrack != null) {
                if(currentTrack.State == SoundState.Paused) {
                    currentTrack.Resume();
                }
            }
        }

        public void StopMusic(bool immediately) {
            if(currentTrack != null) {
                currentTrack.Stop(immediately);
            }
        }

        public void MuteMusic() {
            musicMuted = true;
            if(currentTrack != null) {
                currentTrack.Volume = 0;
            }
        }

        public void UnmuteMusic() {
            musicMuted = false;
            if(currentTrack != null) {
                currentTrack.Volume = 1;
            }
        }

        public void ToggleMusic() {
            if (musicMuted) UnmuteMusic();
            else MuteMusic();
        }

        public void MuteSounds() {
            soundMuted = true;
        }

        public void UnmuteSounds() {
            soundMuted = false;
        }

        public void ToggleSound() {
            if (soundMuted) UnmuteSounds();
            else MuteSounds();
        }

        public void SetMusicVolume(float volume) {
            musicVolume = volume;
            if(currentTrack != null) {
                currentTrack.Volume = musicVolume;
            }
        }

        public void SetSoundVolume(float volume) {
            soundVolume = volume;
        }
    }
}