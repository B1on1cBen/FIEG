// Written by Ben Gordon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEIG
{
    public class Level
    {
        // The colors associated with each type of object in the game.
        private static readonly Color plainsColor = new Color(214, 233, 185);
        private static readonly Color forestColor = new Color(76, 138, 103);
        private static readonly Color waterColor = new Color(76, 76, 255);
        private static readonly Color mountainColor = new Color(114, 109, 108);

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

        public static Palette palette;

        public Level(Palette newPalette, Texture2D map)
        {
			redSpawns = new Point[4];
			blueSpawns = new Point[4];

			palette = newPalette;
            grid = new Tile[levelWidth, levelHeight];
            ReadMapTexture(newPalette, map);
        }

        public void ReadMapTexture(Palette palette, Texture2D map)
        {
            Color[] levelColors = new Color[levelWidth * levelHeight];
            map.GetData(levelColors);

			int yOffset = HUD.offset;

			int blueSpawnsFound = 0;
			int redSpawnsFound = 0;

			for (int x = 0; x < levelWidth; x++)
            {
                for (int y = 0; y < levelHeight; y++)
                {
                    Color currentColor = levelColors[x + y * levelWidth];

					if (currentColor == blueSpawnColor)
					{
						blueSpawns[blueSpawnsFound] = new Point(x, y);
						grid[x, y] = new Tile(x * Palette.tileSize.X, y * Palette.tileSize.Y + yOffset, palette.texture, palette.GetRect(TileType.Plains), TileType.Plains);
						blueSpawnsFound++;
					}

					if (currentColor == redSpawnColor)
					{
						redSpawns[redSpawnsFound] = new Point(x, y);
						grid[x, y] = new Tile(x * Palette.tileSize.X, y * Palette.tileSize.Y + yOffset, palette.texture, palette.GetRect(TileType.Plains), TileType.Plains);
						redSpawnsFound++;
					}

					if (currentColor == plainsColor)
                        grid[x, y] = new Tile(x * Palette.tileSize.X, y * Palette.tileSize.Y + yOffset, palette.texture, palette.GetRect(TileType.Plains), TileType.Plains);
                    if (currentColor == forestColor)
                        grid[x, y] = new Tile(x * Palette.tileSize.X, y * Palette.tileSize.Y + yOffset, palette.texture, palette.GetRect(TileType.Forest), TileType.Forest);
                    if (currentColor == mountainColor)
                        grid[x, y] = new Tile(x * Palette.tileSize.X, y * Palette.tileSize.Y + yOffset, palette.texture, palette.GetRect(TileType.Mountain), TileType.Mountain);
                    if (currentColor == waterColor)
                        grid[x, y] = new Tile(x * Palette.tileSize.X, y * Palette.tileSize.Y + yOffset, palette.texture, palette.GetRect(TileType.Water), TileType.Water);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Tile tile in grid)
                spriteBatch.Draw(palette.texture, tile.position, null, tile.rect, null, 0, Vector2.One, Color.White, SpriteEffects.None, 0);
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
