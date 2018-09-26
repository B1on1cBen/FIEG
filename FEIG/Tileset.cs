using Microsoft.Xna.Framework;

namespace FEIG
{
    public class Tileset
    {
        public SpriteSheet spriteSheet;

        public Tileset() { }

        public Tileset(SpriteSheet spriteSheet)
        {
            this.spriteSheet = spriteSheet;
        }

        public Rectangle[] GetRects()
        {
            Rectangle[] rectangles = new Rectangle[spriteSheet.sheetDimensions.X * spriteSheet.sheetDimensions.Y];
            for (int i = 0; i < rectangles.Length; i++)
            {
                rectangles[i] = new Rectangle(
                    (i % spriteSheet.sheetDimensions.X) * spriteSheet.frameSize.X,
                    (i % spriteSheet.sheetDimensions.Y) * spriteSheet.frameSize.Y,
                    spriteSheet.frameSize.X,
                    spriteSheet.frameSize.Y
                );
            }
            return rectangles;
        }
    }

    public class AnimatedTileSet : Tileset
    {
        public AnimatedTexture animatedTexture;

        public AnimatedTileSet(AnimatedTexture animatedTexture)
        {
            this.animatedTexture = animatedTexture;
            spriteSheet = animatedTexture.spriteSheet;
        }
    }
}
