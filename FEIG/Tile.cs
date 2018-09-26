// Written by Ben Gordon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEIG.Map
{
    public enum TileType
    {
        Plains,
        Forest,
        Mountain,
        Water,
        WhiteSpawn,
        BlackSpawn
    }

    public class Tile
    {
        // These are parallel with the TileType enum,
        // and they are used in the HUD display
        // to show what type of tile the cursor is over.
        public static string[] TileTypeStrings = new string[]
        {
            "Plain",
            "Forest",
            "Mountain",
            "Ocean",
            "Plain", // Spawns are always plains, since every type of unit must be able to navigate them
            "Plain"  // ditto
        };

        public Vector2 position;
        public Texture2D texture;
        public Rectangle rect;
        public TileType type;

        public Tile(Vector2 position, Texture2D texture, Rectangle rect, TileType type)
        {
            this.position = position;
            this.texture = texture;
            this.rect = rect;
            this.type = type;
        }

        public Tile(int x, int y, Texture2D texture, Rectangle rect, TileType type)
        {
            position = new Vector2(x, y);
            this.texture = texture;
            this.rect = rect;
            this.type = type;
        }
    }
}
