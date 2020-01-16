using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SpaceInvaders
    {
        public class Enemy
        {
            public Texture2D Texture;
            public Vector2 Position;
            public float Speed = 5;
            public bool Active;

            public int Width
            {
                get { return Texture.Width; }
            }
            public int Height
            {
                get { return Texture.Height; }
            }

            public void Initialize(Texture2D texture, Vector2 position)
            {
                Texture = texture;
                Position = position;
                Active = true;
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