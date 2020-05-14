using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPTS487_Game
{
    // Factory class responsible for spawning enemy objects...should be script driven
    class EnemyFactory
    {
        private Dictionary <int, Texture2D> graphics;        
        private int frameCount;
        private int waveCount = 0;
        // Maintain a reference to the player factory so that enemy's can shoot at player location
        private PlayerFactory playerFactory;
        // The level object which defines enemy behaviour
        private LevelManager curLevel;
        // Used for random chance of enemy firing a projectile
        private Random rand = new Random();
        // Tracks whether the current level is still active
        public bool Active = true;
        // Maintain a list of enemy objects
        public List<Enemy> Enemies = new List<Enemy>();
        // Projectile factory for firing enemy shots
        public ProjectileFactory EnemyShots;       
        // Current wave timer
        public float waveTimer = 0.0f;
        // Next wave timer
        public float nextWaveTimer = 15.0f;
        // timer for current spawn
        public float spawnTimer = 0.0f;
        // time between enemy spawns
        public float spawnInterval = 0.5f;
       

        // Constructor
        public EnemyFactory(Dictionary <int,Texture2D> graphics, Texture2D shotGraphic, Rectangle initialFrame, 
                            int frameCount, Rectangle screenBounds, LevelManager level, PlayerFactory playerFactory)
        {
            this.graphics = graphics;           
            this.frameCount = frameCount;
            this.playerFactory = playerFactory;
            this.curLevel = level;

            EnemyShots = new ProjectileFactory(shotGraphic, new Rectangle(0, 0, 16, 16), 3, 2, 200f, screenBounds);
        }

        // Construct enemy and add the waypoints assigned to its queue
        // type coresponds to the graphic dictionary index for the graphic you wish to use
        public void SpawnEnemy(int type, Rectangle frameSize, int wave, Queue<Vector2> path)
        {
            Enemy thisEnemy = new Enemy(path.Dequeue(), graphics[type], frameSize, frameCount);
            thisEnemy.waypoints = path;
            thisEnemy.Type = type;
            if (thisEnemy.Type == 0)
            {
                // This is probably not a good place to be setting these values...refactor later
                thisEnemy.Health = 1;
                thisEnemy.ScoreValue = 10;
            }
            else if (thisEnemy.Type == 1)
            {
                thisEnemy.Health = 2;		// sets the health score
                thisEnemy.ScoreValue = 20;
            }
            else
            {
                thisEnemy.Health = 20;
                thisEnemy.ScoreValue = 100;
            }
            Enemies.Add(thisEnemy);
        }

        private void updateWaveSpawns(GameTime gameTime)
        {
            Queue<LevelManager.EnemyInfo> curWave = new Queue<LevelManager.EnemyInfo>();
            curWave = curLevel.waveInfo[waveCount];

            // If spawn timer is greater than spawn interval...next enemy in the wave can spawn
            spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (spawnTimer > spawnInterval)
            {
                if (curWave.Count() > 0)
                {
                    LevelManager.EnemyInfo info = curLevel.waveInfo[waveCount].Dequeue();
                    SpawnEnemy(info.type, info.frameSize, waveCount, info.path);
                }
                // Reset spawn timer when new enemy has been spawned
                spawnTimer = 0f;
            }

            // If nextWaveTimer is greater than 
            waveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (waveTimer > nextWaveTimer)
            {
                if (waveCount < curLevel.waveInfo.Count() - 1)
                {
                    waveCount++;
                }                              
                waveTimer = 0f;
            }
        }

        public void ProcessCollision(Enemy target, int value)
        {
            target.Health -= value;

            if (target.Health <= 0)
            {
                SoundManager.PlayExplosionEffect();
                target.dead = true;
            }
        }

        // not yet implemented
        public void SendShotRequest(Enemy enemy)
        {
            if (enemy.shotTimer >= enemy.minShotTimer)
            {
                
            }
        }

        // Update method for enemy objects
        public void Update(GameTime gameTime)
        {
            EnemyShots.Update(gameTime);

            for (int x = Enemies.Count - 1; x >= 0; x--)
            {
                Enemies[x].Update(gameTime);

                // If enemy is no longer active remove from enemies list
                if (Enemies[x].IsActiveCheck() == false)
                {
                    Enemies.RemoveAt(x);
                }
                // Else, begin projectile firing protocol
                else
                {
                    // Randomized fire shot for enemies
                    if ((float)rand.Next(0, 1000) / 10 <= Enemies[x].enemyShotRate)
                    {
                        Vector2 fireLoc = Enemies[x].Location;
                        fireLoc += Enemies[x].shotOffset;
                        Vector2 shotDirection;
                        // Enemies will fire in the direction of the player
                        switch (Enemies[x].Type)
                        {
                            //type 0: grunt 1
                            //type 1: grunt 2
                            //type 2: midboss
                            case 0:
                                shotDirection = playerFactory.player.Center - fireLoc;
                                shotDirection.Normalize();
                                EnemyShots.FireShot(fireLoc, shotDirection, false);
                                break;

                            case 1:
                                shotDirection = new Vector2(0, 1);
                                shotDirection.Normalize();
                                EnemyShots.FireShot(fireLoc, shotDirection, false);
                                break;

                            case 2:
                                shotDirection = new Vector2(0, 1);
                                EnemyShots.FireTriangle(fireLoc);

                                //shotDirection = playerFactory.player.Center - fireLoc;
                                //shotDirection.Normalize();
                                //EnemyShots.FireShot(fireLoc, shotDirection, false);
                                break;
                        }
                        
                        // Normalize vector for constant projectile speed in all directions
                        

                        //Create new projectile and add to List
                        
                    }
                }
            }
            // If level is still active, update.
            if (Active)
            {
                updateWaveSpawns(gameTime);
            }
        }

            // Draw enemies and their projectiles
            public void Draw(SpriteBatch spriteBatch)
        {
            EnemyShots.Draw(spriteBatch);

            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
    }
}
