using Microsoft.Xna.Framework.Input;

namespace FEIG.Input
{
    public class CursorInput
    {
        Keys[] keys;
        Buttons[] buttons;
        public bool lastPressWasKey = false;
        public bool lastHoldWasKey = false;

        public CursorInput(Keys[] keys, Buttons[] buttons)
        {
            this.keys = keys;
            this.buttons = buttons;
        }

        public bool IsPressed(Cursor cursor)
        {
            foreach (Keys key in keys)
            {
                if (cursor.KeyPressed(key))
                {
                    lastPressWasKey = true;
                    return true;
                }
            }

            foreach (Buttons button in buttons)
            {
                if (cursor.ButtonPressed(button))
                {
                    lastPressWasKey = false;
                    return true;
                }
            }

            return false;
        }

        public bool IsHeld(Cursor cursor)
        {
            foreach (Keys key in keys)
            {
                if (cursor.KeyHeld(key))
                {
                    lastHoldWasKey = true;
                    return true;
                }
            }

            foreach (Buttons button in buttons)
            {
                if (cursor.ButtonHeld(button))
                {
                    lastHoldWasKey = false;
                    return true;
                }
            }

            return false;
        }
    }
}
