using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPTS487_Game
{
    class CollisionManager
    {
        // References to the player and enemy factories
        private PlayerFactory playerFactory;
        private EnemyFactory enemyFactory;

        public CollisionManager(PlayerFactory playerFactory, EnemyFactory enemyFactory)
        {
            this.playerFactory = playerFactory;
            this.enemyFactory = enemyFactory;
        }


        // If enemy is hit decrease health by projectile damage value and if enemy dies
        // increase player score by enemies score value.
        private void playerShotCheck()
        {
            foreach (Projectile projectile in playerFactory.playerShots.Projectiles)
            {
                foreach (Enemy enemy in enemyFactory.Enemies)
                {
                    if (projectile.IsBoxColliding(enemy.PaddedBox))
                    {
                        projectile.dead = true;
                        enemyFactory.ProcessCollision(enemy, projectile.ProjectileDamage);
                        if (enemy.dead)
                        {
                            playerFactory.player.Score += enemy.ScoreValue;
                        }
                    }
                }
            }
        }

        // If player is hit decrease health by projectile damage value and play hit sound...kill projectile
        private void enemyShotCheck()
        {
            foreach (Projectile projectile in enemyFactory.EnemyShots.Projectiles)
            {
                if (projectile.IsBoxColliding(playerFactory.player.PaddedBox))
                {
                    projectile.dead = true;
                    SoundManager.PlayHitEffect();
                    playerFactory.ProcessCollision(playerFactory.player, projectile.ProjectileDamage);
                }
            }
        }

        public void CheckCollision()
        {
            playerShotCheck();
            enemyShotCheck();
        }
    }
}
