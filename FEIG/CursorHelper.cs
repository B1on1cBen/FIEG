using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace FEIG.UI
{
    // Tells the cursor what it's function is when keys are pressed, 
    // within a certain context. I.E., Map context and action bar context
    public class CursorContext
    {
        // Wraps a System.Action with a boolean
        // to enable / disable turbo for said action
        public class CursorAction
        {
            public Action action;
            public bool useTurbo;

            public CursorAction(Action action, bool useTurbo)
            {
                this.action = action;
                this.useTurbo = useTurbo;
            }
        }

        public Dictionary<CursorInput, CursorAction> keys;

        // Stores delegate function calls for each key so that any 
        // context can call any function with any key
        public CursorContext(Dictionary<CursorInput, CursorAction> keys)
        {
            this.keys = keys;
        }
    }

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
