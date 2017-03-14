using System;
using Microsoft.Xna.Framework;

namespace ExodosGame {
    // Represents a camera in the world.
    public class Camera2D {
        public Vector2 position;
        Vector2 direction;
        public float speed;

        // Screen shake related.
        bool shaking;
        int shakeElapsed;
        int shakeDuration;
        public Vector2 preShakePosition;
        ScreenShake xShake;
        ScreenShake yShake;

        public Camera2D(int xPosition, int yPosition, int xDirection, int yDirection, float speed) {
            position = new Vector2(xPosition, yPosition);
            direction = new Vector2(xDirection, yDirection);
            this.speed = speed;
        }

        public void Update(GameTime time) {
            int ms = time.ElapsedGameTime.Milliseconds;
            float deltaTime = (float)ms / 1000;

            if(!shaking) {
                // Normal camera move.
                position += direction * speed * deltaTime;
                return;
            }

            // The camera is shaking.
            shakeElapsed += ms;
            if(shakeElapsed >= shakeDuration) {
                shaking = false;
                position = preShakePosition;
                return;
            }

            xShake.Update(ms);
            yShake.Update(ms);
            Vector2 shakeMove = new Vector2(xShake.GetAmplitude(), yShake.GetAmplitude());
            position += shakeMove;
            position += direction * speed * deltaTime;
            preShakePosition += direction * speed * deltaTime;
        }

        public void Shake(int duration, int frequency, int amplitude) {

            if(!shaking) {
                preShakePosition = position;
            } else {
                position = preShakePosition;
            }
            Random random = new Random();
            xShake = new ScreenShake(duration, frequency, amplitude, random);
            yShake = new ScreenShake(duration, frequency, amplitude, random);
            shaking = true;
            shakeElapsed = 0;
            shakeDuration = duration;
        }
        
        // Use this as the SpriteBatch.Begin parameter.
        public Matrix GetViewMatrix() {
            return Matrix.CreateTranslation(new Vector3(-position, 0.0f));
        }
    }
}
