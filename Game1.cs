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
        private SpriteFont _font;
        private Player _player;
        private List<PlayerBullet> _bullets;
        private List<Enemy> _enemies;
        private int _score = 0;
        private const int ScreenWidth = 600;
        private const int ScreenHeight = 800;

        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = ScreenWidth, PreferredBackBufferHeight = ScreenHeight
            };
            graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _bullets = new List<PlayerBullet>();
            _enemies = new List<Enemy>();
            //TODO:ScreenHeight - (should be _player.Height but doesn't work).
            _player = new Player {Position = new Vector2(0, ScreenHeight - 96)};
            //InitEnemies should go here but it doesn't work properly right now. 
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _ship = Content.Load<Texture2D>("Ship");
            _playerBullet = Content.Load<Texture2D>("PlayerBullet");
            _enemy = Content.Load<Texture2D>("Enemy");
            _font = Content.Load<SpriteFont>("Score");
            //InitEnemies has to go here because of texture loading. 
            //TODO: Find a way to move this to init.
            InitEnemies();
        }

        protected override void Update(GameTime gameTime)
        {
            EventHandler(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();
            //Draw all the bullets.
            foreach (var b in _bullets)
            {
                b.Draw(_spriteBatch);
            }
            //Draw all the enemies.
            foreach (var e in _enemies)
            {
                e.Draw(_spriteBatch);
            }
            //Draw the player's Ship.
            _spriteBatch.Draw(_ship, _player.Position, Color.White);
            _spriteBatch.DrawString(_font,$"Score: {_score}",new Vector2(ScreenWidth - 200, 10),Color.White );     
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        
        //TODO: Event handler class.
        private void EventHandler(GameTime gameTime)
        {
            var rectangle = new Collision();
            var state = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            
            if (state.IsKeyDown(Keys.Right)
                //TODO: Find out why the return width methods won't work. Texture must be added to player at the wrong time.
                && _player.Position.X <= ScreenWidth + 96) 
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

            foreach (var bl in _bullets)
            {
                foreach (var en in _enemies)
                {
                    {
                        if (!Collision.RectangleCollision(bl.Position.X
                            , bl.Position.Y
                            , bl.Texture.Width
                            , bl.Texture.Height
                            , en.Position.X
                            , en.Position.Y
                            , en.Texture.Width
                            , en.Texture.Height)) continue;
                        en.Active = false;
                        bl.Active = false;
                        _score++;
                    }
                }
            }
            UpdateBullets(gameTime);            
            UpdateEnemies(gameTime);
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
            foreach (var enemy in _enemies)
            {
                if (enemy.Position.X >= ScreenWidth - 400)
                {
                    enemy.Speed = -1;
                }

                if (enemy.Position.X <= 0 + 400)
                {
                    enemy.Speed =  1;
                }

                enemy.Position.X += enemy.Speed;
                enemy.Position.Y += 1;
            }
            
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
            for (var i = 50; i < 450; i += 50)
            {
                for (var j = 50; j < 400; j += 50)
                {
                    var enemy = new Enemy();
                    enemy.Initialize(_enemy, new Vector2(i, j));
                    _enemies.Add(enemy);
                }
            }
        }
    }
}