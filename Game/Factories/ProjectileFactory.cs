using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPTS487_Game
{

    // Projectile Factory class, an instance should be instantiated in both player and enemy.
    // Each instantiation maintains a List of projectiles fired by that object.
    // This will account for each projectiles source and remove the chance of player shots damaging the player
    // During collision detection checks.
    public class ProjectileFactory
    {
        // Maintain a list of fired shots
        public List<Projectile> Projectiles = new List<Projectile>();
        // The bounds of the view screen, remove projectiles when off screen
        private Rectangle screenBounds;

        // Animation Frame data
        private static Texture2D Texture;
        private static Rectangle InitialFrame;
        private static int FrameCount;
        private float shotSpeed;
        private static int CollisionRadius;
        Dictionary<string, Vector2[]> shotPatterns = new Dictionary<string, Vector2[]>();
        private struct Linesegment { public Vector2 point1; public Vector2 point2; }
        private struct IntersectionResult
        {
            public bool intersection;
            public bool coincident;
            public Vector2 intersectionPoint;
        }

        public ProjectileFactory(Texture2D texture, Rectangle initialFrame,
                                int frameCount, int collisionRadius, float shotSpeed,
                                Rectangle screenBounds)
        {
            Texture = texture;
            InitialFrame = initialFrame;
            FrameCount = frameCount;
            CollisionRadius = collisionRadius;
            this.shotSpeed = shotSpeed;
            this.screenBounds = screenBounds;
            shotPatterns.Add("triangle20", CreateBulletPattern(3, 0, 20));
            shotPatterns.Add("square20", CreateBulletPattern(4, 0, 20));
            shotPatterns.Add("hexagon20", CreateBulletPattern(6, 0, 20));
        }

        // When called create a new Projectile object and add to the List
        public void FireShot( Vector2 location, Vector2 velocity, bool playerFired)
        {
            Projectile thisShot = new Projectile(location, Texture, InitialFrame, velocity);

            thisShot.Velocity *= shotSpeed;

            for (int i = 1; i < FrameCount; i++)
            {
                thisShot.AddFrame(new Rectangle(
                    InitialFrame.X + (InitialFrame.Width * i),
                    InitialFrame.Y,
                    InitialFrame.Width,
                    InitialFrame.Height));
            }
            thisShot.CollisionRadius = CollisionRadius;

            if (playerFired == true)
            {
                SoundManager.PlayShotEffect();
            }

            Projectiles.Add(thisShot);
        }

        public void FireTriangle(Vector2 location)
        {
            foreach (Vector2 directionvector in shotPatterns["triangle20"])
            {
                FireShot(location, directionvector, false);
            }
        }

        public void FireSquare(Vector2 location)
        {
            foreach (Vector2 directionvector in shotPatterns["square20"])
            {
                FireShot(location, directionvector, false);
            }
        }

        public void FireHexagon(Vector2 location)
        {
            foreach (Vector2 directionvector in shotPatterns["hexagon20"])
            {
                FireShot(location, directionvector, false);
            }
        }

        // Bullet pattern generation "borrowed" from user Felsir on gamedev.stackexchange
        // from the following post https://gamedev.stackexchange.com/questions/124688/how-to-create-shockwave-of-objects-in-other-shapes-than-circle
        private Vector2[] CreateBulletPattern(int segments, double offset, int numberofbullets)
        {
            List<Linesegment> polygon = CreateRadialPolygon(segments, offset);
            List<Vector2> bulletdirections = new List<Vector2>();

            double bulletangle = (2 * Math.PI) / numberofbullets;
            for (int i = 0; i < numberofbullets; i++)
            {
                foreach (Linesegment l in polygon)
                {
                    //do an intersection; if the line intersects add a bullet to the pattern.
                    IntersectionResult r = ProcessIntersection(l, Vector2.Zero, i * bulletangle);
                    if (r.intersection)
                    {
                        //check if the intersectionpoint already is in the result- 
                        //this may be if the intersectionpoint is one of the endpoints of the line segment.
                        if (!bulletdirections.Contains(r.intersectionPoint))
                            bulletdirections.Add(r.intersectionPoint);
                    }
                }
            }
            return bulletdirections.ToArray();
        }

        // Bullet pattern generation "borrowed" from user Felsir on gamedev.stackexchange
        // from the following post https://gamedev.stackexchange.com/questions/124688/how-to-create-shockwave-of-objects-in-other-shapes-than-circle
        private List<Linesegment> CreateRadialPolygon(int segments, double offsetangle)
        {
            if (segments < 3)
                throw new Exception("Polgon should have at least 3 segments");

            List<Linesegment> result = new List<Linesegment>();
            double theta = (2 * Math.PI) / (double)segments;

            for (int i = 0; i < segments; i++)
            {
                result.Add(new Linesegment()
                {
                    point1 = new Vector2((float)Math.Cos(offsetangle + (i * theta)), (float)Math.Sin(offsetangle + (i * theta))),
                    point2 = new Vector2((float)Math.Cos(offsetangle + ((i + 1) * theta)), (float)Math.Sin(offsetangle + ((i + 1) * theta)))
                });

            }
            return result;
        }

        // Bullet pattern generation taken from user Felsir on gamedev.stackexchange
        // from the following post https://gamedev.stackexchange.com/questions/124688/how-to-create-shockwave-of-objects-in-other-shapes-than-circle
        private IntersectionResult ProcessIntersection(Linesegment linesegment, Vector2 origin, double theta)
        {
            //since this is linesegment versus linesegment code; transform the ray into a point somewhere far away...
            Vector2 rayend = origin + new Vector2((float)Math.Cos(theta), (float)Math.Sin(theta)) * 1000;

            float ua = (rayend.X - origin.X) * (linesegment.point1.Y - origin.Y) - (rayend.Y - origin.Y) * (linesegment.point1.X - origin.X);
            float ub = (linesegment.point2.X - linesegment.point1.X) * (linesegment.point1.Y - origin.Y) - (linesegment.point2.Y - linesegment.point1.Y) * (linesegment.point1.X - origin.X);
            float denominator = (rayend.Y - origin.Y) * (linesegment.point2.X - linesegment.point1.X) - (rayend.X - origin.X) * (linesegment.point2.Y - linesegment.point1.Y);

            IntersectionResult result = new IntersectionResult { intersection = false, coincident = false, intersectionPoint = Vector2.Zero };

            if (Math.Abs(denominator) <= 0.00001f) // epsilon, check if the point is on the line (within a very small distance for rounding errors).
            {
                if (Math.Abs(ua) <= 0.00001f && Math.Abs(ub) <= 0.00001f)
                {
                    result.intersection = true;
                    result.coincident = true;
                    result.intersectionPoint = (linesegment.point1 + linesegment.point2) / 2;
                }
            }
            else
            {
                ua /= denominator;
                ub /= denominator;

                if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
                {
                    result.intersection = true;
                    result.intersectionPoint.X = linesegment.point1.X + ua * (linesegment.point2.X - linesegment.point1.X);
                    result.intersectionPoint.Y = linesegment.point1.Y + ua * (linesegment.point2.Y - linesegment.point1.Y);
                }
            }
            return result;
        }

        // Loop backwards, remove shots that have expired...cant loop forward, will need to restart iteration after each removal
        public void Update(GameTime gameTime)
        {
            // Check each projectile and verify still within view screen
            // In not, remove
            for (int x = Projectiles.Count - 1; x >= 0; x--)
            {
                Projectiles[x].Update(gameTime);
                if (!screenBounds.Intersects(Projectiles[x].Destination)||Projectiles[x].dead == true)
                {
                    Projectiles.RemoveAt(x);
                }
            }
        }

        // Iterate through each projectile and tell the projectile to re-draw itself
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Projectile shot in Projectiles)
            {
                shot.Draw(spriteBatch);
            }
        }


    }
}
