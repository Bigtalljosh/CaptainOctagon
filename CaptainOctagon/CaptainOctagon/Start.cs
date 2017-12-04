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

        private GraphicsDeviceManager GraphicsDeviceManager;
        private SpriteBatch SpriteBatch;
        private SpriteManager SpriteManager;
        private Random randomNumber { get; set; }
        
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
        TimeSpan fireTime;
        private ScrollingBackground myBackground;
        int livesRemaining = 5;



        int currentScore = 0;
        public int CurrentScore
        {
            get => currentScore;
            set => currentScore = value;
        }

        double timer = 60;

        public Start()
        {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            randomNumber = new Random();
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
                    if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentGameState = GameState.Level1;
                        SpriteManager.Enabled = true;
                        SpriteManager.Visible = true;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        Exit();
                    }

                    break;
                case GameState.Level1:

                    MediaPlayer.Volume = 0.25f;
                    MediaPlayer.Play(battleTheme);
                    MediaPlayer.IsRepeating = true;

                    if (currentScore == 5)
                    {
                        currentGameState = GameState.GameWin;
                        MediaPlayer.Stop();
                        SpriteManager.Enabled = false;
                        SpriteManager.Visible = false;
                    }

                    // Shortcut to level 2
                    if (Keyboard.GetState().IsKeyDown(Keys.N))
                        currentGameState = GameState.Level2;

                    break;
                case GameState.Level2:
                    MediaPlayer.Play(battleTheme);
                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    timer -= gameTime.ElapsedGameTime.TotalSeconds;
                    myBackground.Update(elapsed * 100);

                    if (timer <= 0)
                        currentGameState = GameState.EndLevel2;

                    break;
                case GameState.Level2Menu:
                    currentScore = 0;
                    livesRemaining = 5;

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    {
                        currentGameState = GameState.Level2;
                        SpriteManager.Enabled = true;
                        SpriteManager.Visible = true;
                    }
                    break;
                case GameState.EndLevel2:
                    SpriteManager.Enabled = false;
                    SpriteManager.Visible = false;

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        Exit();
                    break;
                case GameState.Gameover:
                    MediaPlayer.Stop();
                    loseThemeInstance.Volume = 0.80f;
                    loseThemeInstance.Play();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        Exit();
                    break;
                case GameState.GameWin:
                    MediaPlayer.Stop();
                    winThemeInstance.Volume = 1.00f;
                    winThemeInstance.Play();

                    if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                        currentGameState = GameState.Level2Menu;
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (currentGameState)
            {
                #region Start
                case GameState.Start:
                    SpriteBatch.Begin();

                    string text = "Harvest the red diamonds";
                    SpriteBatch.DrawString(scoreFont, text, new Vector2(50, 300), Color.Red);

                    text = "But avoid the blue ones";
                    SpriteBatch.DrawString(scoreFont, text, new Vector2(50, 350), Color.Blue);

                    text = "Press <Enter> or A button to start";
                    SpriteBatch.DrawString(scoreFont, text, new Vector2(50, 400), Color.White);

                    SpriteBatch.End();
                    break;
                #endregion

                #region Level1
                case GameState.Level1:
                    SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    SpriteBatch.Draw(background, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0);
                    SpriteBatch.DrawString(scoreFont, "Score: " + currentScore, new Vector2(10, 10), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    SpriteBatch.DrawString(scoreFont, "Lives: " + livesRemaining, new Vector2(10, 30), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    SpriteBatch.End();
                    break;
                #endregion

                #region Level2
                case GameState.Level2:
                    SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    myBackground.Draw(SpriteBatch);
                    SpriteBatch.DrawString(scoreFont, "Score: " + currentScore, new Vector2(10, 10), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    SpriteBatch.DrawString(scoreFont, "Lives: " + livesRemaining, new Vector2(10, 30), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    SpriteBatch.DrawString(scoreFont, "Time Remaining: " + Math.Round(timer, 2), new Vector2(200, 10), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    SpriteBatch.End();
                    break;
                #endregion

                #region Level2Menu
                case GameState.Level2Menu:
                    SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
                    string text2 = "See How many RED Diamonds you can collect in 60 Seconds!";
                    SpriteBatch.DrawString(scoreFont, text2, new Vector2(50, 300), Color.Red);

                    text2 = "Press <Enter> to start";
                    SpriteBatch.DrawString(scoreFont, text2, new Vector2(50, 350), Color.White);
                    SpriteBatch.End();
                    break;
                #endregion

                #region Gameover
                case GameState.Gameover:
                    SpriteBatch.Begin();
                    text = "Sorry, You lose!";
                    SpriteBatch.DrawString(scoreFont, text, new Vector2(50, 300), Color.Red);
                    text = "Press <Enter> to quit.";
                    SpriteBatch.DrawString(scoreFont, text, new Vector2(50, 320), Color.Red);
                    SpriteBatch.End();
                    break;
                #endregion

                #region GameWin
                case GameState.GameWin:
                    SpriteBatch.Begin();
                    text = "You Win!";
                    SpriteBatch.DrawString(scoreFont, text, new Vector2(50, 300), Color.Fuchsia);
                    text = "Press <Enter> to continue to Level 2!";
                    SpriteBatch.DrawString(scoreFont, text, new Vector2(50, 320), Color.Fuchsia);
                    SpriteBatch.End();
                    break;
                #endregion

                #region EndLevel2
                case GameState.EndLevel2:
                    SpriteBatch.Begin();
                    SpriteBatch.DrawString(scoreFont, "You Collected: " + currentScore, new Vector2(25, 300), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                    text = "Press <Enter> to quit.";
                    SpriteBatch.DrawString(scoreFont, text, new Vector2(50, 320), Color.Red);
                    SpriteBatch.End();
                    break;
                    #endregion
            }
            base.Draw(gameTime);
        }
    }
}
