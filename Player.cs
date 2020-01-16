using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceInvaders
{
    public class Player
    {
        private Texture2D _texture;
        public Vector2 Position;
        public float Speed = 5;
        private bool _active;

        public int Width => _texture.Width;

        public int Height => _texture.Height;

        public void Initialize(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
            _active = true;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Color.White);
        }
    }
}
