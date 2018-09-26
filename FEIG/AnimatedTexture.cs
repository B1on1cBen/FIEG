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
        private float frameRate;
        private Point currentFrame;
        private float frameTimer;
        private Rectangle frameRect;

        SpriteSheet spriteSheet;

        public AnimatedTexture(SpriteSheet spriteSheet, float frameRate)
        {
            Initialize(spriteSheet, new Point(0, 0), LoopType.All, frameRate);
        }

        public AnimatedTexture(SpriteSheet spriteSheet, Point startFrame, float frameRate)
        {
            Initialize(spriteSheet, startFrame, LoopType.All, frameRate);
        }

        public AnimatedTexture(SpriteSheet spriteSheet, LoopType loopType, float frameRate)
        {
            Initialize(spriteSheet, new Point(0, 0), loopType, frameRate);
        }

        public AnimatedTexture(SpriteSheet spriteSheet, Point startFrame, LoopType loopType, float frameRate)
        {
            Initialize(spriteSheet, startFrame, loopType, frameRate);
        }

        private void Initialize(SpriteSheet spriteSheet, Point startFrame, LoopType loopType, float frameRate)
        {
            this.spriteSheet = spriteSheet;
            this.loopType = loopType;
            this.frameRate = frameRate;

            frameTimer = frameRate;
            currentFrame = startFrame;
            frameRect = new Rectangle(startFrame.X * spriteSheet.frameSize.X, startFrame.Y * spriteSheet.frameSize.Y, spriteSheet.frameSize.X, spriteSheet.frameSize.Y);

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
                    if (currentFrame.X < spriteSheet.sheetDimensions.Y - 1)
                        currentFrame.X++;
                    else
                    {
                        currentFrame.X = 0;

                        if (currentFrame.Y < spriteSheet.sheetDimensions.X - 1)
                            currentFrame.Y++;
                        else
                            currentFrame.Y = 0;
                    }
                    break;

                case LoopType.Horizontal:
                    if (currentFrame.X < spriteSheet.sheetDimensions.Y - 1)
                        currentFrame.X++;
                    else
                        currentFrame.X = 0;
                    break;

                case LoopType.Vertical:
                    if (currentFrame.Y < spriteSheet.sheetDimensions.X - 1)
                        currentFrame.Y++;
                    else
                        currentFrame.Y = 0;
                    break;
            }

            frameRect = new Rectangle(currentFrame.X * spriteSheet.frameSize.X, currentFrame.Y * spriteSheet.frameSize.Y, spriteSheet.frameSize.X, spriteSheet.frameSize.Y);
        }

        public Rectangle GetFrameRect()
        {
            return frameRect;
        }

        public Texture2D GetTexture()
        {
            return spriteSheet.texture;
        }
    }
}
