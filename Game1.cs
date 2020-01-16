using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SpaceInvaders
{
    public class Game1 : Game
    {
        private SpriteBatch _spriteBatch;
        private Texture2D _ship;
        private Texture2D _playerBullet;
        private Texture2D _enemy;
        private Player _player;
        private List<PlayerBullet> _bullets;
        private List<Enemy> _enemies;
        private const int ScreenWidth = 600;
        private const int ScreenHeight = 800;

        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _bullets = new List<PlayerBullet>();
            _enemies = new List<Enemy>();
            _player = new Player {Position = new Vector2(0, ScreenHeight - 96)};
            InitEnemies();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _ship = Content.Load<Texture2D>("Ship");
            _playerBullet = Content.Load<Texture2D>("PlayerBullet");
            _enemy = Content.Load<Texture2D>("Enemy");
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
            foreach (var b in _bullets)
            {
                b.Draw(_spriteBatch);
            }
            foreach (var e in _enemies)
            {
                e.Draw(_spriteBatch);
            }
            _spriteBatch.Draw(_ship, _player.Position, Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void EventHandler(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Right)
                && _player.Position.X <= ScreenWidth - _player.Width)
            {
                _player.Position.X += 2;
            }
            if (state.IsKeyDown(Keys.Left)
                && _player.Position.X >= 0)
            {
                _player.Position.X -= 2;
            }

            if (state.IsKeyDown(Keys.Space))
            {
                CreateBullet();
            }

            UpdateBullets(gameTime);
            UpdateEnemies(gameTime);
            foreach (var bl in _bullets)
            {
                foreach (var en in _enemies)
                {
                    {
                        if (!Collision(bl.Position.X
                            , bl.Position.Y
                            , bl.Texture.Width
                            , bl.Texture.Height
                            , en.Position.X
                            , en.Position.Y
                            , en.Texture.Width
                            , en.Texture.Height)) continue;
                        en.Active = false;
                        bl.Active = false;
                    }
                }
            }
        }

        private static bool Collision(float xFirstPos
            , float yFirstPos
            , float firstWidth
            , float firstHeight
            , float xSecondPos
            , float ySecondPos
            , float secondWidth
            , float secondHeight)
        {
            return xFirstPos < (xSecondPos + firstWidth)
                   && (xFirstPos + firstWidth) > xSecondPos 
                   && yFirstPos < (ySecondPos + secondWidth)
                   && yFirstPos + firstHeight > (ySecondPos);
        }

        private void UpdateBullets(GameTime gameTime)
        {
            for (var i = 0; i < _bullets.Count; i++)
            {
                _bullets[i].Update(gameTime);
                if (!_bullets[i].Active || Math.Abs(_bullets[i].Position.Y) < 0)
                {
                    _bullets.Remove(_bullets[i]);
                }
            }
        }
        private void UpdateEnemies(GameTime gameTime)
        {
            for (var i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Update(gameTime);
                if (!_enemies[i].Active)
                {
                    _enemies.Remove(_enemies[i]);
                }
            }
        }
        private void CreateBullet()
        {
            var bullet = new PlayerBullet();
            bullet.Initialize(_playerBullet, new Vector2(_player.Position.X + 45, _player.Position.Y - 10));
            _bullets.Add(bullet);
        }

        private void InitEnemies()
        {
            for (var i = 0; i < 250; i += 50)
            {
                for (var j = 0; j < 250; j += 50)
                {
                    var enemy = new Enemy();
                    enemy.Initialize(_enemy, new Vector2(i, j));
                    _enemies.Add(enemy);
                }
            }
        }
    }
}
