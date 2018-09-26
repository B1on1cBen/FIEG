using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEIG
{
    public class SpriteSheet
    {
        public Texture2D texture;
        public Point sheetDimensions;
        public Point frameSize;

        public SpriteSheet(Texture2D texture, Point sheetDimensions, Point frameSize)
        {
            this.texture = texture;
            this.sheetDimensions = sheetDimensions;
            this.frameSize = frameSize;
        }
    }
}
