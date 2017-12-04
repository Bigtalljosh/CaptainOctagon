using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CaptainOctagon
{
    abstract class Sprite
    {
        Texture2D textureImage;
        protected Point frameSize;
        Point currentFrame;
        Point sheetSize;
        int collisionOffset;
        int timeSinceLastFrame = 0;
        int millisecondsPerFrame;
        Color drawingColor;

        const int defaultMilliSecondsPerFrame = 16;
        protected Vector2 speed;
        public Vector2 position;
        public int scoreValue { get; protected set; }


        public Sprite(Texture2D textureImage, Color color, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed)
            : this(textureImage, color, position, frameSize, collisionOffset, currentFrame, sheetSize, speed, defaultMilliSecondsPerFrame)
        {

        }

        public Sprite(Texture2D textureImage, Color color, Vector2 position, Point frameSize, int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed, int millisecondsPerFrame)
        {
            this.textureImage = textureImage;
            this.position = position;
            this.frameSize = frameSize;
            this.collisionOffset = collisionOffset;
            this.currentFrame = currentFrame;
            this.sheetSize = sheetSize;
            this.speed = speed;
            this.millisecondsPerFrame = millisecondsPerFrame;
        }
        
        public virtual void Update(GameTime gameTime, Rectangle clientBounds)
        {
            timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
            if (timeSinceLastFrame > millisecondsPerFrame)
            {
                timeSinceLastFrame = 0;
                ++currentFrame.X;
                if (currentFrame.X >= sheetSize.X)
                {
                    currentFrame.X = 0;
                    ++currentFrame.Y;
                    if (currentFrame.Y >= sheetSize.Y)
                    {
                        currentFrame.Y = 0;
                    }
                }
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) => spriteBatch.Draw(textureImage, position, new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), drawingColor, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

        public Color DrawingColor
        {
            get => drawingColor;
            set => drawingColor = value;
        }

        public abstract Vector2 direction
        {
            get;
        }

        public Vector2 GetPosition => position;

        public Vector2 SetPosition
        {
            set => position = Vector2.Zero;
        }

        public Rectangle collisionRect => new Rectangle((int)position.X + collisionOffset, (int)position.Y + collisionOffset, frameSize.X - (collisionOffset * 2), frameSize.Y - (collisionOffset * 2));

        public bool IsOutOfBounds(Rectangle clientRect)
        {
            if (position.X < -frameSize.X || position.X > clientRect.Width || position.Y < -frameSize.Y || position.Y > clientRect.Height)
            {
                return true;
            }

            return false;
        }
    }
}
