using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SpaceInvaders
{
    public class Game1 : Game
    {
        private SpriteBatch spriteBatch;
        private Texture2D ship;
        private Texture2D playerBullet;
        private Texture2D enemy;
        private SpriteFont font;
        private Player player;
        private List<PlayerBullet> bullets;
        private List<Enemy> enemies;
        private TimeSpan bulletSpawnTime;
        private TimeSpan previousBulletSpawnTime;
        private bool gameOver = false;
        private int score = 0;
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
            bullets = new List<PlayerBullet>();
            enemies = new List<Enemy>();
            bulletSpawnTime = TimeSpan.FromSeconds((0.5f));
            //TODO:ScreenHeight - (should be _player.Height but doesn't work).
            player = new Player {Position = new Vector2(0, ScreenHeight - 96)};
            //InitEnemies should go here but it doesn't work properly right now. 
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ship = Content.Load<Texture2D>("Ship");
            playerBullet = Content.Load<Texture2D>("PlayerBullet");
            enemy = Content.Load<Texture2D>("Enemy");
            font = Content.Load<SpriteFont>("Score");
            //InitEnemies has to go here because of texture loading. 
            //TODO: Find a way to move this to init.
            InitEnemies();
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameOver == false)
            {
                EventHandler(gameTime);
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            //Draw all the bullets.
            foreach (var b in bullets)
            {
                b.Draw(spriteBatch);
            }
            //Draw all the enemies.
            foreach (var e in enemies)
            {
                e.Draw(spriteBatch);
            }
            //Draw the player's Ship.
            spriteBatch.Draw(ship, player.Position, Color.White);
            //Draw Score.
            spriteBatch.DrawString(font
                ,$"Score: {score.ToString()}"
                ,new Vector2(ScreenWidth - 200, 10)
                ,Color.White );     
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
        
        //TODO: Event handler class.
        private void EventHandler(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            
            if (state.IsKeyDown(Keys.Right)
                //TODO: Find out why the return width methods won't work. Texture must be added to player at the wrong time.
                && player.Position.X <= ScreenWidth + 96) 
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
                CreateBullet(gameTime);
            }

            foreach (var bl in bullets)
            {
                foreach (var en in enemies)
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
                        score++;
                    }
                }
            }
            UpdateBullets(gameTime);            
            UpdateEnemies(gameTime);
        }

        
        private void UpdateBullets(GameTime gameTime)
        {
            for (var i = 0; i < bullets.Count; i++)
            {
                bullets[i].Update(gameTime);
                if (!bullets[i].Active || Math.Abs(bullets[i].Position.Y) < 0)
                {
                    bullets.Remove(bullets[i]);
                }
            }
        }
        //TODO: Make the enemies move like space invaders.
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (var en in enemies)
            {
                if (en.Position.X >= ScreenWidth )
                {
                    en.Speed = -5;
                    en.Position.Y += 10;
                }

                if (en.Position.X <= -40)
                {
                    en.Speed =  5;
                    en.Position.Y += 10;
                }

                if ((int)en.Position.Y == ScreenHeight)
                {
                    gameOver = true;
                }
                
                en.Position.X += en.Speed;
            }
            
            for (var i = 0; i < enemies.Count; i++)
            {
                enemies[i].Update(gameTime);
                if (!enemies[i].Active)
                {
                    enemies.Remove(enemies[i]);
                }
            }
        }

        private void CreateBullet(GameTime gameTime)
        {
            if (gameTime.TotalGameTime - previousBulletSpawnTime > bulletSpawnTime)
            {
                previousBulletSpawnTime = gameTime.TotalGameTime;
                var bullet = new PlayerBullet();
                bullet.Initialize(playerBullet, new Vector2(player.Position.X + 45, player.Position.Y - 10));
                bullets.Add(bullet);
            }
        }

        private void InitEnemies()
        {
            for (var i = 50; i < 450; i += 50)
            {
                for (var j = 50; j < 400; j += 50)
                {
                    var en = new Enemy();
                    en.Initialize(this.enemy, new Vector2(i, j));
                    enemies.Add(en);
                }
            }
        }
    }
}