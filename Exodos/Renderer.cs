using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExodosGame {
    // Responsible for all the drawing and storing graphical assets.
    class Renderer {
        // It's globally available (and a singleton).
        private static Renderer _instance;
        public static Renderer Instance {
            get {
                if(_instance == null) {
                    _instance = new Renderer();
                    _instance.Initialize();
                }
                return _instance;
            }
        }

        // All textures present in the game.
        public Texture2D player;
        public Texture2D exhaust;
        public Texture2D enemy;
        public Texture2D explosion_large;
        public Texture2D explosion;
        public Texture2D bullets;
        public Texture2D blueHit;
        public Texture2D powerup_weapon;
        public Texture2D powerup_life;
        public Texture2D powerup_bomb;
        public Texture2D fill_bar;
        public Texture2D bar_fill;
        public Texture2D greenHit;
        public Texture2D bug;
        public Texture2D turret;

        public Texture2D explosion_player_1;
        public Texture2D explosion_player_2;
        public Texture2D explosion_player_3;
        public Texture2D explosion_player_4;
        public Texture2D implosion;

        // Menu-related textures.
        public Texture2D cursor;
        public Texture2D switch_on;
        public Texture2D switch_off;

        // Fonts used in the game.
        public SpriteFont titleFont;
        public SpriteFont regularFont;
        public SpriteFont menu_title;
        public SpriteFont menu_option;
        public SpriteFont menu_score;
        public SpriteFont ui;

        // SpriteBatch used for drawing.
        SpriteBatch spriteBatch;
        // SpriteBatch used for drawing the UI so it's not affected by the screen shake effect.
        SpriteBatch UIBatch;

        public void Initialize() {
            // Load all the content here.
            _instance = this;

            // Textures.
            player = LoadTexture("player.png");
            exhaust = LoadTexture("exhaust");
            enemy = LoadTexture("enemy.png");
            explosion_large = LoadTexture("explosion_large");
            explosion = LoadTexture("explosion");
            bullets = LoadTexture("bullets");
            blueHit = LoadTexture("bullet_hit_blue");
            powerup_weapon = LoadTexture("powerup_weapon");
            powerup_life = LoadTexture("powerup_life");
            powerup_bomb = LoadTexture("powerup_bomb");
            fill_bar = LoadTexture("fill_bar");
            bar_fill = LoadTexture("fill_bar_fill");
            greenHit = LoadTexture("bullet_hit_green");
            bug = LoadTexture("Bug");
            turret = LoadTexture("stationary");

            explosion_player_1 = LoadTexture("explosion_player1");
            explosion_player_2 = LoadTexture("explosion_player2");
            explosion_player_3 = LoadTexture("explosion_player_3");
            explosion_player_4 = LoadTexture("explosion_player_4");
            implosion = LoadTexture("implosion");

            // Menu-related.
            cursor = LoadTexture("cursor");
            switch_on = LoadTexture("on_off_switch_on");
            switch_off = LoadTexture("on_off_switch_off");

            // Fonts.
            regularFont = LoadFont("regularFont");
            titleFont = LoadFont("titleFont");
            menu_title = LoadFont("title");
            menu_option = LoadFont("menu_option");
            menu_score = LoadFont("high_score");
            ui = LoadFont("ui");

            spriteBatch = new SpriteBatch(Exodos.Instance.GraphicsDevice);
            UIBatch = new SpriteBatch(Exodos.Instance.GraphicsDevice);
        }

        public Texture2D LoadTexture(string path) {
            return Exodos.Instance.Content.Load<Texture2D>("Graphics\\" + path);
        }

        public SpriteFont LoadFont(string path) {
            return Exodos.Instance.Content.Load<SpriteFont>("Fonts\\" + path);
        }

        public void Render() {
            Exodos.Instance.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(transformMatrix: Exodos.Instance.CurrentScene.GetCameraViewMatrix());

            Scene s = Exodos.Instance.CurrentScene;
            s.Render(spriteBatch);

            spriteBatch.End();

            UIBatch.Begin();

            s.RenderUI(UIBatch);

            UIBatch.End();
        }
    }
}
