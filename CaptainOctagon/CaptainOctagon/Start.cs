using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using static CaptainOctagon.Enums;

namespace CaptainOctagon
{
    public partial class Start : Game
    {

        GraphicsDeviceManager GraphicsDeviceManager;
        SpriteBatch SpriteBatch;
        SpriteManager SpriteManager;
        GameState currentGameState = GameState.Start;

        SpriteFont scoreFont;

        SoundEffect loseTheme;
        SoundEffect winTheme;
        SoundEffectInstance loseThemeInstance;
        SoundEffectInstance winThemeInstance;
        Song battleTheme;

        Texture2D background;

        Texture2D projectileTexture;
        List<Projectile> projectiles;

        public Random rand { get; private set; }

        TimeSpan fireTime;
        TimeSpan previousFireTime;

        private ScrollingBackground myBackground;
        private ScrollingBackground myBackground2;

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
                    SpriteManager.Enabled = false;
                    SpriteManager.Visible = false;
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

        public Start()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            rand = new Random();
        }

        protected override void Initialize()
        {
            SpriteManager = new SpriteManager(this);
            Components.Add(SpriteManager);
            SpriteManager.Enabled = false;
            SpriteManager.Visible = false;
            projectiles = new List<Projectile>();
            // Set the laser to fire every quarter second
            fireTime = TimeSpan.FromSeconds(.15f);
            base.Initialize();
        }


        protected override void LoadContent()
        {
            //Create new SpriteBatch to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
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
                    SpriteManager.Enabled = false;
                    SpriteManager.Visible = false;
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
                SpriteManager.Enabled = false;
                SpriteManager.Visible = false;
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
                    SpriteBatch.Begin();

                    string text = "Harvest the red diamonds";
                    SpriteBatch.DrawString(scoreFont, text,
                        new Vector2(50, 300), Color.Red);

                    text = "But avoid the blue ones";
                    SpriteBatch.DrawString(scoreFont, text,
                        new Vector2(50, 350), Color.Blue);

                    text = "Press <Enter> or A button to start";
                    SpriteBatch.DrawString(scoreFont, text,
                        new Vector2(50, 400), Color.White);

                    SpriteBatch.End();

                    if (Keyboard.GetState().IsKeyDown(Keys.A))
                    {
                        currentGameState = GameState.Level1;
                        SpriteManager.Enabled = true;
                        SpriteManager.Visible = true;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentGameState = GameState.Level1;
                        SpriteManager.Enabled = true;
                        SpriteManager.Visible = true;
                    }
                    break;
                #endregion
                #region Level1
                case GameState.Level1:
                    GraphicsDevice.Clear(Color.Black);
                    SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    SpriteBatch.Draw(background, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height),
                        null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    SpriteBatch.DrawString(scoreFont, "Score: " + currentScore,
                        new Vector2(10, 10), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    SpriteBatch.DrawString(scoreFont, "Lives: " + livesRemaining,
                        new Vector2(10, 30), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    SpriteBatch.End();
                    break;
                #endregion
                #region Level2
                case GameState.Level2:
                    GraphicsDevice.Clear(Color.Black);
                    SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    myBackground.Draw(SpriteBatch);
                    SpriteBatch.DrawString(scoreFont, "Score: " + currentScore,
                        new Vector2(10, 10), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    SpriteBatch.DrawString(scoreFont, "Lives: " + livesRemaining,
                        new Vector2(10, 30), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    SpriteBatch.DrawString(scoreFont, "Time Remaining: " + timer,
                        new Vector2(200, 10), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    SpriteBatch.End();
                    break;
                #endregion
                #region Level2Menu
                case GameState.Level2Menu:
                    GraphicsDevice.Clear(Color.Black);
                    SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    string text2 = "See How many RED Diamonds you can collect in 60 Seconds!";
                    SpriteBatch.DrawString(scoreFont, text2,
                        new Vector2(50, 300), Color.Red);

                    text2 = "Press <Enter> to start";
                    SpriteBatch.DrawString(scoreFont, text2,
                        new Vector2(50, 350), Color.White);
                    SpriteBatch.End();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentGameState = GameState.Level2;
                        SpriteManager.Enabled = true;
                        SpriteManager.Visible = true;
                    }

                    break;
                #endregion
                #region Gameover
                case GameState.Gameover:
                    GraphicsDevice.Clear(Color.Black);
                    SpriteBatch.Begin();
                    text = "Sorry, You lose!";
                    SpriteBatch.DrawString(scoreFont, text,
                    new Vector2(50, 300), Color.Red);
                    text = "Press <Enter> to quit.";
                    SpriteBatch.DrawString(scoreFont, text,
                    new Vector2(50, 320), Color.Red);
                    SpriteBatch.End();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        Exit();
                    }
                    break;
                #endregion
                #region GameWin
                case GameState.GameWin:
                    GraphicsDevice.Clear(Color.Black);
                    SpriteBatch.Begin();
                    GraphicsDevice.Clear(Color.Black);
                    text = "You Win!";
                    SpriteBatch.DrawString(scoreFont, text,
                    new Vector2(50, 300), Color.Fuchsia);
                    text = "Press <Enter> to continue to Level 2!";
                    SpriteBatch.DrawString(scoreFont, text,
                    new Vector2(50, 320), Color.Fuchsia);
                    SpriteBatch.End();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentGameState = GameState.Level2Menu;
                    }
                    break;
                #endregion
                #region EndLevel2
                case GameState.EndLevel2:
                    GraphicsDevice.Clear(Color.Black);
                    SpriteBatch.Begin();
                    SpriteBatch.DrawString(scoreFont, "You Collected: " + currentScore,
                        new Vector2(25, 300), Color.White, 0, Vector2.Zero,
                        1, SpriteEffects.None, 1);
                    text = "Press <Enter> to quit.";
                    SpriteBatch.DrawString(scoreFont, text,
                    new Vector2(50, 320), Color.Red);
                    SpriteBatch.End();

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
