using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace CaptainOctagon
{
    public class SpriteManager : DrawableGameComponent
    {
        private SpriteBatch SpriteBatch;
        private UserControlledSprite player;
        private Random randomNumber { get; set; }

        private int livesRemaining { get; set; }

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
        Vector2 gemPosition;
        bool setGemBlue;
        bool setGemRed;
        bool collisionWithPlayer;

        //SOUNDS
        SoundEffect explosionSound;
        SoundEffectInstance explosionInstance;
        SoundEffect diamondCollected;
        SoundEffectInstance diamondCollectedInstance;


        public SpriteManager(Game game) : base(game)
        {
            randomNumber = new Random();
            
        }

        public override void Initialize()
        {
            ResetSpawnTime();
            ResetGemTime();
            ResetRedGemTime();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
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
                        --NumberLivesRemaining;
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
                        ++((Start)Game).CurrentScore;
                    }
                    //else take away a life
                    else
                    {
                        --NumberLivesRemaining;
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
                SpawnEnemy();
                //Spawn red gem
                SpawnRed();
                // Reset spawn timer
                ResetSpawnTime();
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
            SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            //Draw the player
            player.Draw(gameTime, SpriteBatch);
            player.DrawingColor = Color.White;

            //Draw all sprites
            //Swap between drawing colour of red and blue
            foreach (Sprite s in redList)
            {
                s.Draw(gameTime, SpriteBatch);
                s.DrawingColor = Color.LightBlue;
                setGemBlue = true;
                redGemSpawnTime -= gameTime.ElapsedGameTime.Milliseconds;

                if (redGemSpawnTime < 0)
                {
                    s.Draw(gameTime, SpriteBatch);
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
                s.Draw(gameTime, SpriteBatch);
                s.DrawingColor = Color.LightBlue;
                setGemBlue = true;
            }

            Vector2 gemPosition1 = getSpritePosition();

            //Draw explosion when collide With Player
            if (collisionWithPlayer == true)
            {
                //Draw explosion
                SpriteBatch.Draw(explosion, (gemPosition1), new Rectangle(currentFrame.X * frameSize.X, currentFrame.Y * frameSize.Y, frameSize.X, frameSize.Y), Color.White, 0, new Vector2(50, 50), 1, SpriteEffects.None, 0);
            }

            SpriteBatch.End();
            base.Draw(gameTime);
        }

        //Get Posistion for explosion animation
        public Vector2 getSpritePosition() => player.GetPosition;

        //get width for projectile calclations
        public int getSpriteWidth() => 2;

        //Reset Gem spawn Time
        private void ResetSpawnTime() => nextSpawnTime = randomNumber.Next(enemySpawnMinMilliseconds, enemySpawnMaxMilliseconds);

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
        private void SpawnEnemy()
        {
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;
            Point frameSize = new Point(75, 75);

            //Choose randomly - switch 1 or 2
            switch (randomNumber.Next(2))
            {
                case 0:
                    //Random posistion on right hand side of screen
                    position = new Vector2(Game.GraphicsDevice.PresentationParameters.BackBufferWidth, (randomNumber.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferHeight - frameSize.Y)));

                    //Random Speed
                    speed = new Vector2(-randomNumber.Next(enemyMinSpeed, enemyMaxSpeed), (randomNumber.Next(enemyMinSpeed, enemyMaxSpeed)));
                    break;

                //Random Posistion On top of screen
                case 1:
                    position = new Vector2(randomNumber.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferWidth - frameSize.X), -10);

                    //Random Speed
                    speed = new Vector2(-randomNumber.Next(enemyMinSpeed, enemyMaxSpeed), randomNumber.Next(enemyMinSpeed, enemyMaxSpeed));
                    break;
            }

            spriteList.Add(new GemSprite(Game.Content.Load<Texture2D>(@"Textures\Gem"), drawingColor, position, new Point(32, 32), 6, new Point(0, 0), new Point(0, 0), speed, 0));
            gemPosition = position;
        }

        private void SpawnRed()
        {
            Vector2 speed = Vector2.Zero;
            Vector2 position = Vector2.Zero;
            Point frameSize = new Point(75, 75);

            switch (randomNumber.Next(2))
            {
                case 0:
                    position = new Vector2(randomNumber.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferWidth - frameSize.X), -10);
                    speed = new Vector2(-randomNumber.Next(enemyMinSpeed, enemyMaxSpeed), randomNumber.Next(enemyMinSpeed, enemyMaxSpeed));
                    break;
                case 1:
                    position = new Vector2(Game.GraphicsDevice.PresentationParameters.BackBufferWidth, randomNumber.Next(0, Game.GraphicsDevice.PresentationParameters.BackBufferHeight - frameSize.Y));
                    speed = new Vector2(-randomNumber.Next(enemyMinSpeed, enemyMaxSpeed), randomNumber.Next(enemyMinSpeed, enemyMaxSpeed));
                    break;
            }

            redList.Add(new GemSprite(Game.Content.Load<Texture2D>(@"Textures\Gem"), drawingColor, position, new Point(32, 32), 6, new Point(0, 0), new Point(0, 0), speed, 0));

            gemPosition = position;
        }

        private int NumberLivesRemaining
        {
            get => livesRemaining;
            set
            {
                livesRemaining = value;
                if (livesRemaining == 0)
                {
                    // TODO Set GameOver
                }
            }
        }
    }
}