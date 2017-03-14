using System;

namespace ExodosGame {
    class ScreenShake {
        int duration;
        int frequency;
        float[] samples;
        int elapsed;
        int amplitude;

        public ScreenShake(int dur, int freq, int amp, Random random) {
            duration = dur;
            frequency = freq;
            amplitude = amp;

            int sampleCount = (int)((duration / 1000f) * frequency);


            samples = new float[sampleCount];
            for(int i = 0; i < sampleCount; i++) {
                samples[i] = ((float)random.NextDouble() * 2f - 1f);
            }

            elapsed = 0;
        }

        public void Update(int ms) {
            elapsed += ms;
            if(elapsed >= duration) {
                return;
            }
        }

        float Noise(int sample) {
            if(sample >= samples.Length) {
                return 0f;
            }
            return samples[sample];
        }

        // Returns the current amplitude of the shake curve.
        // Apply this to the camera's position.
        public float GetAmplitude() {
            float s = (float)elapsed / 1000f * frequency;
            int s0 = (int)Math.Floor(s);
            int s1 = s0 + 1;

            return (Noise(s0) + (s - s0) * (Noise(s1) - Noise(s0))) * Decay() * amplitude;
        }

        float Decay() {
            return (float)(duration - elapsed) / (float)duration;
        }
    }
}
