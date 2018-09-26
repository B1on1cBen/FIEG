// Written by Ben Gordon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FEIG
{
    public class PaletteRow
    {
        public int rowIndex;
        public int cellCount;

        public PaletteRow(int rowIndex, int cellCount)
        {
            this.rowIndex = rowIndex;
            this.cellCount = cellCount;
        }
    }

    // This class stores all of the tiles for a level palette texture in an easily accessible way.
    public class Palette
    {
        public Texture2D texture;
        public Point paletteSize;
        public static Point tileSize;

        Rectangle[] plainsTextures;
        Rectangle[] forestTextures;
        Rectangle[] mountainTextures;
        Rectangle[] waterTextures;

        static Random rand;

        public Palette(Texture2D texture,
                       Point paletteSize,
                       PaletteRow plainsPalette,
                       PaletteRow forestPalette,
                       PaletteRow mountainsPalette,
                       PaletteRow waterPalette
        )
        {
            rand = new Random();

            this.texture = texture;
            this.paletteSize = paletteSize; // How many tiles across and how many 
            tileSize = new Point(texture.Width / paletteSize.X, texture.Height / paletteSize.Y);

            plainsTextures = new Rectangle[plainsPalette.cellCount];
            forestTextures = new Rectangle[forestPalette.cellCount];
            mountainTextures = new Rectangle[mountainsPalette.cellCount];
            waterTextures = new Rectangle[waterPalette.cellCount];

            // Set up all of the rects for each type of tile
            for (int y = 0; y < paletteSize.Y; y++) // Rows second
            {
                for (int x = 0; x < paletteSize.X; x++) // Columns first
                {
                    if (y == plainsPalette.rowIndex)
                        if (x < plainsPalette.cellCount)
                            plainsTextures[x] = new Rectangle(new Point(x * tileSize.X, y * tileSize.Y), tileSize);

                    if (y == forestPalette.rowIndex)
                        if (x < forestPalette.cellCount)
                            forestTextures[x] = new Rectangle(new Point(x * tileSize.X, y * tileSize.Y), tileSize);

                    if (y == mountainsPalette.rowIndex)
                        if (x < mountainsPalette.cellCount)
                            mountainTextures[x] = new Rectangle(new Point(x * tileSize.X, y * tileSize.Y), tileSize);

                    if (y == waterPalette.rowIndex)
                        if (x < waterPalette.cellCount)
                            waterTextures[x] = new Rectangle(new Point(x * tileSize.X, y * tileSize.Y), tileSize);
                }
            }
        }

        public Rectangle GetRect(TileType type)
        {
            switch (type)
            {
                default:
                    return plainsTextures[rand.Next(0, plainsTextures.Length)];

                case TileType.Plains:
                    return plainsTextures[rand.Next(0, plainsTextures.Length)];
                case TileType.Forest:
                    return forestTextures[rand.Next(0, forestTextures.Length)];
                case TileType.Mountain:
                    return mountainTextures[rand.Next(0, mountainTextures.Length)];
                case TileType.Water:
                    return waterTextures[rand.Next(0, waterTextures.Length)];
            }
        }
    }
}
