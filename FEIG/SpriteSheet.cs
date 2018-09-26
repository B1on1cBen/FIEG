// Written By Ben Gordon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEIG.Graphics
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

        public Rectangle GetRect(Point frame)
        {
            return new Rectangle(
                sheetDimensions.X * frame.X,
                sheetDimensions.Y * frame.Y,
                frameSize.X,
                frameSize.Y
            );
        }

        public int Count
        {
            get { return sheetDimensions.X * sheetDimensions.Y; }
        }
    }
}
