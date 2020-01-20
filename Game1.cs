using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

namespace SpaceInvaders
{
    public class Game1 : Game
    {
        private SpriteBatch spriteBatch;
        private Texture2D ship;
        private Texture2D playerBullet;
        private Texture2D enemy;
        private Texture2D enemyLaserTexture;
        private SoundEffect explosion;
        private SpriteFont font;
        private Player player;
        private List<PlayerBullet> bullets;
        private List<Enemy> enemies;
        private List<Laser> lasers;
        private TimeSpan bulletSpawnTime;
        private TimeSpan previousBulletSpawnTime;
        private TimeSpan laserSpawnTime;
        private TimeSpan previousLaserSpawnTime;
        private bool gameOver = false;
        private bool win = false;
        private int score = 0;
        private const int ScreenWidth = 600;
        private const int ScreenHeight = 800;
        private double enemySpeedModifier = 1;
        
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
            lasers = new List<Laser>();
            bulletSpawnTime = TimeSpan.FromSeconds((0.5f));
            laserSpawnTime = TimeSpan.FromSeconds((0.5f));
            player = new Player {Position = new Vector2(0, ScreenHeight - 55)};  
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ship = Content.Load<Texture2D>("Ship");
            player.Texture = ship;
            explosion = Content.Load<SoundEffect>("explosion");
            playerBullet = Content.Load<Texture2D>("PlayerBullet");
            enemy = Content.Load<Texture2D>("Enemy");
            enemyLaserTexture = Content.Load <Texture2D>("Laser"); 
            font = Content.Load<SpriteFont>("Score");
            //InitEnemies has to go here because of texture loading. 
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
            if (gameOver == true)
                        {
                            GraphicsDevice.Clear(Color.Black);
                            spriteBatch.DrawString(font
                            , "GAME OVER"
                            , new Vector2(ScreenWidth / 2
                                , ScreenHeight / 2)
                            , Color.White); 
                        }
            
                        if (win == true)
                        {
                            GraphicsDevice.Clear(Color.Black);
                            spriteBatch.DrawString(font
                            , "YOU ARE WINNER"
                            , new Vector2(ScreenWidth / 2
                                , ScreenHeight/ 2)
                            , Color.White);
                        }
            foreach (var b in bullets)
            {
                b.Draw(spriteBatch);
            }
            foreach (var e in enemies)
            {
                e.Draw(spriteBatch);
            }
            foreach (var l in lasers)
            {
                l.Draw(spriteBatch);
            }
            //Draw the player's Ship.
            spriteBatch.Draw(ship, player.Position, Color.White);
            //Draw Score.
            spriteBatch.DrawString(font
                ,$"Score: {score.ToString()}"
                ,new Vector2(ScreenWidth - 200, 10)
                ,Color.White );
            spriteBatch.DrawString(font
                , $"Lives : {player.Lives.ToString()}"
                , new Vector2(0, 10)
                , Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        
        private void EventHandler(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape)) Exit();
            
            if (state.IsKeyDown(Keys.Right)
                && player.Position.X <= ScreenWidth - player.Width) 
            {
                player.Position.X += player.Speed;
            }

            if (state.IsKeyDown(Keys.Left)
                && player.Position.X >= 0) 
            {
                player.Position.X -= player.Speed;
            }

            if (state.IsKeyDown(Keys.Space))
            {
                CreateBullet(gameTime);
            }
            //Bullet/ enemy collision.
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
                        explosion.Play();
                        score++;
                        enemySpeedModifier += 0.25;
                    }
                }
            }
            //Check laser collision.
            foreach (var l in lasers)
            {
                if (!Collision.RectangleCollision(l.Position.X
                    , l.Position.Y
                    , l.Width
                    , l.Height
                    , player.Position.X
                    , player.Position.Y
                    , player.Width
                    , player.Height)) continue;
                explosion.Play();
                l.Active = false;
                player.Lives--;
            }
            
            UpdateBullets(gameTime);            
            UpdateEnemies(gameTime);
            UpdateLasers(gameTime);
            if (enemies.Count <= 0)
            {
                win = true;
            }
            if (player.Lives < 0)
            {
                gameOver = true;
            }
        }

        private void UpdateLasers(GameTime gameTime)
        {
            for (var i = 0; i < lasers.Count; i++)
            {
                lasers[i].Update(gameTime);
                if (!lasers[i].Active || Math.Abs(lasers[i].Position.Y) > ScreenHeight)
                {
                    lasers.Remove(lasers[i]);
                }
            }
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
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (var en in enemies)
            {
                if (en.Position.X + en.Texture.Width >= ScreenWidth)
                {
                    foreach (var item in enemies)
                    {
                        item.Speed = -enemySpeedModifier;
                        item.Position.Y += (float)enemySpeedModifier;
                    }
                }
                if (en.Position.X <= 0)
                {
                    foreach (var item in enemies)
                    {
                        item.Speed = enemySpeedModifier;
                        item.Position.Y += (float)enemySpeedModifier;
                    }
                }
                //Game over if they reach the bottom.
                foreach (var item in enemies)
                {
                    if ((int) en.Position.Y + en.Height >= ScreenHeight)
                    {
                        gameOver = true;
                    }
                }

                en.Position.X += (float)en.Speed;
                // Create a laser at the bottom of the enemy and the middle.
                var minX = enemies.Min(x => x.Position.X);
                var maxX = enemies.Max(x => x.Position.X);
                var maxY = enemies.Max(x => x.Position.Y);
                var random = new Random();
                CreateLaser(gameTime
                    , new Vector2(random.Next((int)minX,(int)maxX) + (en.Width / 2)
                    , maxY + en.Height)); 
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
                bullet.Initialize(playerBullet, new Vector2(
                    player.Position.X + player.Width / 2
                    , player.Position.Y - player.Height/4));
                bullets.Add(bullet);
            }
        }

        private void CreateLaser(GameTime gameTime,Vector2 pos)
        {
            if (gameTime.TotalGameTime - previousLaserSpawnTime > laserSpawnTime)
            {
                previousLaserSpawnTime = gameTime.TotalGameTime;
                var laser = new Laser();
                laser.Initialize(enemyLaserTexture
                    , pos);
                lasers.Add(laser);
            }
        }
        
        private void InitEnemies()
        {
            for (var i = 50; i < ScreenWidth / 1.5; i += 50)
            {
                for (var j = 50; j < ScreenHeight/2; j += 50)
                {
                    var en = new Enemy();
                    en.Initialize(enemy, new Vector2(i, j));
                    enemies.Add(en);
                }
            }
        }
    }
}