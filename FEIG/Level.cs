// Written by Ben Gordon

using FEIG.Graphics;
using FEIG.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEIG.Map
{
#pragma warning disable CS0618
    public class Level
    {
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

        public static TileSet[] palette;
        public static Point tileSize = new Point(64, 64);

        public Level(Texture2D map, params TileSet[] palette)
        {
            redSpawns = new Point[4];
            blueSpawns = new Point[4];

            Level.palette = palette;

            for (int i = 0; i < palette.Length; i++)
            {
                palette[i].SetIndex(i);
            }

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
                grid[x, y] = palette[0].GetTile((int)(x * tileSize.X * Game1.WindowScale.X), (int)((y * tileSize.Y + HUD.offset) * Game1.WindowScale.Y));
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
                grid[x, y] = palette[0].GetTile((int)(x * tileSize.X * Game1.WindowScale.X), (int)((y * tileSize.Y + HUD.offset) * Game1.WindowScale.Y));
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
                    grid[x, y] = palette[i].GetTile((int)(x * tileSize.X * Game1.WindowScale.X), (int)((y * tileSize.Y + HUD.offset) * Game1.WindowScale.Y));
                    return;
                }
            }

            grid[x, y] = palette[0].GetTile((int)(x * tileSize.X * Game1.WindowScale.X), (int)((y * tileSize.Y + HUD.offset) * Game1.WindowScale.Y));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Tile tile in grid)
            {
                if (tile.animated)
                {
                    AnimatedTileSet animatedTileSet = (AnimatedTileSet)palette[tile.tilesetIndex];
                    AnimatedTexture animatedTexture = animatedTileSet.animatedTexture;
                    spriteBatch.Draw(animatedTexture.GetTexture(), tile.position, null, animatedTexture.GetFrameRect(), null, 0, Game1.WindowScale, null, SpriteEffects.None, 0);             
                }
                else
                {
                    spriteBatch.Draw(tile.texture, tile.position, null, tile.rect, null, 0, Game1.WindowScale, Color.White, SpriteEffects.None, 0);
                }
            }
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
