using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SpaceInvaders
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D Ship;
        private Texture2D PlayerBullet;
        private Texture2D Enemy;
        private Player player;
        private List<PlayerBullet> bullets;
        private List<Enemy> enemies;
        const int SCREEN_WIDTH = 600;
        const int SCREEN_HEIGHT = 800;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            bullets = new List<PlayerBullet>();
            enemies = new List<Enemy>();
            player = new Player();
            player.Position = new Vector2(0, SCREEN_HEIGHT - 96);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Ship = Content.Load<Texture2D>("Ship");
            PlayerBullet = Content.Load<Texture2D>("PlayerBullet");
            Enemy = Content.Load<Texture2D>("Enemy");
            InitEnemies();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            EventHandler(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            foreach (var b in bullets)
            {
                b.Draw(_spriteBatch);
            }
            foreach (var e in enemies)
            {
                e.Draw(_spriteBatch);
            }
            _spriteBatch.Draw(Ship, player.Position, Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void EventHandler(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Right)
                && player.Position.X <= SCREEN_WIDTH - 96)
            {
                player.Position.X += 2;
            }
            if (state.IsKeyDown(Keys.Left)
                && player.Position.X >= 0)
            {
                player.Position.X -= 2;
            }

            if (state.IsKeyDown(Keys.Space))
            {
                CreateBullet();
            }

            UpdateBullets(gameTime);
            UpdateEnemies(gameTime);
            foreach (var bl in bullets)
            {
                foreach (var en in enemies)
                {
                    {
                        Collision(bl, en);
                    }
                }
            }
        }

        private static void Collision(PlayerBullet bl, Enemy en)
        {
            if ((bl.Position.X < (en.Position.X + bl.Texture.Width)
                && (bl.Position.X + bl.Texture.Width) > en.Position.X
                && bl.Position.Y < (en.Position.Y + en.Texture.Width)
                && bl.Position.Y + bl.Texture.Height > (en.Position.Y)))
            {
                en.Active = false;
                bl.Active = false;
            }
        }

        private void UpdateBullets(GameTime gameTime)
        {
            for (var i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(gameTime);
                if (!bullets[i].Active || bullets[i].Position.Y == 0)
                {
                    bullets.Remove(bullets[i]);
                }
            }
        }
        private void UpdateEnemies(GameTime gameTime)
        {
            for (var i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update(gameTime);
                if (!enemies[i].Active)
                {
                    enemies.Remove(enemies[i]);
                }
            }
        }
        private void CreateBullet()
        {
            var bullet = new PlayerBullet();
            bullet.Initialize(PlayerBullet, new Vector2(player.Position.X + 45, player.Position.Y - 10));
            bullets.Add(bullet);
        }

        private void InitEnemies()
        {
            for (var i = 0; i < 250; i += 50)
            {
                for (var j = 0; j < 250; j += 50)
                {
                    var enemy = new Enemy();
                    enemy.Initialize(Enemy, new Vector2(i, j));
                    enemies.Add(enemy);
                }
            }
        }
    }
}
