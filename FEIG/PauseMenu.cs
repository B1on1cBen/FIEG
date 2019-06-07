//--Shawn Murdoch--//
//--I used the ActionBar script that Ben wrote and worked it so that it would work for the pause menu--//

// Ben Gordon
// And it worked great, but I tweaked it a little bit :)

using FEIG.Input;
using FEIG.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace FEIG.UI
{
#pragma warning disable CS0618
    public class PauseMenu
    {
        public enum MenuOptions
        {
            EndTurn,
            Resume,
            Quit
        }

        public static Cursor cursor;
        public bool Active { get; set; }

        private MenuOptions selectedOption;
        private readonly Rectangle[] optionRects;
        private Texture2D menuTexture;

        public PauseMenu(Texture2D menuTexture)
        {
            selectedOption = MenuOptions.EndTurn;
            this.menuTexture = menuTexture;

            int width = menuTexture.Width / 3;
            int height = menuTexture.Height;

            optionRects = new Rectangle[3];
            optionRects[0] = new Rectangle(0, 0, width, height);
            optionRects[1] = new Rectangle(width, 0, width, height);
            optionRects[2] = new Rectangle(width * 2, 0, width, height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
                spriteBatch.Draw(menuTexture, new Vector2(Game1.windowSize.X / 2 - menuTexture.Width / 6, Game1.windowSize.Y / 2 - menuTexture.Height / 2), null, optionRects[(int)selectedOption], null, 0, null, null, SpriteEffects.None, 0);
        }

        public void OnDown()
        {
            Game1.navigateForwardSound.Play();
            SelectButton(((int)selectedOption + 1) % 3);
        }

        public void OnUp()
        {
            Game1.navigateBackwardSound.Play();

            int index = (int)selectedOption - 1;
            if (index < 0) index = 2;

            SelectButton(index);
        }

        public void OnConfirm()
        {
            switch (selectedOption)
            {
                case MenuOptions.EndTurn:
                    Game1.confirmSound.Play();
                    Active = false;
                    Game1.gameState = Game1.GameStates.EnemyTurn;

                    foreach (Unit unit in Game1.units)
                    {
                        if (unit.team == Team.Blue)
                            unit.active = false;
                    }

                    cursor.FinishSelection();
                    break;

                case MenuOptions.Resume:
                    Game1.backSound.Play();
                    Active = false;
                    Game1.gameState = Game1.GameStates.PlayerTurn;
                    break;

                case MenuOptions.Quit:
                    Game1.confirmSound.Play();
                    MediaPlayer.Stop();
                    Game1.gameState = Game1.GameStates.Quit;
                    break;
            }
        }

        public void SelectButton(int index)
        {
            selectedOption = (MenuOptions) index;
        }
    }
}