// Written by Ben Gordon

using FEIG.Map;
using FEIG.UI;
using FEIG.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEIG.Input
{
#pragma warning disable CS0618
    public class HUD
    {
        public static int offset = 128;

        Texture2D hudBack;
        readonly Texture2D icons;
        SpriteFont defaultFont;
        SpriteFont hpFont;

        public HUD(Texture2D hudBack, Texture2D icons, SpriteFont defaultFont, SpriteFont hpFont)
        {
            this.hudBack = hudBack;
            this.icons = icons;
            this.defaultFont = defaultFont;
            this.hpFont = hpFont;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            int x = 0;

            // Draw the HUD with Unit display visible, if a unit is highlighted
            if (Cursor.CursorOverUnit)
            {
                if (Cursor.hoveredUnit.team == Team.Red)
                    x = hudBack.Width / 2;

                spriteBatch.Draw(hudBack, Vector2.Zero, null, new Rectangle(x, 0, hudBack.Width, 128), Vector2.Zero, 0, Game1.WindowScale, Color.White, SpriteEffects.None, 0);

                Unit unit = Cursor.hoveredUnit;
                // Portrait
                spriteBatch.Draw(unit.portraitTexture.texture, new Vector2(16 * Game1.WindowScale.X, 15 * Game1.WindowScale.Y), null, unit.portraitTexture.rect, null, 0, Game1.WindowScale, null, SpriteEffects.None, 0);
                // Weapon Type Icon
                spriteBatch.Draw(icons, new Vector2(18 * Game1.WindowScale.X, 90 * Game1.WindowScale.Y), null, GetIconRect(unit.weapon.iconIndex), null, 0, Game1.WindowScale, null, SpriteEffects.None, 0);
                // Move Icon
                spriteBatch.Draw(icons, new Vector2(92 * Game1.WindowScale.X, 91 * Game1.WindowScale.Y), null, GetIconRect((int)unit.moveType, 0), null, 0, Game1.WindowScale, null, SpriteEffects.None, 0);
                // Name
                spriteBatch.DrawString(defaultFont, unit.name, new Vector2(126 * Game1.WindowScale.X, 6 * Game1.WindowScale.Y), Color.White);
                // Stats
                spriteBatch.DrawString(defaultFont, (unit.weapon.might + unit.stats.ATK).ToString(), new Vector2(216 * Game1.WindowScale.X, 32 * Game1.WindowScale.Y), Color.White);
                spriteBatch.DrawString(defaultFont, unit.stats.SPD.ToString(), new Vector2(216 * Game1.WindowScale.X, 52 * Game1.WindowScale.Y), Color.White);
                spriteBatch.DrawString(defaultFont, unit.stats.DEF.ToString(), new Vector2(216 * Game1.WindowScale.X, 72 * Game1.WindowScale.Y), Color.White);
                spriteBatch.DrawString(defaultFont, unit.stats.RES.ToString(), new Vector2(216 * Game1.WindowScale.X, 92 * Game1.WindowScale.Y), Color.White);
                // HP
                string hpString = unit.CurrentHP.ToString() + "/" + unit.stats.HP.ToString();
                Vector2 hpSize = hpFont.MeasureString(hpString);
                spriteBatch.DrawString(hpFont, hpString, new Vector2(Game1.windowSize.X - hpSize.X - 11 * Game1.WindowScale.X, 16 * Game1.WindowScale.Y), Color.White);
                // Weapon
                string weaponString = unit.weapon.name;
                Vector2 weaponStringSize = defaultFont.MeasureString(weaponString);
                spriteBatch.DrawString(defaultFont, weaponString, new Vector2(Game1.windowSize.X - weaponStringSize.X - 16 * Game1.WindowScale.X, 50 * Game1.WindowScale.Y), Color.White);
            }
            else
            {
                // Draw the HUD with nothing in it.
                spriteBatch.Draw(hudBack, Vector2.Zero, null, new Rectangle(x, 128, hudBack.Width, 128), Vector2.Zero, 0, Game1.WindowScale, Color.White, SpriteEffects.None, 0);
            }

            // Draw Action Bar back
            spriteBatch.Draw(hudBack, new Vector2(0, Game1.windowSize.Y - ActionBar.offset * Game1.WindowScale.Y), null, new Rectangle(x, 256, hudBack.Width / 2, 64), null, 0, Game1.WindowScale, null, SpriteEffects.None, 0);

            // Draw the name of the tile that the cursor is looking at
            string tileTypeString = Tile.TileTypeStrings[(int)Cursor.hoveredTile.type];
            Vector2 stringSize = defaultFont.MeasureString(tileTypeString);
            Vector2 position = new Vector2(Game1.windowSize.X - stringSize.X - 17 * Game1.WindowScale.X, offset * Game1.WindowScale.Y - 43 * Game1.WindowScale.Y);

            spriteBatch.DrawString(defaultFont, tileTypeString, position, Color.White);
        }

        Rectangle GetIconRect(Point point)
        {
            return GetIconRect(point.X, point.Y);
        }

        Rectangle GetIconRect(int x, int y)
        {
            return new Rectangle(x * 22, y * 22, 22, 22);
        }
    }
}
