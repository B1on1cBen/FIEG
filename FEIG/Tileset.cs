// Written By Ben Gordon

using FEIG.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace FEIG.Map
{
    public class TileSet
    {
        public Color mapColor;
        public TileType tileType;
        public SpriteSheet spriteSheet;
        protected static Random rand;

        protected TileSet() { }

        public TileSet(Color mapColor, TileType tileType, SpriteSheet spriteSheet)
        {
            if (rand == null)
                rand = new Random();

            this.mapColor = mapColor;
            this.tileType = tileType;
            this.spriteSheet = spriteSheet;
        }

        public Tile GetTile(int x, int y)
        {
            return new Tile(x, y, spriteSheet.texture, GetRandomRect(), tileType);
        }

        protected Rectangle GetRandomRect()
        {
            return GetRects()[rand.Next(0, spriteSheet.Count - 1)];
        }

        protected Rectangle[] GetRects()
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

    public class AnimatedTileSet : TileSet
    {
        public AnimatedTexture animatedTexture;

        public AnimatedTileSet(Color mapColor, TileType tileType, AnimatedTexture animatedTexture)
        {
            if (rand == null)
                rand = new Random();

            this.mapColor = mapColor;
            this.tileType = tileType;
            this.animatedTexture = animatedTexture;
            spriteSheet = animatedTexture.SpriteSheet;
        }
    }
}
