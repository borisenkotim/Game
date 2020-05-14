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
    public class Player : Entity
    {
        //player characterisctics
        private int score = 0;
        private int health = 5;
        private int maxHealth = 20;
        private int lives = 3;

        // Track time since last shot...limit shot rate
        public float shotTimer = 0.0f;
        public float minShotTimer = 0.2f;
        // Shots should not come from center of player object...offset
        public Vector2 shotOffset = new Vector2(15, 10);       
        //used for buffering collision detection
        public int playerRadius = 15;

        

        public Player(Vector2 location, Texture2D spriteSheet, Rectangle initialFrame, Vector2 velocity)
                           : base(location, spriteSheet, initialFrame, velocity)
        {
        }

        // Constructor which includes playerSpeed as an attribute
        public Player(Vector2 location, Texture2D spriteSheet, Rectangle initialFrame, Vector2 velocity,
                      float enemySpeed)
        {
        }

        // Score getter/setter
        public int Score
        {
            get { return score; }
            set { score = value; }
        }

        // Health getter/setter
        public int Health
        {
            get { return health; }
            set {
                // Health should not exceed max health
                health = value;
                if (health > maxHealth)
                {
                    health = maxHealth;
                }               
            }
        }

        // MaxHealth getter/setter
        public int MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }

        // Lives getter/setter
        public int Lives
        {
            get { return lives; }
            set { lives = value; }
        }
    }   
}
