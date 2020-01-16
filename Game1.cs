using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SpaceInvaders
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D Ship;
        private Texture2D PlayerBullet;
        private Vector2 Position;
        private List<PlayerBullet> bullets;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Position = new Vector2(0, 380);
            bullets = new List<PlayerBullet>();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Ship = this.Content.Load<Texture2D>("Ship");
            PlayerBullet = this.Content.Load<Texture2D>("PlayerBullet");
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
            foreach(var b in bullets)
            {
                b.Draw(_spriteBatch);
            }
            _spriteBatch.Draw(Ship, Position, Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private void EventHandler(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Right)
                && Position.X <= 700)
            {
                Position.X += 2;
            }
            if (state.IsKeyDown(Keys.Left)
                && Position.X >= 0)
            {
                Position.X -= 2;
            }

            if (state.IsKeyDown(Keys.Space))
            {
                FileBullet();
            }

            UpdateBullets(gameTime);
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

        private void FileBullet()
        {
            var bullet = new PlayerBullet();
            bullet.Initialize(PlayerBullet, new Vector2(Position.X + 45, Position.Y - 10));
            bullets.Add(bullet);
        }
    }
}
