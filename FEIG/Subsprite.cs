using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEIG
{
    // A sprite built from a region of a texture2D
    public class SubTexture
    {
        public Texture2D texture;
        public Rectangle rect;

        public SubTexture(Texture2D texture, Rectangle rect)
        {
            this.texture = texture;
            this.rect = rect;
        }
    }
}
