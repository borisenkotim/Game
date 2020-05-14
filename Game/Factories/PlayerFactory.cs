using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CPTS487_Game
{
    class PlayerFactory
    {
		// Enum for fast or slow selection
		enum VelocitySelection { Slow, Fast };
        // Start at fast initially
        VelocitySelection velocitySelection = VelocitySelection.Fast;
		// Player object
        public Player player;
		// Projectile Factory for managing the players projectiles
        public ProjectileFactory playerShots; 
		// Used to enforce player bounds
        private Rectangle playerBounds;   

		// Constructor...graphic name, Rectangle for the initial animation frame, #frames in strip, Rectangle for view size
		public PlayerFactory(Texture2D graphic, Texture2D shotGraphic, Rectangle initialFrame, int frameCount, Rectangle screenBounds)
        {
            player = new Player(new Vector2(512, 650), graphic, initialFrame, Vector2.Zero);
            playerShots = new ProjectileFactory(shotGraphic, new Rectangle(0, 0, 16, 16), 3, 2, 250f, screenBounds);
            //player is limited to the bottom half of the screen
            playerBounds = new Rectangle(0, screenBounds.Height / 2, screenBounds.Width, screenBounds.Height / 2);

			for (int i = 1; i < frameCount; i++)
            {
                player.AddFrame(
                    new Rectangle(
						// iterate through frames...animation frames should be of equal width
                        initialFrame.X + (initialFrame.Width * i),
                        initialFrame.Y,
                        initialFrame.Width,
                        initialFrame.Height));
            }
            player.CollisionRadius = player.playerRadius;
        }        

		// If the min ammount of time since last shot has expired, allow player to fire a projectile
        private void SendShotRequest()
        {
            if (player.shotTimer >= player.minShotTimer)
            {
                playerShots.FireShot(
                    player.Location + player.shotOffset,
                    new Vector2(0, -1),
                    true);
                player.shotTimer = 0.0f;
            }
        }

        // Method for use with keyboard controls
        private void HandleKeyboardInput(KeyboardState keyboard)
        {
            // Keyboard control, WASD controls
            if (keyboard.IsKeyDown(Keys.A))
            {
                player.Velocity += new Vector2(-1, 0);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                player.Velocity += new Vector2(1, 0);
            }
            if (keyboard.IsKeyDown(Keys.W))
            {
                player.Velocity += new Vector2(0, -1);
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                player.Velocity += new Vector2(0, 1);
            }
            if (keyboard.IsKeyDown(Keys.Space))
            {
                SendShotRequest();
            }
			if (keyboard.IsKeyDown(Keys.F))
            {
                velocitySelection = VelocitySelection.Slow;
            }
			if (keyboard.IsKeyDown(Keys.G))
            {
                velocitySelection = VelocitySelection.Fast;
            }
        }

        // Method for use with gamepad controls...for future implementation
        private void HandleControllerInput(GamePadState gamePad)
        {

        }

		// Limit player movement to the area defined by th view screen
		// Moving off screen is not allowed.
        private void enforceBounds()
        {
            Vector2 location = player.Location;

            if (location.X < playerBounds.X)
                location.X = playerBounds.X;

            if (location.X > (playerBounds.Right - player.Source.Width))
                location.X = (playerBounds.Right - player.Source.Width);

            if (location.Y < playerBounds.Y)
                location.Y = playerBounds.Y;

            if (location.Y >
                (playerBounds.Bottom - player.Source.Height))
                location.Y = (playerBounds.Bottom - player.Source.Height);

            player.Location = location;
        }

        public void ProcessCollision (Player player, int value)
        {
            player.Health -= value;
            
            if (player.Health <= 0)
            {
                player.dead = true;
            }
        }

        // If player is not dead, update
        public void Update(GameTime gameTime)
        {
            playerShots.Update(gameTime);

            if (player.dead == false)
            {
                player.Velocity = Vector2.Zero;

                player.shotTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                HandleKeyboardInput(Keyboard.GetState());

                // Normalize is used to ensure a vector length of 1 in all conditions
                // If not normalized, when the player has both up/down and left/right
                // A vector length of 1.4 would result...moving faster diagonally (unwanted behaviour)
                // Normalize = vector/vector.length = length of one
                // Physics and shit
                player.Velocity.Normalize();

                switch (velocitySelection)
                {
                    case VelocitySelection.Slow:
                        player.Velocity *= player.moveSpeed/2;
                        break;

                    case VelocitySelection.Fast:
                        player.Velocity *= player.moveSpeed;
                        break;
                }

                player.Update(gameTime);
                enforceBounds();
            }
        }

		// If player is not dead, draw
        public void Draw(SpriteBatch spriteBatch)
        {
            playerShots.Draw(spriteBatch);

            if (player.dead == false)
            {
                player.Draw(spriteBatch);
            }
        }
    }
}
