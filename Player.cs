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
        public Texture2D Texture;
        public Vector2 Position;
        public float Speed = 5;

        public int Width => Texture.Width;

        public int Height => Texture.Height;

        public void Initialize(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
