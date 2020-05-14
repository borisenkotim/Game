using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPTS487_Game.Entities
{
    class Boss : Enemy
    {
        public Boss(Vector2 location, Texture2D spriteSheet, Rectangle initialFrame, int frameCount)
                               : base(location, spriteSheet, initialFrame, frameCount)
        {

        }

    }
}
