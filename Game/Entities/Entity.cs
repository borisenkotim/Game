using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPTS487_Game
{
    public class Entity
    {
        // SpriteSheet variable points to the sprite sheet for the animation
        public Texture2D SpriteSheet;
        //Queue of waypoints for enemy to follow
        public Queue<Vector2> waypoints = new Queue<Vector2>();
        // Location of current waypoint
        public Vector2 currentWaypoint = Vector2.Zero;
        // Location of previous waypoint
        public Vector2 previousWaypoint = Vector2.Zero;

        // Animation frame data, each frame will be contained in the frames List
        // Current Frame holds the currently displayed frame in the animation
        private List<Rectangle> frames = new List<Rectangle>();
        private int frameWidth = 0;
        private int frameHeight = 0;
        private int currentFrame;
        private float frameTime = 0.1f;
        private float currentFrameTime = 0.0f;
        private Color tint = Color.White;
        // angle of 0 indicates moving right across the screen on the horizontal axis
        private float rotation = 0.0f;

        // Collision Detection data
        public int CollisionRadius = 0;
        public int XPadding = 0;
        public int YPadding = 0;

        // Location and Velocity Data
        private Vector2 location = Vector2.Zero;
        private Vector2 velocity = Vector2.Zero;

        // Entity Characteristics        
        public bool dead = false;
        public float moveSpeed = 300.0f;
        

        // Base Constructor
        public Entity()
        {

        }

        // Constructor, should be self explanatory
        public Entity (Vector2 location, Texture2D spriteSheet, Rectangle initialFrame, Vector2 velocity)
        {
            this.location = location;
            this.velocity = velocity;
            SpriteSheet = spriteSheet;
            frames.Add(initialFrame);
            frameWidth = initialFrame.Width;
            frameHeight = initialFrame.Height;
        }

        // Constructor allows adding animation frame count during instantiation
        // Waypoints are used for enemy scripting
        // Mainly for enemy instantiation, scripting of projectiles may be useful...not sure yet
        public Entity (Vector2 location, Texture2D spriteSheet, Rectangle initialFrame, int frameCount)
        {
            this.Location = location;
            SpriteSheet = spriteSheet;
            frames.Add(initialFrame);
            frameWidth = initialFrame.Width;
            frameHeight = initialFrame.Height;

            for (int i =1; i < frameCount; i++)
            {
                this.AddFrame(new Rectangle(
                    initialFrame.X = initialFrame.Width * i,
                    initialFrame.Y, initialFrame.Width, initialFrame.Height));
            }
            //set current and previous to spawn point upon instantiation
            previousWaypoint = location;
            currentWaypoint = location;
        }

        // Location getter/setter
        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        // Velocity getter/setter
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        // Tint color getter/setter
        public Color TintColor
        {
            get { return tint; }
            set { tint = value; }
        }

        // Rotation getter/setter
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value % MathHelper.TwoPi; }
        }

        // Animation frame getter/setter...does not allow for setting outside of frames contained in frame list
        public int Frame
        {
            get { return currentFrame; }
            set { currentFrame = (int)MathHelper.Clamp(value, 0, frames.Count - 1); }
        }

        // Frame time getter/setter...allows speed at which animation gets updated to be adjusted
        // Frame time of 0 will update its frame every Update() cycle
        public float FrameTime
        {
            get { return frameTime; }
            set { frameTime = MathHelper.Max(0, value); }
        }

        // Returns the rectangle associated with the currentFrame from the frames List
        public Rectangle Source
        {
            get { return frames[currentFrame]; }
        }

        // Builds a new Rectangle based on the sprites current screen location and frame width/height
        public Rectangle Destination
        {
            get
            {
                return new Rectangle(
                  (int)location.X,
                  (int)location.Y,
                  frameWidth,
                  frameHeight);
            }
        }

        // Calculates the center of the animation frame
        // location offset by half the width and height
        public Vector2 Center
        {
            get
            {
                return location + new Vector2(frameWidth / 2, frameHeight / 2);
            }
        }

        // Padded box returns a rectangle which ignores padding value on each side during
        // colision detection...allows for fine tuning the collision detection
        public Rectangle PaddedBox
        {
            get
            {
                return new Rectangle(
                    (int)location.X + XPadding,
                    (int)location.Y + YPadding,
                    frameWidth - (XPadding * 2),
                    frameHeight - (YPadding * 2));
            }
        }

        // Collision detection check, returns true if the two boxes overlap
        // Uses the Rectangle objects Intersects() method to check
        public bool IsBoxColliding (Rectangle OtherBox)
        {
            return PaddedBox.Intersects(OtherBox);
        }

        // Method for adding an animation frame
        public void AddFrame(Rectangle frameRectangle)
        {
            frames.Add(frameRectangle);
        }

        // When update is called track time since last update, if time is greater than frameTime
        // move to next animation frame and reset currentFrameTime..if at last frame, move back to first frame
        public virtual void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            currentFrameTime += elapsed;

            if (currentFrameTime >= FrameTime)
            {
                currentFrame = (currentFrame + 1) % (frames.Count);
                currentFrameTime = 0.0f;
            }

            location += (velocity * elapsed);
        }

        // Call to the spriteBatch draw method which allows for rotation and resizing
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(SpriteSheet, Center, Source, TintColor, rotation,
                            new Vector2(frameWidth / 2, frameHeight / 2),
                            1.0f, //scale argument...resize
                            SpriteEffects.None,
                            0.0f);
        }
    }
}
