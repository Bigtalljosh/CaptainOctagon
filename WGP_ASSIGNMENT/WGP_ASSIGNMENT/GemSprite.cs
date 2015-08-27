using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace WGP_ASSIGNMENT
{
    class GemSprite: Sprite
    {
        public GemSprite(Texture2D textureImage, Color color, Vector2 position, Point frameSize,
        int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed
            )
            : base(textureImage, color, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed)
        {
        }

        public GemSprite(Texture2D textureImage, Color color, Vector2 position, Point frameSize,
        int collisionOffset, Point currentFrame, Point sheetSize, Vector2 speed,
        int millisecondsPerFrame)
            : base(textureImage, color, position, frameSize, collisionOffset, currentFrame,
            sheetSize, speed, millisecondsPerFrame)
        {
        }

        public override Vector2 direction
        {
            get { return speed; }
        }

        public override void Update(GameTime gameTime, Rectangle clientBounds)
        {
            position += direction;
            
            base.Update(gameTime, clientBounds);
        }
    }
}
