using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaptainOctagon
{
    class Projectile
    {
        public Texture2D Texture;
        public Vector2 Position;
        Viewport viewport;
        float projectileMoveSpeed;        
        public bool isActive;

        public int Width => Texture.Width;
        public int Height => Texture.Height;

        public void Initialize(Viewport viewport, Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;
            this.viewport = viewport;

            isActive = true;

            projectileMoveSpeed = 20f;
        }

        public void Update()
        {
            // Projectiles always move to the right
            Position.X += projectileMoveSpeed;

            // Deactivate the bullet if it goes out of screen
            if (Position.X + Texture.Width / 2 > viewport.Width)
                isActive = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0f, new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
        }
    }
}
