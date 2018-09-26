// Written by Ben Gordon

using Microsoft.Xna.Framework;
using System;

namespace FEIG
{
    public class Palette
    {
        public static Point tileSize;

        public Tileset plainsTileset;
        public Tileset forestTileset;
        public Tileset mountainsTileset;
        public Tileset waterTileset;

        static Random rand;

        public Palette(Tileset plainsTileset, Tileset forestTileset, Tileset mountainsTileset, Tileset waterTileset)
        {
            rand = new Random();
            tileSize = plainsTileset.spriteSheet.frameSize;
            this.plainsTileset = plainsTileset;
            this.forestTileset = forestTileset;
            this.mountainsTileset = mountainsTileset;
            this.waterTileset = waterTileset;
        }

        public Rectangle GetRect(TileType type)
        {
            switch (type)
            {
                default:
                case TileType.Plains:
                    return plainsTileset.GetRects()[rand.Next(0, plainsTileset.spriteSheet.Count - 1)];

                case TileType.Forest:
                    return forestTileset.GetRects()[rand.Next(0, forestTileset.spriteSheet.Count - 1)];

                case TileType.Mountain:
                    return mountainsTileset.GetRects()[rand.Next(0, mountainsTileset.spriteSheet.Count - 1)];

                case TileType.Water:
                    return waterTileset.GetRects()[rand.Next(0, waterTileset.spriteSheet.Count - 1)];
            }
        }
    }
}
