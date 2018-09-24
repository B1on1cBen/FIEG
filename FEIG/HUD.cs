// Written by Ben Gordon

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEIG
{
    public class HUD
    {
        public static int offset = 128;

        Texture2D hudBack;
        Texture2D icons;
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
                if (Cursor.hoveredUnit.team == Team.Red) x = hudBack.Width / 2;

                spriteBatch.Draw(hudBack, Vector2.Zero, null, new Rectangle(x, 0, hudBack.Width, 128), Vector2.Zero, 0, null, Color.White, SpriteEffects.None, 0);

                // Draw unit stuff in the HUD
                Unit unit = Cursor.hoveredUnit;

                // Portrait
                spriteBatch.Draw(unit.portrait, new Vector2(16, 15), null, unit.portaitRect, null, 0, null, null, SpriteEffects.None, 0);

                // Weapon Type Icon
                spriteBatch.Draw(icons, new Vector2(18, 90), null, GetIconRect(unit.weapon.iconIndex), null, 0, null, null, SpriteEffects.None, 0);

                // Move Icon
                spriteBatch.Draw(icons, new Vector2(92, 91), null, GetIconRect((int)unit.moveType, 0), null, 0, null, null, SpriteEffects.None, 0);

                // Name
                spriteBatch.DrawString(defaultFont, unit.name, new Vector2(126, 6), Color.White);

                // Stats
                spriteBatch.DrawString(defaultFont, (unit.weapon.might + unit.stats.ATK).ToString(), new Vector2(216, 32), Color.White);
                spriteBatch.DrawString(defaultFont, unit.stats.SPD.ToString(), new Vector2(216, 52), Color.White);
                spriteBatch.DrawString(defaultFont, unit.stats.DEF.ToString(), new Vector2(216, 72), Color.White);
                spriteBatch.DrawString(defaultFont, unit.stats.RES.ToString(), new Vector2(216, 92), Color.White);

                // HP
                string hpString = unit.CurrentHP.ToString() + "/" + unit.stats.HP.ToString();
                Vector2 hpSize = hpFont.MeasureString(hpString);
                spriteBatch.DrawString(hpFont, hpString, new Vector2(Game1.windowSize.X - hpSize.X - 11, 16), Color.White);

                // Weapon
                string weaponString = unit.weapon.name;
                Vector2 weaponStringSize = defaultFont.MeasureString(weaponString);
                spriteBatch.DrawString(defaultFont, weaponString, new Vector2(Game1.windowSize.X - weaponStringSize.X - 16, 50), Color.White);
            }
            else
            {
                // Draw the HUD with nothing in it.
                spriteBatch.Draw(hudBack, Vector2.Zero, null, new Rectangle(x, 128, hudBack.Width, 128), Vector2.Zero, 0, null, Color.White, SpriteEffects.None, 0);
            }

            // Draw Action Bar back
            spriteBatch.Draw(hudBack, new Vector2(0, Game1.windowSize.Y - ActionBar.offset), null, new Rectangle(x, 256, hudBack.Width / 2, 64), null, 0, null, null, SpriteEffects.None, 0);

            // Draw the name of the tile that the cursor is looking at
            string typeString = Tile.TileTypeStrings[(int)Cursor.hoveredTile.type];
            Vector2 stringSize = defaultFont.MeasureString(typeString);
            Vector2 position = new Vector2(Game1.windowSize.X - stringSize.X - 17, offset - 43);

            spriteBatch.DrawString(defaultFont, typeString, position, Color.White);
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
