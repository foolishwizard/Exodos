using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExodosGame {
    class MenuCursor {
        Vector2 leftPosition;
        Vector2 rightPosition;

        Sprite leftPart;
        Sprite rightPart;

        // For processing input.
        bool leftPressed;
        bool rightPressed;
        bool upPressed;
        bool downPressed;
        bool spacePressed;
        float leftTimer;
        float rightTimer;

        const float timerMax = 0.125f;

        public MenuOption currentOption;

        public MenuCursor(MenuOption option) {
            leftPart = new Sprite(Renderer.Instance.cursor, 0, 23, 32, 80, Color.Red);
            rightPart = new Sprite(Renderer.Instance.cursor, 32, 23, 32, 80, Color.Red);

            leftTimer = timerMax;
            rightTimer = timerMax;

            MoveTo(option);
        }

        public void Update(GameTime gameTime) {
            if (leftPressed) leftTimer -= gameTime.ElapsedGameTime.Milliseconds / 1000f;
            else if (rightPressed) rightTimer -= gameTime.ElapsedGameTime.Milliseconds / 1000f;
        }

        // Returns: 0 - nothing, 1 - space, 2 - left, 3 - right.
        public int ProcessInput() {
            int returnVal = 0;
            if (Keyboard.GetState().IsKeyDown(Keys.Down)) downPressed = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Up)) upPressed = true;
            if (Keyboard.GetState().IsKeyDown(Keys.Left)) {
                if (leftPressed && leftTimer <= 0) {
                    returnVal = 2;
                    leftTimer = timerMax;
                }
                leftPressed = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right)) {
                if (rightPressed && rightTimer <= 0) {
                    returnVal = 3;
                    rightTimer = timerMax;
                }
                rightPressed = true;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space)) spacePressed = true;
            if (Keyboard.GetState().IsKeyUp(Keys.Down)) {
                if (downPressed && currentOption.down != null) MoveTo(currentOption.down);
                downPressed = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Up)) {
                if (upPressed && currentOption.up != null) MoveTo(currentOption.up);
                upPressed = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Left)) {
                if (leftPressed) returnVal = 2;
                leftPressed = false;
            }
            if (Keyboard.GetState().IsKeyUp(Keys.Right)) {
                if (rightPressed) returnVal = 3;
                rightPressed = false;
            }
            if(Keyboard.GetState().IsKeyUp(Keys.Space)) {
                if (spacePressed) returnVal = 1;
                spacePressed = false;
            }
            return returnVal;
        }

        public void MoveTo(MenuOption option) {
            leftPosition = option.position;
            leftPosition.X -= 10;
            rightPosition = option.position;
            rightPosition.X += option.Size - 22;
            leftPosition.Y -= 10;
            rightPosition.Y -= 10;
            currentOption = option;

            AudioSystem.Instance.PlaySound("ui_move");
        }

        public void Draw(SpriteBatch spriteBatch) {
            leftPart.Draw(spriteBatch, leftPosition);
            rightPart.Draw(spriteBatch, rightPosition);
        }
    }
}