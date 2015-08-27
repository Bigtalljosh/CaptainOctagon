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
/// Josh Dadak (d005578a) Windows Game Programming Assignment -  Captain Octagon’s Diamond Adventure
namespace WGP_ASSIGNMENT
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Fields
        GraphicsDeviceManager graphics;
        //SPRITE MANAGER
        SpriteBatch spriteBatch;
        SpriteManager spriteManager;
        //Parallaxing Background

        //GAME STATES
        enum GameState { Start, Level1, Level2, Gameover, GameWin, Level2Menu, EndLevel2 };
        GameState currentGameState = GameState.Start;
        //FONTS
        //score
        SpriteFont scoreFont;
        //SOUNDS
        SoundEffect loseTheme;
        SoundEffect winTheme;
        SoundEffectInstance loseThemeInstance;
        SoundEffectInstance winThemeInstance;
        Song battleTheme;
        //TEXTURES
        //Background
        Texture2D background;
        // Projectiles
        Texture2D projectileTexture;
        List<Projectile> projectiles;
        //RANDOM NUMBER
        public Random rand { get; private set; }
        //Fire Rate
        TimeSpan fireTime;
        TimeSpan previousFireTime;

        #endregion

        #region Scores
        //LIVES
        int livesRemaining = 5;
        public int NumberLivesRemaining
        {
            get { return livesRemaining; }
            set
            {
                livesRemaining = value;
                if (livesRemaining == 0)
                {
                    currentGameState = GameState.Gameover;
                    spriteManager.Enabled = false;
                    spriteManager.Visible = false;
                }
            }
        }
        //SCORES
        int currentScore = 0;
        public int CurrentScore
        {
            get { return currentScore; }
            set
            {
                currentScore = value;
            }
        }
        //Level 2 Timer
        double timer = 60;

        #endregion
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            rand = new Random();
        }

        protected override void Initialize()
        {
            spriteManager = new SpriteManager(this);
            Components.Add(spriteManager);
            spriteManager.Enabled = false;
            spriteManager.Visible = false;
            projectiles = new List<Projectile>();
            // Set the laser to fire every quarter second
            fireTime = TimeSpan.FromSeconds(.15f);
            base.Initialize();
        }

        private ScrollingBackground myBackground, myBackground2;

        protected override void LoadContent()
        {
            //Create new SpriteBatch to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //Sounds Load
            battleTheme = Content.Load<Song>(@"Audio\BattleTheme");
            loseTheme = Content.Load<SoundEffect>(@"Audio\LoseTheme");
            winTheme = Content.Load<SoundEffect>(@"Audio\WinTheme");
            loseThemeInstance = loseTheme.CreateInstance();
            winThemeInstance = winTheme.CreateInstance();
            loseThemeInstance.IsLooped = false;
            winThemeInstance.IsLooped = false;
            //Font Load
            scoreFont = Content.Load<SpriteFont>(@"Fonts\score");
            //Texture Load
            background = Content.Load<Texture2D>(@"Textures\starfield");
            projectileTexture = Content.Load<Texture2D>(@"Textures\laser");
            // Load the parallaxing background
            myBackground = new ScrollingBackground();
            Texture2D scrollBackground = Content.Load<Texture2D>(@"Textures\starfield");
            myBackground.Load(GraphicsDevice, scrollBackground);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            switch (currentGameState)
            {
                case GameState.Start:
                    break;
                case GameState.Level1:
                    break;
                case GameState.Level2:
                    break;
                case GameState.Level2Menu:
                    break;
                case GameState.EndLevel2:
                    break;
                case GameState.Gameover:
                    break;
            }
            #region Level 1
            if (currentGameState == GameState.Level1)
            {
                if (MediaPlayer.State == MediaState.Stopped)
                {
                    MediaPlayer.Volume = 0.25f;
                    MediaPlayer.Play(battleTheme);
                }
                MediaPlayer.IsRepeating = true;

                if (currentScore == 5)
                {
                    currentGameState = GameState.GameWin;
                    spriteManager.Enabled = false;
                    spriteManager.Visible = false;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.N))
                {
                    currentGameState = GameState.Level2;
                }
            }
            #endregion
            #region Level 2
            else if (currentGameState == GameState.Level2)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (MediaPlayer.State == MediaState.Stopped)
                {
                    MediaPlayer.Play(battleTheme);
                }
                MediaPlayer.IsRepeating = true;

                timer -= gameTime.ElapsedGameTime.TotalSeconds;
                myBackground.Update(elapsed * 100);
                if (timer <= 0)
                {
                    currentGameState = GameState.EndLevel2;
                }
            }
            #endregion
            #region Gameover
            else if (currentGameState == GameState.Gameover)
            {
                MediaPlayer.Stop();
                loseThemeInstance.Volume = 0.80f;
                loseThemeInstance.Play();
            }
            #endregion
            #region GameWin
            else if (currentGameState == GameState.GameWin)
            {
                MediaPlayer.Stop();
                winThemeInstance.Volume = 1.00f;
                winThemeInstance.Play();
            }
            #endregion
            #region Level2Menu
            else if (currentGameState == GameState.Level2Menu)
            {
                currentScore = 0;
                livesRemaining = 5;
            }
            #endregion
            #region EndLevel2
            else if (currentGameState == GameState.EndLevel2)
            {
                spriteManager.Enabled = false;
                spriteManager.Visible = false;
            }
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            switch (currentGameState)
            {
                #region Start
                case GameState.Start:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin();

                    string text = "Harvest the red diamonds";
                    spriteBatch.DrawString(scoreFont, text,
                        new Vector2(50, 300), Color.Red);

                    text = "But avoid the blue ones";
                    spriteBatch.DrawString(scoreFont, text,
                        new Vector2(50, 350), Color.Blue);

                    text = "Press <Enter> or A button to start";
                    spriteBatch.DrawString(scoreFont, text,
                        new Vector2(50, 400), Color.White);

                    spriteBatch.End();

                    if (Keyboard.GetState().IsKeyDown(Keys.A))
                    {
                        currentGameState = GameState.Level1;
                        spriteManager.Enabled = true;
                        spriteManager.Visible = true;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentGameState = GameState.Level1;
                        spriteManager.Enabled = true;
                        spriteManager.Visible = true;
                    }
                    break;
                #endregion
                #region Level1
                case GameState.Level1:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    spriteBatch.Draw(background, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height),
                        null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    spriteBatch.DrawString(scoreFont, "Score: " + currentScore,
                        new Vector2(10, 10), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(scoreFont, "Lives: " + livesRemaining,
                        new Vector2(10, 30), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    spriteBatch.End();
                    break;
                #endregion
                #region Level2
                case GameState.Level2:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    myBackground.Draw(spriteBatch);
                    spriteBatch.DrawString(scoreFont, "Score: " + currentScore,
                        new Vector2(10, 10), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(scoreFont, "Lives: " + livesRemaining,
                        new Vector2(10, 30), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    spriteBatch.DrawString(scoreFont, "Time Remaining: " + timer,
                        new Vector2(200, 10), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    spriteBatch.End();
                    break;
                #endregion
                #region Level2Menu
                case GameState.Level2Menu:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    string text2 = "See How many RED Diamonds you can collect in 60 Seconds!";
                    spriteBatch.DrawString(scoreFont, text2,
                        new Vector2(50, 300), Color.Red);

                    text2 = "Press <Enter> to start";
                    spriteBatch.DrawString(scoreFont, text2,
                        new Vector2(50, 350), Color.White);
                    spriteBatch.End();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentGameState = GameState.Level2;
                        spriteManager.Enabled = true;
                        spriteManager.Visible = true;
                    }

                    break;
                #endregion
                #region Gameover
                case GameState.Gameover:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin();
                    text = "Sorry, You lose!";
                    spriteBatch.DrawString(scoreFont, text,
                    new Vector2(50, 300), Color.Red);
                    text = "Press <Enter> to quit.";
                    spriteBatch.DrawString(scoreFont, text,
                    new Vector2(50, 320), Color.Red);
                    spriteBatch.End();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        Exit();
                    }
                    break;
                #endregion
                #region GameWin
                case GameState.GameWin:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin();
                    GraphicsDevice.Clear(Color.Black);
                    text = "You Win!";
                    spriteBatch.DrawString(scoreFont, text,
                    new Vector2(50, 300), Color.Fuchsia);
                    text = "Press <Enter> to continue to Level 2!";
                    spriteBatch.DrawString(scoreFont, text,
                    new Vector2(50, 320), Color.Fuchsia);
                    spriteBatch.End();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentGameState = GameState.Level2Menu;
                    }
                    break;
                #endregion
                #region EndLevel2
                case GameState.EndLevel2:
                    GraphicsDevice.Clear(Color.Black);
                    spriteBatch.Begin();
                    spriteBatch.DrawString(scoreFont, "You Collected: " + currentScore,
                        new Vector2(25, 300), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    text = "Press <Enter> to quit.";
                    spriteBatch.DrawString(scoreFont, text,
                    new Vector2(50, 320), Color.Red);
                    spriteBatch.End();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        Exit();
                    }
                    break;
                #endregion
            }
            base.Draw(gameTime);
        }
    }
}
