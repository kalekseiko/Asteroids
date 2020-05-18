using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using AsteroidMono;


namespace AsteroidMono
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 

    // Состояния игры
    enum States
    {
        SplashScreen,
        Game,
        GameOver,
        Pause
    }


    public class Game1 : Game
    {
        
        GraphicsDeviceManager graphics; // основное устройство вывода графики - видеокарта
        SpriteBatch spriteBatch; // буфер для всех спрайтов
        States stat = States.SplashScreen;

        // размеры игрового окна
        static int screenWidth;
        static int screenHeight;

        static GameTime currentGameTime;

        static public int ScreenWidth
        {
            get
            {
                return screenWidth;
            }
        }
        static public int ScreenHeight
        {
            get
            {
                return screenHeight;
            }
        }

        static public GameTime getCurrentGameTime
        {
            get { return currentGameTime; }
        }

        public Game1()
        {
            screenWidth = 1680;
            screenHeight = 1050;
            //gameTime = new GameTime();
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // задаем размер игрового окна
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges(); // применяем три верхние строчки
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            SplashScreen.Background = Content.Load<Texture2D>("img/SplashScreenImg2"); // картинка для заставки
            SplashScreen.HeaderFont = Content.Load<SpriteFont>("fonts/SplashScreenFont"); // шрифт для главных надписей
            SplashScreen.TextFont = Content.Load<SpriteFont>("fonts/TextFont"); // шрифт для остальных надписей
            GameOver.Background = Content.Load<Texture2D>("img/GameOver"); // картинка для заставки
            GameOver.HeaderFont = Content.Load<SpriteFont>("fonts/TextFont"); // шрифт для главных надписей
            GameOver.TextFont = Content.Load<SpriteFont>("fonts/LittleFont"); // шрифт для остальных надписей
            
            Asteroids.Init(spriteBatch, screenWidth, screenHeight);
            Background.Texture2D = Content.Load<Texture2D>("spriteMaps/Backgrounds");
            Star.Texture2D = Content.Load<Texture2D>("spriteMaps/SpriteMapStars");
            StarShip.Texture2D = Content.Load<Texture2D>("spriteMaps/spaceship");
            SustainerEngine.Texture2D = Content.Load<Texture2D>("spriteMaps/JetStreamSprite");
            BigFire.Texture2D = Content.Load<Texture2D>("spriteMaps/PlazmaBullet");
            Asteroid.Texture2D = Content.Load<Texture2D>("spriteMaps/Asteroids");
            Blast.Texture2D = Content.Load<Texture2D>("spriteMaps/blast");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();
            switch(stat)
            {
                case States.Game:
                    if (keyboardState.IsKeyDown(Keys.Escape)) stat = States.SplashScreen;
                    Asteroids.Update();
                    if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)) Asteroids.StarShip1.Up();
                    if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)) Asteroids.StarShip1.Down();
                    if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A)) 
                    {
                        Asteroids.StarShip1.Left();
                        Asteroids.SustainerEngine1.isOn = false;
                    }
                    if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D)) Asteroids.StarShip1.Right();
                    if ((keyboardState.IsKeyDown(Keys.Space)) || (mouseState.LeftButton == ButtonState.Pressed)) Asteroids.Shoot();
                    if ((keyboardState.IsKeyUp(Keys.Space)) && (mouseState.LeftButton == ButtonState.Released)) Asteroids.fireTimerCounter = 0;

                    if (Asteroids.StarShip1.Strength < 1 ) {
                        stat=States.GameOver;
                    }
                    break;
                case States.SplashScreen:
                    SplashScreen.Update();
                    if (keyboardState.IsKeyDown(Keys.Space) || (mouseState.LeftButton == ButtonState.Pressed))
                        {
                            stat = States.Game;
                        }
                    break;
                case States.GameOver:
                    GameOver.Update();
                    if (keyboardState.IsKeyDown(Keys.Space)) 
                        {
                            Asteroids.StarShip1.Strength = 100;
                            stat = States.Game;
                        }
                    break;
                    
                
            }

            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed )
                Exit();

            base.Update(gameTime);
            currentGameTime = gameTime;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            switch (stat)
            {
                case States.SplashScreen:
                    SplashScreen.Draw(spriteBatch);
                    break;
                case States.Game:
                    Asteroids.Draw();
                    break;
                case States.GameOver:
                    GameOver.Draw(spriteBatch);
                    Asteroids.Reset();
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
