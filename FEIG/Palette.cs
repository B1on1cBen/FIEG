// Written by Ben Gordon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FEIG
{
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

                       int plainsRow,     // The row where plains are located within the palette.
                       int plainsCount,   // How many types of plains are there in the row?

                       int forestRow,     // The row where forests are located within the palette.
                       int forestCount,   // How many types of forests are there in the row?

                       int mountainRow,   // The row where mountains are located within the palette.
                       int mountainCount, // How many types of mountains are there in the row?

                       int waterRow,      // The row where water is located within the palette.
                       int waterCount)    // How many types of water are there in the row?
        {
            rand = new Random();

            this.texture = texture;
            this.paletteSize = paletteSize; // How many tiles across and how many 
            tileSize = new Point(texture.Width / paletteSize.X, texture.Height / paletteSize.Y);

            plainsTextures = new Rectangle[plainsCount];
            forestTextures = new Rectangle[forestCount];
            mountainTextures = new Rectangle[mountainCount];
            waterTextures = new Rectangle[waterCount];

            // Set up all of the rects for each type of tile
            for (int y = 0; y < paletteSize.Y; y++) // Rows second
            {
                for (int x = 0; x < paletteSize.X; x++) // Columns first
                {
                    if (y == plainsRow)
                        if (x < plainsCount)
                            plainsTextures[x] = new Rectangle(new Point(x * tileSize.X, y * tileSize.Y), tileSize);

                    if (y == forestRow)
                        if (x < forestCount)
                            forestTextures[x] = new Rectangle(new Point(x * tileSize.X, y * tileSize.Y), tileSize);

                    if (y == mountainRow)
                        if (x < mountainCount)
                            mountainTextures[x] = new Rectangle(new Point(x * tileSize.X, y * tileSize.Y), tileSize);

                    if (y == waterRow)
                        if (x < waterCount)
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
