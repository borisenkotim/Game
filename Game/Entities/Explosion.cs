using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CPTS487_Game
{
    // This whole class is obsolete now, need to refactor entire thing...from previous attempts at creating an explosion
    class Explosion
    {
        // strip location and draw rectangle
        string StripName = @"graphics\explosion";
        Rectangle drawRectangle;
        Texture2D spriteStrip;

        // strip parameters
        int frameWidth;
        int frameHeight;
        // strip information should be moved to external file later. 
        // only here for testing purposes
        int framesPerRow = 3;
        int NumRows = 3;
        int NumFrames = 9;

        // for tracking and drawing animation
        Rectangle sourceRectangle;
        int currentFrame;
        int TotalFrameMilliseconds = 10;
        int elapsedFrameMilliseconds = 0;

        // bool for tracking playing status
        bool playing = false;

        // Constructor
        public Explosion (ContentManager contentManager)
        {
            // start animation at frame 0
            currentFrame = 0;

            LoadContent(contentManager);
        }

        public void Update (GameTime gameTime)
        {
            if (playing)
            {
                // update the ammount of time current frame has been displayed
                // ElapsedGameTime returns how much time has elapsed since last update
                elapsedFrameMilliseconds += gameTime.ElapsedGameTime.Milliseconds;

                // if elapsed time is greater than the desired frame time
                // if so reset elapsed time and move to next frame
                // if not, no action during this update
                if (elapsedFrameMilliseconds > TotalFrameMilliseconds)
                {
                    //reset
                    elapsedFrameMilliseconds = 0;
                }

                // verify there is a next frame to move to
                if (currentFrame < NumFrames - 1)
                {
                    currentFrame++;
                    SetSourceRectangleLocation(currentFrame);
                }

                // else, no more frames...end animation sequence
                else
                {
                    // animation has completed
                    playing = false;
                }
            }
        }

        // draw the object if playing is true
        // should be event driven, only starts playing when triggered
        public void Draw (SpriteBatch spriteBatch)
        {
            if (playing)
            {
                spriteBatch.Draw(spriteStrip, drawRectangle, sourceRectangle, Color.White);
            }
        }

        // Play animation at selected x,y location
        public void Play (int x, int y)
        {
            // make playing true and start animation at first frame
            playing = true;
            elapsedFrameMilliseconds = 0;
            currentFrame = 0;

            // location data, shouldn't break wih changing resolutions
            drawRectangle.X = x - drawRectangle.Width / 2;
            drawRectangle.Y = y - drawRectangle.Height / 2;
            SetSourceRectangleLocation(currentFrame);
        }

        public void LoadContent (ContentManager contentManager)
        {
            // load selected sprite strip...currently only one
            spriteStrip = contentManager.Load<Texture2D>(StripName);

            // get frame size, currently only one strip but could implement new option with more frames
            frameWidth = spriteStrip.Width / framesPerRow;
            frameHeight = spriteStrip.Height / NumRows;

            // draw and source rectangles
            drawRectangle = new Rectangle(0, 0, frameWidth, frameHeight);
            sourceRectangle = new Rectangle(0, 0, frameWidth, frameHeight);
           
        }

        // set source rectangle to allign with given frame
        // source rectangle iterates through the frames of the spriteStrip
        // left to right and top to bottom
        public void SetSourceRectangleLocation (int frameNumber)
        {
            // calculate x and y coordinates based on frame #
            sourceRectangle.X = (frameNumber % framesPerRow) * frameWidth;
            sourceRectangle.Y = (frameNumber / framesPerRow) * frameHeight;
        }

    }
}
