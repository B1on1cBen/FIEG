// Written By Ben Gordon

using FEIG.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace FEIG.Map
{
    public class AnimatedTileSet : TileSet
    {
        public AnimatedTexture animatedTexture;

        public AnimatedTileSet(Color mapColor, TileType tileType, AnimatedTexture animatedTexture, int index)
        {
            if (rand == null)
                rand = new Random();

            this.mapColor = mapColor;
            this.tileType = tileType;
            this.animatedTexture = animatedTexture;
            spriteSheet = animatedTexture.SpriteSheet;
            this.index = index;
        }

        public override Tile GetTile(int x, int y)
        {
            return new Tile(x, y, spriteSheet.texture, GetRandomRect(), tileType, index, true);
        }
    }
}
