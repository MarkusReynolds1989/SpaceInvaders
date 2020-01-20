using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NUnit.Framework.Constraints;

namespace SpaceInvaders
{
    public class Laser
    {
        public Texture2D Texture;
        public Vector2 Position;
        public bool Active = true;
        public float Speed = 3;

        public int Width => Texture.Width;

        public int Height => Texture.Height;

        public void Initialize(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
        }

        public void Update(GameTime gameTime)
        {
            Position.Y += Speed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}