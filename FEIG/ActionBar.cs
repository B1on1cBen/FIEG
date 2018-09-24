﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FEIG
{

    public class ActionBar
    {
        public enum BarButtons
        {
            Attack,
            Wait,
            Back
        }

        public static readonly int offset = 64;
        public static Cursor cursor;

        public BarButtons selectedButton;
        private Texture2D buttonTexture;
        private Rectangle[] buttonRects;

        private string prompt = "";

        private static SpriteFont font;

        public ActionBar(Texture2D buttonTexture, SpriteFont promptFont)
        {
            font = promptFont;

            selectedButton = BarButtons.Attack;

            this.buttonTexture = buttonTexture;

            int width = buttonTexture.Width;
            int height = buttonTexture.Height / 3;

            buttonRects = new Rectangle[3];

            buttonRects[0] = new Rectangle(0, 0, width, height);
            buttonRects[1] = new Rectangle(0, height, width, height);
            buttonRects[2] = new Rectangle(0, height * 2, width, height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
                spriteBatch.Draw(buttonTexture, new Vector2(0, Game1.windowSize.Y - offset), null, buttonRects[(int)selectedButton], null, 0, null, null, SpriteEffects.None, 0);
            else
                spriteBatch.DrawString(font, prompt, new Vector2(20, Game1.windowSize.Y - offset + 11), Color.White);
        }

        public void SetPrompt(string prompt)
        {
            this.prompt = prompt;
        }

        public void OnLeft()
        {
            Game1.navigateBackwardSound.Play();

            int index = (int)selectedButton - 1;

            if (index < 0)
                index = 2;

            SelectButton(index);
        }

        public void OnRight()
        {
            Game1.navigateForwardSound.Play();

            int index = (int)selectedButton + 1;

            if (index > 2)
                index = 0;

            SelectButton(index);
        }

        public void OnConfirm()
        {
            switch (selectedButton)
            {
                case BarButtons.Attack:
                    Game1.confirmSound.Play();
                    cursor.EnterAttackMode();
                    break;

                case BarButtons.Wait:
                    Game1.confirmSound.Play();
                    Cursor.selectedUnit.active = false;
                    cursor.FinishSelection();
                    break;

                case BarButtons.Back:
                    Game1.backSound.Play();
                    cursor.OnBack();
                    break;
            }
        }

        public void SelectButton(int index)
        {
            switch (index)
            {
                case 0:
                    selectedButton = BarButtons.Attack;
                    break;

                case 1:
                    selectedButton = BarButtons.Wait;
                    break;

                case 2:
                    selectedButton = BarButtons.Back;
                    break;
            }
        }

        public void SelectButton(BarButtons button)
        {
            selectedButton = button;
        }

        public bool Active { get; set; } = false;
    }
}