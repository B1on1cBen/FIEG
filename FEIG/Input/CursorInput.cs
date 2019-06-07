using Microsoft.Xna.Framework.Input;

namespace FEIG.Input
{
    public class CursorInput
    {
        readonly Keys[] keys;
        readonly Buttons[] buttons;
        public bool lastPressWasKey = false;
        public bool lastHoldWasKey = false;

        public CursorInput(Keys[] keys, Buttons[] buttons)
        {
            this.keys = keys;
            this.buttons = buttons;
        }

        public bool IsPressed()
        {
            foreach (Keys key in keys)
            {
                if (Cursor.KeyPressed(key))
                {
                    lastPressWasKey = true;
                    return true;
                }
            }

            foreach (Buttons button in buttons)
            {
                if (Cursor.ButtonPressed(button))
                {
                    lastPressWasKey = false;
                    return true;
                }
            }

            return false;
        }

        public bool IsHeld()
        {
            foreach (Keys key in keys)
            {
                if (Cursor.KeyHeld(key))
                {
                    lastHoldWasKey = true;
                    return true;
                }
            }

            foreach (Buttons button in buttons)
            {
                if (Cursor.ButtonHeld(button))
                {
                    lastHoldWasKey = false;
                    return true;
                }
            }

            return false;
        }
    }
}
