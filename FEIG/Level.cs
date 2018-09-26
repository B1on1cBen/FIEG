// Written by Ben Gordon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEIG
{
    public class Level
    {
        // The colors associated with each type of object in the game.


        // These are used to tell where units are placed at the beginning of the game. 
        // These tiles are always plains.
        private static readonly Color blueSpawnColor = new Color(0, 0, 0);
        private static readonly Color redSpawnColor = new Color(255, 255, 255);

        public static Tile[,] grid;

        public static Point[] redSpawns;
        public static Point[] blueSpawns;

        // The size for every level
        public static readonly int levelWidth = 6;
        public static readonly int levelHeight = 8;

        //public static Palette palette;
        public static TileSet[] palette;
        public static Point tileSize = new Point(64, 64);

        public Level(Texture2D map, params TileSet[] palette)
        {
            redSpawns = new Point[4];
            blueSpawns = new Point[4];

            Level.palette = palette;
            grid = new Tile[levelWidth, levelHeight];
            ReadMapTexture(palette, map);
        }

        public void ReadMapTexture(TileSet[] palette, Texture2D map)
        {
            Color[] levelColors = new Color[levelWidth * levelHeight];
            map.GetData(levelColors);

            int blueSpawnsFound = 0;
            int redSpawnsFound = 0;

            for (int x = 0; x < levelWidth; x++)
            {
                for (int y = 0; y < levelHeight; y++)
                {
                    Color currentColor = levelColors[x + y * levelWidth];

                    if (ReadBlueSpawns(ref blueSpawnsFound, x, y, currentColor))
                        continue;

                    if (ReadRedSpawns(ref redSpawnsFound, x, y, currentColor))
                        continue;

                    ReadMapTile(x, y, currentColor);
                }
            }
        }

        bool ReadBlueSpawns(ref int blueSpawnsFound, int x, int y, Color currentColor)
        {
            if (currentColor == blueSpawnColor)
            {
                blueSpawns[blueSpawnsFound] = new Point(x, y);
                grid[x, y] = palette[0].GetTile(x * tileSize.X, y * tileSize.Y + HUD.offset);
                blueSpawnsFound++;
                return true;
            }
            return false;
        }

        bool ReadRedSpawns(ref int redSpawnsFound, int x, int y, Color currentColor)
        {
            if (currentColor == redSpawnColor)
            {
                redSpawns[redSpawnsFound] = new Point(x, y);
                grid[x, y] = palette[0].GetTile(x * tileSize.X, y * tileSize.Y + HUD.offset);
                redSpawnsFound++;
                return true;
            }
            return false;
        }

        void ReadMapTile(int x, int y, Color currentColor)
        {
            for (int i = 0; i < palette.Length; i++)
            {
                if (currentColor == palette[i].mapColor)
                {
                    grid[x, y] = palette[i].GetTile(x * tileSize.X, y * tileSize.Y + HUD.offset);
                    return;
                }
            }

            grid[x, y] = palette[0].GetTile(x * tileSize.X, y * tileSize.Y + HUD.offset);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in grid)
                spriteBatch.Draw(tile.texture, tile.position, null, tile.rect, null, 0, Vector2.One, Color.White, SpriteEffects.None, 0);
        }

        public static Tile GetTile(Point point)
        {
            if (point.X >= 0 && point.X <= levelWidth && point.Y >= 0 && point.Y <= levelHeight)
                return grid[point.X, point.Y];

            return null;
        }

        public static Tile GetTile(int x, int y)
        {
            return GetTile(new Point(x, y));
        }

        public static bool ContainsPoint(int x, int y)
        {
            if (x < 0)
                return false;

            if (x > levelWidth - 1)
                return false;

            if (y < 0)
                return false;

            if (y > levelHeight - 1)
                return false;

            return true;
        }
    }
}
