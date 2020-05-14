using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPTS487_Game
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {

        // Game states
        private enum GameStates { Title, Playing, GameEnd };
        private GameStates gameState = GameStates.Title;
        private Texture2D titleScreen;
        private Texture2D gameEndScreen;
        private Texture2D playerSheet;
        private Texture2D playerProjectileSheet;
        private Texture2D enemyProjectileSheet;
        private PlayerFactory playerFactory;
        private EnemyFactory enemyFactory;
        private CollisionManager collisionManager;
        private GraphicsDeviceManager graphics;
        private LevelManager level;
        private Dictionary<int, Texture2D> enemySheet;
        private SpriteBatch spriteBatch;
        private SpriteFont Arial20;
        private Vector2 scoreDisplay = new Vector2(50, 50);
        private Vector2 healthDisplay = new Vector2(50, 700);

        public string wayPointsJSON;
        public string waveInfoJSON;
        public const int WindowWidth = 1024;
        public const int WindowHeight = 768;
       

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

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
            Arial20 = Content.Load<SpriteFont>(@"Fonts\Arial20");
            wayPointsJSON = (@"Content\JSONs\pathWaypoints.json");
            waveInfoJSON = (@"Content\JSONs\waveInfo.json");
            playerProjectileSheet = Content.Load<Texture2D>(@"graphics\BlinkingCircle");
            enemyProjectileSheet = Content.Load<Texture2D>(@"graphics\BlinkingCircle");
            playerSheet = Content.Load<Texture2D>(@"graphics\BlinkingTriangle");
            enemySheet = new Dictionary<int, Texture2D>();
            enemySheet[0] = Content.Load<Texture2D>(@"graphics\EnemyArrow");
            enemySheet[1] = Content.Load<Texture2D>(@"graphics\EnemyArrow2");
            enemySheet[2] = Content.Load<Texture2D>(@"graphics\BossEnemy");
            titleScreen = Content.Load<Texture2D>(@"graphics\TitleScreen");
            gameEndScreen = Content.Load<Texture2D>(@"graphics\GameOverScreen");

            // Level design is currently hard coded inside the level class...use JSON in future to read in level design
            level = new LevelManager();
            level.readWaypointsJson(wayPointsJSON);
            level.readWaveInfoJson(waveInfoJSON);

            playerFactory = new PlayerFactory(
                playerSheet, playerProjectileSheet, new Rectangle(0, 0, 50, 50), 4,
                new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height));

            enemyFactory = new EnemyFactory(
                enemySheet, enemyProjectileSheet, new Rectangle(0, 0, 50, 50), 4,
                new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height),
                level, playerFactory);

            collisionManager = new CollisionManager(playerFactory, enemyFactory);

            SoundManager.Initialize(Content);
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

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            switch (gameState)
            {
                case GameStates.Title:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        gameState = GameStates.Playing;
                    }                   
                    break;

                case GameStates.Playing:
                    if (playerFactory.player.dead)
                    {
                        gameState = GameStates.GameEnd;
                    }
                    playerFactory.Update(gameTime);
                    enemyFactory.Update(gameTime);
                    collisionManager.CheckCollision();
                    break;

                case GameStates.GameEnd:
                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        base.Initialize();
                        gameState = GameStates.Title;
                    }
                    break;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //draw graphics
            spriteBatch.Begin();

            if (gameState == GameStates.Title)
            {
                spriteBatch.Draw(titleScreen,
                    new Rectangle(0, 0, graphics.PreferredBackBufferWidth,
                    graphics.PreferredBackBufferWidth),
                    Color.White);
            }

            if (gameState == GameStates.Playing)
            {
                spriteBatch.DrawString (Arial20, 
                    "Score: " + playerFactory.player.Score.ToString(),
                    scoreDisplay, Color.White);
                spriteBatch.DrawString (Arial20,
                    "Health: " + playerFactory.player.Health.ToString(),
                    healthDisplay, Color.White);
                playerFactory.Draw(spriteBatch);
                enemyFactory.Draw(spriteBatch);
            }

            if (gameState == GameStates.GameEnd)
            {
                spriteBatch.Draw(gameEndScreen,
                    new Rectangle(0, 0, graphics.PreferredBackBufferWidth,
                    graphics.PreferredBackBufferWidth),
                    Color.White);
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
