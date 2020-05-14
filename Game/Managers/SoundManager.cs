using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace CPTS487_Game
{
    // All sounds used were randomly generated using project_sfxr
    // http://www.drpetter.se/project_sfxr.html
 
    public static class SoundManager
    {        
        private static SoundEffect playerHit;
        private static SoundEffect playerShot;
        private static SoundEffect explosion;

        public static void Initialize (ContentManager content)
        {
            try
            {
                playerHit = content.Load<SoundEffect>(@"Sounds\hit");
                playerShot = content.Load<SoundEffect>(@"Sounds\shoot");
                explosion = content.Load<SoundEffect>(@"Sounds\explosion");
            }
            catch
            {
                Debug.Write("Sound Effect initialization failed");
            }
        }

        public static void PlayHitEffect()
        {
            try
            {
                playerHit.Play();
            }
            catch
            {
                Debug.Write("PlayerHit Sound Effect Failed");
            }
        }

        public static void PlayShotEffect()
        {
            try
            {
                playerShot.Play();
            }
            catch
            {
                Debug.Write("PlayerShot Sound Effect Failed");
            }
        }

        public static void PlayExplosionEffect()
        {
            try
            {
                explosion.Play();
            }
            catch
            {
                Debug.Write("Explosion Sound Effect Failed");
            }
        }
    }
}
