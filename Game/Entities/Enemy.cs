using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPTS487_Game
{
    public class Enemy : Entity
    {   
        private int scoreValue = 10;
        private int health = 2;
        // 0=grunt1, 1=grunt2, 2=midboss, 3=finalboss
        private int type;

        // Track time since last shot...limit shot rate
        public float shotTimer = 0.0f;
        public float minShotTimer = 0.5f;
        // % chance to shoot a projectile every update
        // Could be scripted if desired...probably needs to be for bosses
        public float enemyShotRate = 1.5f;
        // Shots should not come from center of enemy object...offset
        public Vector2 shotOffset = new Vector2(25, 25);
        // Used for buffering collision detection
        public int enemyRadius = 15;

        public Enemy(Vector2 location, Texture2D spriteSheet, Rectangle initialFrame, int frameCount)
                                :base(location, spriteSheet, initialFrame, frameCount)
        {

        }

        // Score getter/setter
        public int ScoreValue
        {
            get { return scoreValue; }
            set { scoreValue = value; }
        }

        // Health getter/setter
        public int Health
        {
            get { return health; }
            set { health = value; }
        }

        // Type getter/setter
        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        // Add a waypoint to the enemy, this should be used to create a scripted
        // behaviour for the enemy...read in list of waypoints from JSON later
        public void AddWaypoint(Vector2 waypoint)
        {
            waypoints.Enqueue(waypoint);
        }

        // Check to see if enemy has reached its currently defined waypoint
        public bool ReachedWaypointCheck()
        {
            // Cannot make this exact, enemy will just keep wiggling around the waypoint forever
            // Just need to check if it is "close enough"
            // Currently "close enough" = half of the sprites width
            // Fine tune this in later versions
            if (Vector2.Distance(this.Location, currentWaypoint) <
                (float)this.Source.Width / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsActiveCheck()
        {
            // Player has killed enemy, no longer active
            if (dead)
            {
                return false;
            }

            // Enemy still has more waypoints to navigate...still active
            if (waypoints.Count > 0)
            {
                return true;
            }

            // Waypoint queue is empty and the current waypoint has been reached.
            // Final waypoint for all enemies should go off screen so that the
            // enemy doesn't just disapear while on screen.
            // Reaching this point means that the player didn't destroy the enemy
            // and it has completed its script...despawn
            if (ReachedWaypointCheck())
            {
                return false;
            }

            // Enemy is not dead and waypoint queue is not empty...
            // still navigating to next waypoint
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            // If enemy is still active...
            if (IsActiveCheck())
            {
                // set direction vector to the location enemy is attempting to reach
                Vector2 dirVector = currentWaypoint - this.Location;
                // Cannot normalize a vector of zero...check
                // Normalize = vector/vector.length = length of one
                // if vector is zero results in a divide by zero...computer dont like
                if (dirVector != Vector2.Zero)
                {
                    // Normalize is used to ensure a vector length of 1 in all conditions
                    // If not normalized, when the enemy has both up/down and left/right movement
                    // A vector length of 1.4 would result...moving faster diagonally (unwanted behaviour)
                    // Physics and shit
                    // Normalize = vector/vector.length = length of one
                    dirVector.Normalize();
                }
                // Multiply by enemy speed
                dirVector *= moveSpeed;
                this.Velocity = dirVector;
                // Save current location as the new "previousWaypoint"
                previousWaypoint = this.Location;
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
                // Call base entity update method to move the enemy
                base.Update(gameTime);
                // Find the angle for the direction the enemy is headed...rotate frame to that direction
                // angle of 0 indicates moving right across the screen on the horizontal axis
                // Boss types (type 2 and type 3) should not rotate
                if (this.Type == 0 || this.Type == 1)
                {
                    this.Rotation = (float)Math.Atan2(
                               this.Location.Y - previousWaypoint.Y,
                               this.Location.X - previousWaypoint.X);
                }               

                // Check to see if the enemy has reached its current waypoint
                // If it has, move to the next one
                if (ReachedWaypointCheck())
                {
                    // ...If there are still waypoints in the Queue
                    if (waypoints.Count > 0)
                    {
                        currentWaypoint = waypoints.Dequeue();
                    }
                }
            }
        }

        // If active draw...
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActiveCheck())
            {
                base.Draw(spriteBatch);
            }
        }
    }
}
