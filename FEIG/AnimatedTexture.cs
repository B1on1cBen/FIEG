// Written by Ben Gordon

using FEIG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FEIG.Graphics
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
        private Point currentFrame;
        private Rectangle frameRect;
        private SimpleTimer frameTimer;

        public SpriteSheet SpriteSheet { get; private set; }

        public AnimatedTexture(SpriteSheet spriteSheet, float frameRate, bool includeInList = true)
        {
            Initialize(spriteSheet, new Point(0, 0), LoopType.All, frameRate);
        }

        public AnimatedTexture(SpriteSheet spriteSheet, Point startFrame, float frameRate, bool includeInList = true)
        {
            Initialize(spriteSheet, startFrame, LoopType.All, frameRate);
        }

        public AnimatedTexture(SpriteSheet spriteSheet, LoopType loopType, float frameRate, bool includeInList = true)
        {
            Initialize(spriteSheet, new Point(0, 0), loopType, frameRate);
        }

        public AnimatedTexture(SpriteSheet spriteSheet, Point startFrame, LoopType loopType, float frameRate, bool includeInList = true)
        {
            Initialize(spriteSheet, startFrame, loopType, frameRate);
        }

        private void Initialize(SpriteSheet spriteSheet, Point startFrame, LoopType loopType, float frameRate, bool includeInList = true)
        {
            this.SpriteSheet = spriteSheet;
            this.loopType = loopType;
            frameTimer = new SimpleTimer(frameRate);
            currentFrame = startFrame;
            frameRect = new Rectangle(startFrame.X * spriteSheet.frameSize.X, startFrame.Y * spriteSheet.frameSize.Y, spriteSheet.frameSize.X, spriteSheet.frameSize.Y);

            if(includeInList)
                AnimatedTextures.Add(this);
        }

        public void Update(GameTime gameTime)
        {
            if (frameTimer.TimeLeft <= 0)
            {
                AdvanceFrame();
                frameTimer.Reset();
            }
            else
            {
                frameTimer.Tick(gameTime);
            }
        }

        private void AdvanceFrame()
        {
            switch (loopType)
            {
                case LoopType.All:
                    LoopAll();
                    break;

                case LoopType.Horizontal:
                    LoopHorizontal();
                    break;

                case LoopType.Vertical:
                    LoopVertical();
                    break;
            }

            frameRect = new Rectangle(currentFrame.X * SpriteSheet.frameSize.X, currentFrame.Y * SpriteSheet.frameSize.Y, SpriteSheet.frameSize.X, SpriteSheet.frameSize.Y);
        }

        private void LoopAll()
        {
            if (currentFrame.X < SpriteSheet.sheetDimensions.Y - 1)
            {
                currentFrame.X++;
            }
            else
            {
                currentFrame.X = 0;

                if (currentFrame.Y < SpriteSheet.sheetDimensions.X - 1)
                    currentFrame.Y++;
                else
                    currentFrame.Y = 0;
            }
        }

        private void LoopHorizontal()
        {
            if (currentFrame.X < SpriteSheet.sheetDimensions.Y - 1)
                currentFrame.X++;
            else
                currentFrame.X = 0;
        }

        private void LoopVertical()
        {
            if (currentFrame.Y < SpriteSheet.sheetDimensions.X - 1)
                currentFrame.Y++;
            else
                currentFrame.Y = 0;
        }

        public Rectangle GetFrameRect()
        {
            return frameRect;
        }

        public Texture2D GetTexture()
        {
            return SpriteSheet.texture;
        }
    }
}
