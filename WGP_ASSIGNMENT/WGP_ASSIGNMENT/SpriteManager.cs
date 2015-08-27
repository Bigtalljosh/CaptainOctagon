using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WGP_ASSIGNMENT
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    /// 
    public class SpriteManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        UserControlledSprite player;

        List<Sprite> spriteList = new List<Sprite>();
        List<Sprite> redList = new List<Sprite>();

        //Random enemy spawn variables
        int enemySpawnMinMilliseconds = 0000;
        int enemySpawnMaxMilliseconds = 2000;
        int enemyMinSpeed = 2;
        int enemyMaxSpeed = 4;
        int nextSpawnTime = 0;
        int gemSpawnTime = 0;
        int redGemSpawnTime = 0;

        //Explosion 
        Texture2D explosion;
        Point frameSize = new Point(125, 125);
        Point currentFrame = new Point(2, 2);
        Point sheetSize = new Point(4, 3);
        int timeSinceLastFrame = 0;
        int millisecondsPerFrame = 100;
        
        //Gem
        private Color drawingColor;
        Vector2 gemPosition;
        bool setGemBlue;
        bool setGemRed;
        bool collisionWithPlayer;

        int changeGemColorTimer = 2000;

        //SOUNDS
        SoundEffect explosionSound;
        SoundEffectInstance explosionInstance;
        SoundEffect diamondCollected;
        SoundEffectInstance diamondCollectedInstance;


        public SpriteManager(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        public override void Initialize()
        {
            ResetSpawnTime();
            ResetGemTime();
            ResetRedGemTime();
            base.Initialize();
        }

        protected override void LoadContent( )
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            player = new UserControlledSprite(
            Game.Content.Load<Texture2D>(@"Textures/Playership"), Color.White,
            Vector2.Zero, new Point(32, 41), 6, new Point(0, 0),
            new Point(0, 0), new Vector2(10));
            explosion = Game.Content.Load<Texture2D>(@"Textures/Explosion");

        //SOUNDS
        explosionSound = Game.Content.Load<SoundEffect>(@"Audio\Explosion");
        explosionInstance = explosionSound.CreateInstance();
        explosionInstance.IsLooped = false;
        diamondCollected = Game.Content.Load<SoundEffect>(@"Audio\DiamondCollected");
        diamondCollectedInstance = diamondCollected.CreateInstance();
        diamondCollectedInstance.IsLooped = false;
        }

        public override void Update(GameTime gameTime)
        {
            // Update player
            player.Update(gameTime, Game.Window.ClientBounds);

            // Update all sprites in Spritelist
            for (int i = 0; i < spriteList.Count; ++i)
            {
                Sprite s = spriteList[i];
                s.Update(gameTime, Game.Window.ClientBounds);

                //if collision with player 
                //Remove sprite from game
                if (s.collisionRect.Intersects(player.collisionRect))
                {
                    //Play Collision Sound
                    explosionInstance.Volume = 1.00f;
                    explosionInstance.Play();
                    //Vibrate Gamepad
                    GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);

                    spriteList.RemoveAt(i);
                    --i;
                    //if gem is blue take away a life
                    if (setGemBlue == true)
                    {                            
                        --((Game1)Game).NumberLivesRemaining;
                        collisionWithPlayer = true;                          
                    }
                }
                GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);   
                //if out of Game Window
                //Remove sprite from game
                if (s.IsOutOfBounds(Game.Window.ClientBounds))
                {
                    spriteList.RemoveAt(i);
                    --i;
                }
            }

            // Update all sprites in Spritelist
            for (int i = 0; i < redList.Count; ++i)
            {
                Sprite r = redList[i];
                r.Update(gameTime, Game.Window.ClientBounds);
                //if collision with player 
                //Remove sprite from game
                if (r.collisionRect.Intersects(player.collisionRect))
                {
                    redList.RemoveAt(i);
                    --i;
                    //if gem is red add to score
                    if (setGemRed == true)
                    {
                        diamondCollectedInstance.Volume = 1.00f;
                        diamondCollectedInstance.Play();
                        ++((Game1)Game).CurrentScore;
                    }
                    //else take away a life
                    else 
                    {
                        --((Game1)Game).NumberLivesRemaining;
                        collisionWithPlayer = true;
                    }
                }

                //if out of Game Window
                //Remove sprite from game
                if (r.IsOutOfBounds(Game.Window.ClientBounds))
                {
                    redList.RemoveAt(i);
                    --i;
                }
            } 
  
            nextSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;
            if (nextSpawnTime < 0)
            {
                //Spawn blue gem
                SpawnEnemy( );
                //Spawn red gem
                SpawnRed();
                // Reset spawn timer
                ResetSpawnTime( );
            }

            //animation for explosion
            if (collisionWithPlayer == true)
            {
                timeSinceLastFrame += gameTime.ElapsedGameTime.Milliseconds;
                if (timeSinceLastFrame > millisecondsPerFrame)
                {
                    timeSinceLastFrame -= millisecondsPerFrame;
                    ++currentFrame.X;
                    if (currentFrame.X >= sheetSize.X)
                    {
                        currentFrame.X = -1;
                        ++currentFrame.Y;
                        if (currentFrame.Y >= sheetSize.Y)
                        {
                            currentFrame.X = -1;
                            currentFrame.Y = 1;
                            collisionWithPlayer = false;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            //Draw the player
            player.Draw(gameTime, spriteBatch);
            player.DrawingColor = Color.White;
            
            //Draw all sprites
            //Swap between drawing colour of red and blue
            foreach (Sprite s in redList)
            {
                s.Draw(gameTime, spriteBatch);
                s.DrawingColor = Color.LightBlue;
                setGemBlue = true;
                redGemSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (redGemSpawnTime < 0)
                {
                    s.Draw(gameTime, spriteBatch);
                    s.DrawingColor = Color.Red;
                    setGemRed = true;
                    gemSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;
                    if (gemSpawnTime < 0)
                    {
                        //s.Draw(gameTime, spriteBatch);
                        s.DrawingColor = Color.LightBlue;
                        setGemRed = false;
                        ResetRedGemTime();
                        ResetGemTime();
                    }
                }
            }

            foreach (Sprite s in spriteList)
            {
                s.Draw(gameTime, spriteBatch);
                s.DrawingColor = Color.LightBlue;
                setGemBlue = true;
            }
            Vector2 gemPosition1 = getSpritePosition();

            //Draw explosion when collide With Player
            if (collisionWithPlayer == true)
            {
                //Draw explosion
                spriteBatch.Draw(explosion, (gemPosition1),
                    new Rectangle(currentFrame.X * frameSize.X,
                    currentFrame.Y * frameSize.Y,
                    frameSize.X,
                    frameSize.Y),
                    Color.White, 0, new Vector2(50, 50),
                1, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }

        //Get Posistion for explosion animation
        public Vector2 getSpritePosition()
        {
            return player.GetPosition;
        }
        //get width for projectile calclations
        public int getSpriteWidth()
        {
            return 2;
        }

        //Reset Gem spawn Time
        private void ResetSpawnTime()
        {
            nextSpawnTime = ((Game1)Game).rand.Next(
            enemySpawnMinMilliseconds,
            enemySpawnMaxMilliseconds);
        }

        private void ResetRedGemTime()
        {
            redGemSpawnTime = (5000);
            ResetGemTime();
        }

        private void ResetGemTime()
        {
            gemSpawnTime = 10000;
        }

        //Spawn Gem
        private void SpawnEnemy( )
        {
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;
            Point frameSize = new Point(75, 75);

            //Choose randomly - switch 1 or 2
            switch (((Game1)Game).rand.Next(2))
            {
                case 0:
                //Random posistion on right hand side of screen
                position = new Vector2(
                Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                ((Game1)Game).rand.Next(0,Game.GraphicsDevice.PresentationParameters.BackBufferHeight - frameSize.Y));

                //Random Speed
                speed = new Vector2(
                    -((Game1)Game).rand.Next(enemyMinSpeed, enemyMaxSpeed)
                    , ((Game1)Game).rand.Next(enemyMinSpeed, enemyMaxSpeed));
                break;

                //Random Posistion On top of screen
                case 1:
                position = new Vector2(
                    ((Game1)Game).rand.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferWidth - frameSize.X),
                    -10);

                //Random Speed
                speed = new Vector2(
                    -((Game1)Game).rand.Next(enemyMinSpeed, enemyMaxSpeed)
                    , ((Game1)Game).rand.Next(enemyMinSpeed, enemyMaxSpeed));
                break;
            }

            spriteList.Add(
            new GemSprite(Game.Content.Load<Texture2D>(@"Textures\Gem"), drawingColor,
            position, new Point(32, 32), 6, new Point(0, 0),
            new Point(0, 0), speed, 0));
            gemPosition = position;
        }

       private void SpawnRed()
       {
           Vector2 speed = Vector2.Zero;
           Vector2 position = Vector2.Zero;
           Point frameSize = new Point(75, 75);

           switch (((Game1)Game).rand.Next(2))
           {
                 case 0:
                   position = new Vector2(
                       ((Game1)Game).rand.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferWidth - frameSize.X),
                       -10);

                   speed = new Vector2(
                       -((Game1)Game).rand.Next(enemyMinSpeed, enemyMaxSpeed)
                       , ((Game1)Game).rand.Next(enemyMinSpeed, enemyMaxSpeed));
                   break;
                 case 1:
                   position = new Vector2(
                   Game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                   ((Game1)Game).rand.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferHeight - frameSize.Y));

                   speed = new Vector2(
                       -((Game1)Game).rand.Next(enemyMinSpeed, enemyMaxSpeed)
                       , ((Game1)Game).rand.Next(enemyMinSpeed, enemyMaxSpeed));
                   break;
           }

           redList.Add(
            new GemSprite(Game.Content.Load<Texture2D>(@"Textures\Gem"), drawingColor,
                position, new Point(32, 32), 6, new Point(0, 0),
            new Point(0, 0), speed, 0));

           gemPosition = position;
       }
    }
}