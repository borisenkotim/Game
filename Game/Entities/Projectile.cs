using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPTS487_Game
{
    public class Projectile : Entity
    {
        private int projectileDamage = 1;

        public Projectile(Vector2 location, Texture2D spriteSheet, Rectangle initialFrame, Vector2 velocity) 
                            : base(location, spriteSheet, initialFrame, velocity)
        {
        }

        // Projectile Damage getter/setter
        public int ProjectileDamage
        {
            get { return projectileDamage; }
            set { projectileDamage = value; }
        }
    }
}
