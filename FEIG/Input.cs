//using FEIG.CursorHelper;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Input;
//using System.Collections.Generic;

//namespace FEIG
//{
//    public class Input
//    {
//        private static KeyboardState prevKeyboardState;
//        private static GamePadState prevGamePadState;

//        // Allows you to hold the cursor and move continuously in a direction
//        private static readonly float holdDelay = 200; // How long to hold before turbo activates
//        private static readonly float turboDelay = 50; // How often turbo repeats

//        // Timers that use the delays above as values. (Allows timers to reset)
//        private static float holdTimer;
//        private static float turboTimer;

//        // The movement key currently being held. (Can only be one)
//        private static Keys heldKey;
//        private static Keys prevHeldKey;

//        private static Buttons heldButton;
//        private static Buttons prevheldButton;

//        private static bool keyHeldThisFrame = false;

//        // Defines all of the keys used across the entire game
//        public static readonly Dictionary<string, CursorInput> inputs = new Dictionary<string, CursorInput>()
//        {
//            {"Confirm",          new CursorInput(new Keys[] {Keys.Z, Keys.Enter},             new Buttons[] {Buttons.A})},
//            {"Back",             new CursorInput(new Keys[] {Keys.X, Keys.Escape, Keys.Back}, new Buttons[] {Buttons.B})},
//            {"ToggleDangerZone", new CursorInput(new Keys[] {Keys.Space},                     new Buttons[] {Buttons.Y})},
//            {"Up",               new CursorInput(new Keys[] {Keys.Up, Keys.W},                new Buttons[] {Buttons.DPadUp})},
//            {"Down",             new CursorInput(new Keys[] {Keys.Down, Keys.S},              new Buttons[] {Buttons.DPadDown})},
//            {"Left",             new CursorInput(new Keys[] {Keys.Left, Keys.A},              new Buttons[] {Buttons.DPadLeft})},
//            {"Right",            new CursorInput(new Keys[] {Keys.Right, Keys.D},             new Buttons[] {Buttons.DPadRight})},
//        };

//        public CursorInput this[string name]
//        {
//            get
//            {
//                return inputs[name];
//            }
//        }

//        public static bool IsPressed(string inputName)
//        {
//            return inputs[inputName].IsPressed();
//        }

//        public static bool IsHeld(string inputName)
//        {
//            return inputs[inputName].IsHeld();
//        }

//        public static bool KeyPressed(Keys key)
//        {
//            return Keyboard.GetState().IsKeyDown(key) && !prevKeyboardState.IsKeyDown(key);
//        }

//        public static bool KeyHeld(Keys key)
//        {
//            bool held = Keyboard.GetState().IsKeyDown(key) && prevKeyboardState.IsKeyDown(key);

//            if (held && !keyHeldThisFrame)
//            {
//                keyHeldThisFrame = true;
//                heldKey = key;
//                return true;
//            }

//            return false;
//        }

//        public static bool ButtonPressed(Buttons button)
//        {
//            return GamePad.GetState(0).IsButtonDown(button) && !prevGamePadState.IsButtonDown(button);
//        }

//        public static bool ButtonHeld(Buttons button)
//        {
//            bool held = GamePad.GetState(0).IsButtonDown(button) && prevGamePadState.IsButtonDown(button); ;

//            if (held && !keyHeldThisFrame)
//            {
//                keyHeldThisFrame = true;
//                heldButton = button;
//                return true;
//            }

//            return false;
//        }

//        public Input()
//        {
//            prevKeyboardState = Keyboard.GetState();
//            prevGamePadState = GamePad.GetState(0);

//            holdTimer = holdDelay;
//            turboTimer = turboDelay;
//        }

//        public void EarlyUpdate()
//        {
//            keyHeldThisFrame = false;
//        }

//        public void Update(GameTime gameTime)
//        {
//            // This will handle pressed keys for any cursor context
//            foreach (KeyValuePair<CursorInput, CursorAction> key in currentContext.keys)
//            {
//                // PRESSING KEY
//                //if (key.Key.IsPressed())
//                //    key.Value.action();

//                // HOLDING KEY
//                if (key.Value.useTurbo && key.Key.IsHeld())
//                {
//                    // Reset timers if new key is held
//                    if (key.Key.lastHoldWasKey)
//                    {
//                        if (prevHeldKey != heldKey)
//                        {
//                            holdTimer = holdDelay;
//                            turboTimer = turboDelay;
//                        }
//                    }
//                    else
//                    {
//                        if (prevheldButton != heldButton)
//                        {
//                            holdTimer = holdDelay;
//                            turboTimer = turboDelay;
//                        }
//                    }

//                    if (holdTimer <= 0)
//                    {
//                        if (turboTimer <= 0)
//                        {
//                            key.Value.action();
//                            turboTimer = turboDelay;
//                        }
//                        else
//                            turboTimer -= gameTime.ElapsedGameTime.Milliseconds;
//                    }
//                    else
//                        holdTimer -= gameTime.ElapsedGameTime.Milliseconds;
//                }
//            }
//        }

//        public void ResetTurboTimer()
//        {

//        }

//        // To be called at the end of the main game loop
//        public void LateUpdate()
//        {
//            prevKeyboardState = Keyboard.GetState();
//            prevGamePadState = GamePad.GetState(0);
//            prevHeldKey = heldKey;
//            prevheldButton = heldButton;
//            heldKey = Keys.None;
//            heldButton = Buttons.BigButton;
//        }
//    }
//}
