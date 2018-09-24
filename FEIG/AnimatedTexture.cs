// Written by Ben Gordon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FEIG
{
    public class AnimatedTexture
    {
        public static List<AnimatedTexture> AnimatedTextures = new List<AnimatedTexture>();

        public enum LoopType
        {
            All,
            Horizontal,
            Vertical
        }

        private LoopType loopType;
        private Texture2D texture;
        private Point frameSize;
        private float frameRate;
        private int rows, cols;
        private Point frameIndex; // What frame is the animated texture on?
        private float frameTimer;
        private Rectangle frameRect;

        public AnimatedTexture(Texture2D texture, int rows, int cols, Point frameSize, Point startFrame, LoopType loopType, float frameRate)
        {
            this.rows = rows;
            this.cols = cols;
            this.texture = texture;
            this.frameSize = frameSize;
            this.loopType = loopType;
            this.frameRate = frameRate;

            frameTimer = frameRate;
            frameIndex = startFrame;
            frameRect = new Rectangle(startFrame.X * frameSize.X, startFrame.Y * frameSize.Y, frameSize.X, frameSize.Y);

            // Added so that we can update all animated textures all at once
            AnimatedTextures.Add(this);
        }

        public void Update(GameTime gameTime)
        {
            if (frameTimer <= 0)
            {
                AdvanceFrame();
                frameTimer = frameRate;
            }
            else
                frameTimer -= gameTime.ElapsedGameTime.Milliseconds;
        }

        private void AdvanceFrame()
        {
            switch (loopType)
            {
                case LoopType.All:
                    if (frameIndex.X < cols - 1)
                        frameIndex.X++;
                    else
                    {
                        frameIndex.X = 0;

                        if (frameIndex.Y < rows - 1)
                            frameIndex.Y++;
                        else
                            frameIndex.Y = 0;
                    }
                    break;

                case LoopType.Horizontal:
                    if (frameIndex.X < cols - 1)
                        frameIndex.X++;
                    else
                        frameIndex.X = 0;
                    break;

                case LoopType.Vertical:
                    if (frameIndex.Y < rows - 1)
                        frameIndex.Y++;
                    else
                        frameIndex.Y = 0;
                    break;
            }

            frameRect = new Rectangle(frameIndex.X * frameSize.X, frameIndex.Y * frameSize.Y, frameSize.X, frameSize.Y);
        }

        public Rectangle GetFrameRect()
        {
            return frameRect;
        }

        public Texture2D GetTexture()
        {
            return texture;
        }
    }
}
