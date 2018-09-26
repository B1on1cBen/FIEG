﻿// Written by Ben Gordon and Shawn Murdoch

using FEIG.CursorHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using static FEIG.CursorHelper.CursorContext;

namespace FEIG
{
    public class Cursor
    {
        private static Point position; // Where the cursor is on the grid
        public static Tile hoveredTile; // What tile is the cursor hovering over?
        public static Unit hoveredUnit; // What unit is the cursor hovering over?
        public static Unit selectedUnit; // What unit is the cursor currently selecting?

        public static ActionBar actionBar; // A reference to couple the cursor to the action bar
        public static PauseMenu pauseMenu;

        // Allows disabling the cursor
        public bool active = true;

        private AnimatedTexture texture; // Make the cursor grow and shrink because it looks really really nice
        private Texture2D moveArrowTexture;

        // Used to undo movement for units
        private Point selectionPreviousPos;

        private static KeyboardState prevKeyboardState;
        private static GamePadState prevGamePadState;

        // Allows you to hold the cursor and move continuously in a direction
        private static readonly float holdDelay = 200; // How long to hold before turbo activates
        private static readonly float turboDelay = 50; // How often turbo repeats

        // Timers that use the delays above as values. (Allows timers to reset)
        private static float holdTimer;
        private static float turboTimer;

        // The movement key currently being held. (Can only be one)
        private static Keys heldKey;
        private static Keys prevHeldKey;

        private static Buttons heldButton;
        private static Buttons prevheldButton;

        private static bool keyHeldThisFrame = false;

        // This context moves the cursor along the grid.
        private static readonly CursorContext mapContext = new CursorContext(new Dictionary<CursorInput, CursorAction>
        {
            {Game1.Input["Up"],    new CursorAction(()=>MoveOnGrid(new Point( 0, -1)), true)},
            {Game1.Input["Down"],  new CursorAction(()=>MoveOnGrid(new Point( 0,  1)), true)},
            {Game1.Input["Left"],  new CursorAction(()=>MoveOnGrid(new Point(-1,  0)), true)},
            {Game1.Input["Right"], new CursorAction(()=>MoveOnGrid(new Point( 1,  0)), true)},
            {Game1.Input["ToggleDangerZone"], new CursorAction(()=>Game1.ToggleDangerZone(), false)}
        });

        // This is sort of like a cursor context, but it's really only a flag. 
        // The cursor still moves on the map, so we keep the mapContext, rather 
        // than making a new context for each flag (Messy!)
        private enum MapCursorMode
        {
            MoveCursor,
            MoveUnit,
            AttackUnit
        }

        private static MapCursorMode mapCursorMode;

        // This context will move the cursor along the action bar
        private static readonly CursorContext actionBarContext = new CursorContext(new Dictionary<CursorInput, CursorAction>
        {
            {Game1.Input["Left"], new CursorAction(()=>actionBar.OnLeft(),  true)},
            {Game1.Input["Right"], new CursorAction(()=>actionBar.OnRight(), true)},
        });

        private static readonly CursorContext pauseMenuContext = new CursorContext(new Dictionary<CursorInput, CursorAction>
        {
            {Game1.Input["Up"], new CursorAction(()=>pauseMenu.OnUp(),  true)},
            {Game1.Input["Down"], new CursorAction(()=>pauseMenu.OnDown(), true)},
        });

        // The context the cursor is currently using
        private static CursorContext currentContext;

        // Where the cursor can go when in "move unit" mode
        public static List<Point> validMoveTiles = new List<Point>();
        // Where the cursor can go when in "attack" mode
        public static List<Point> validAttackTiles = new List<Point>();

        public Cursor(Point startingPosition, Texture2D texture, Texture2D moveArrowTexture)
        {
            currentContext = mapContext;
            mapCursorMode = MapCursorMode.MoveCursor;

            this.texture = new AnimatedTexture(new SpriteSheet(texture, new Point(1, 2), Palette.tileSize), 500);
            this.moveArrowTexture = moveArrowTexture;

            position = startingPosition;
            hoveredUnit = Game1.GetUnit(startingPosition);
            hoveredTile = Level.GetTile(startingPosition);

            prevKeyboardState = Keyboard.GetState();
            prevGamePadState = GamePad.GetState(0);

            holdTimer = holdDelay;
            turboTimer = turboDelay;
        }

        public void Update(GameTime gameTime)
        {
            keyHeldThisFrame = false;

            if (active)
            {
                if (Game1.Input["Confirm"].IsPressed())
                    OnConfirm();

                if (Game1.Input["Back"].IsPressed())
                    OnBack();

                // This will handle pressed keys for any cursor context
                foreach (KeyValuePair<CursorInput, CursorAction> key in currentContext.keys)
                {
                    // Note: it is possible to press a key and hold a key at the same time
                    // with this setup. This is intentional. It doesn't feel right otherwise.

                    // PRESSING KEY
                    if (key.Key.IsPressed())
                        key.Value.action();

                    // HOLDING KEY
                    if (key.Value.useTurbo && key.Key.IsHeld())
                    {
                        // Reset timers if new key is held
                        if (key.Key.lastHoldWasKey)
                        {
                            if (prevHeldKey != heldKey)
                            {
                                holdTimer = holdDelay;
                                turboTimer = turboDelay;
                            }
                        }
                        else
                        {
                            if (prevheldButton != heldButton)
                            {
                                holdTimer = holdDelay;
                                turboTimer = turboDelay;
                            }
                        }

                        if (holdTimer <= 0)
                        {
                            if (turboTimer <= 0)
                            {
                                key.Value.action();
                                turboTimer = turboDelay;
                            }
                            else
                                turboTimer -= gameTime.ElapsedGameTime.Milliseconds;
                        }
                        else
                            holdTimer -= gameTime.ElapsedGameTime.Milliseconds;
                    }
                }

                prevKeyboardState = Keyboard.GetState();
                prevGamePadState = GamePad.GetState(0);
                prevHeldKey = heldKey;
                prevheldButton = heldButton;
                heldKey = Keys.None;
                heldButton = Buttons.BigButton;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentContext == mapContext)
            {
                Vector2 drawPos = new Vector2(position.X * Palette.tileSize.X, position.Y * Palette.tileSize.Y + HUD.offset);

                if (InMoveUnitMode)
                    spriteBatch.Draw(moveArrowTexture, drawPos, null, new Rectangle(64, 0, 64, 64), null, 0, null, null, SpriteEffects.None, 0);
                else if (mapCursorMode == MapCursorMode.AttackUnit)
                    spriteBatch.Draw(moveArrowTexture, drawPos, null, new Rectangle(64, 64, 64, 64), null, 0, null, null, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(texture.GetTexture(), drawPos, null, texture.GetFrameRect(), null, 0, null, null, SpriteEffects.None, 0);
            }
        }

        public void OnConfirm()
        {
            if (currentContext == mapContext)
            {
                switch (mapCursorMode)
                {
                    case MapCursorMode.MoveCursor:
                        OnSelectUnit();
                        if (selectedUnit != null && selectedUnit.team == Team.Blue)
                        {
                            mapCursorMode = MapCursorMode.MoveUnit;
                            Game1.menuSound.Play();
                        }
                        break;

                    case MapCursorMode.MoveUnit:
                        if (validMoveTiles.Contains(position))
                        {
                            currentContext = actionBarContext;
                            actionBar.Active = true;
                            Game1.confirmSound.Play();
                        }
                        else
                            Game1.deniedSound.Play();
                        break;

                    case MapCursorMode.AttackUnit:
                        if (selectedUnit.validAttackPoints.Contains(position))
                        {
                            Unit unit = Game1.GetUnit(position);
                            if (unit != null && unit.team == Team.Red)
                            {
                                selectedUnit.Attack(unit);
                                selectedUnit.active = false;
                                FinishSelection();
                                Game1.UpdateDangerZone();
                                Game1.confirmSound.Play();
                            }
                            else
                                Game1.deniedSound.Play();
                        }
                        else
                            Game1.deniedSound.Play();
                        break;
                }
            }
            else if (currentContext == actionBarContext)
            {
                actionBar.OnConfirm();
            }
            else if (currentContext == pauseMenuContext)
            {
                pauseMenu.OnConfirm();
                currentContext = mapContext;
            }
        }

        public void OnBack()
        {
            if (currentContext == mapContext)
            {
                switch (mapCursorMode)
                {
                    case MapCursorMode.MoveCursor:
                        currentContext = pauseMenuContext;
                        pauseMenu.Active = true;
                        Game1.menuSound.Play();
                        break;

                    case MapCursorMode.MoveUnit:
                        validMoveTiles.Clear();
                        selectedUnit.validAttackPoints.Clear();
                        selectedUnit.Position = selectionPreviousPos;
                        selectedUnit = null;
                        mapCursorMode = MapCursorMode.MoveCursor;
                        Game1.backSound.Play();
                        break;

                    case MapCursorMode.AttackUnit:
                        selectedUnit.validAttackPoints.Clear();
                        currentContext = actionBarContext;
                        actionBar.Active = true;
                        Game1.backSound.Play();
                        break;
                }
            }
            else if (currentContext == actionBarContext)
            {
                selectedUnit.Position = selectionPreviousPos;
                Position = selectedUnit.Position;
                currentContext = mapContext;
                actionBar.Active = false;
                mapCursorMode = MapCursorMode.MoveUnit;
                Game1.backSound.Play();
            }
            else if (currentContext == pauseMenuContext)
            {
                currentContext = mapContext;
                pauseMenu.Active = false;
                mapCursorMode = MapCursorMode.MoveCursor;
                Game1.backSound.Play();
            }
        }

        public static bool KeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) && !prevKeyboardState.IsKeyDown(key);
        }

        public static bool KeyHeld(Keys key)
        {
            bool held = Keyboard.GetState().IsKeyDown(key) && prevKeyboardState.IsKeyDown(key);

            if (held && !keyHeldThisFrame)
            {
                keyHeldThisFrame = true;
                heldKey = key;
                return true;
            }

            return false;
        }

        public static bool ButtonPressed(Buttons button)
        {
            return GamePad.GetState(0).IsButtonDown(button) && !prevGamePadState.IsButtonDown(button);
        }

        public static bool ButtonHeld(Buttons button)
        {
            bool held = GamePad.GetState(0).IsButtonDown(button) && prevGamePadState.IsButtonDown(button); ;

            if (held && !keyHeldThisFrame)
            {
                keyHeldThisFrame = true;
                heldButton = button;
                return true;
            }

            return false;
        }

        public void SwitchContexts(CursorContext newContext)
        {
            currentContext = newContext;
        }

        public void FinishSelection()
        {
            if (selectedUnit != null)
                selectedUnit.validAttackPoints.Clear();
            selectedUnit = null;
            selectionPreviousPos = Position;
            currentContext = mapContext;
            actionBar.Active = false;
            mapCursorMode = MapCursorMode.MoveCursor;
            validMoveTiles.Clear();
            Game1.CheckForEndTurn();
        }

        public void EnterAttackMode()
        {
            mapCursorMode = MapCursorMode.AttackUnit;
            actionBar.Active = false;
            currentContext = mapContext;
            selectedUnit.UpdateAttackTiles();
        }

        public static Point Position
        {
            get { return position; }

            set
            {
                // Make sure cursor stays in bounds
                if (value.X >= 0 && value.X < Level.levelWidth && value.Y >= 0 && value.Y < Level.levelHeight)
                {
                    position = value;
                    Game1.moveCursorSound.Play();
                }
            }
        }

        public static void MoveOnGrid(Point point)
        {
            MoveOnGrid(point.X, point.Y);
        }

        public static void MoveOnGrid(int xAmount, int yAmount)
        {
            Position = new Point(position.X + xAmount, position.Y + yAmount);
            hoveredTile = Level.grid[position.X, position.Y];
            hoveredUnit = Game1.GetUnit(position);

            // Update danger zones to reflect where selected unit is being moved
            if (mapCursorMode == MapCursorMode.MoveUnit)
            {
                selectedUnit.Position = position;
                Game1.UpdateDangerZone();
            }
        }

        public static bool CursorOverUnit
        {
            get
            {
                if (hoveredUnit == null)
                    return false;

                if (!hoveredUnit.alive)
                    return false;

                return true;
            }
        }

        private void OnSelectUnit()
        {
            if (CursorOverUnit && hoveredUnit.active && mapCursorMode == MapCursorMode.MoveCursor)
            {
                if (hoveredUnit.team == Team.Blue)
                {
                    selectedUnit = hoveredUnit;
                    selectionPreviousPos = hoveredUnit.Position;

                    validMoveTiles.Clear();

                    // Register tiles that are valid for unit to move to
                    for (int y = 0; y < Level.grid.GetLength(1); y++)
                    {
                        for (int x = 0; x < Level.grid.GetLength(0); x++)
                        {
                            if (selectedUnit.ValidPath(selectionPreviousPos, new Point(x, y)))
                                validMoveTiles.Add(new Point(x, y));
                        }
                    }
                }
                else
                {
                    Game1.confirmSound.Play();

                    // Toggle selection to show danger zone
                    hoveredUnit.selected = !hoveredUnit.selected;
                    hoveredUnit.UpdateDangerZone();

                    // Reset global danger zone so that the next time we toggle it, everything will turn on
                    Game1.globalDangerZoneState = false;
                }
            }
            else
                Game1.deniedSound.Play();
        }

        public bool InMoveUnitMode
        {
            get
            {
                return currentContext == mapContext && mapCursorMode == MapCursorMode.MoveUnit;
            }
        }

        public bool InAttackMode
        {
            get
            {
                return currentContext == mapContext && mapCursorMode == MapCursorMode.AttackUnit;
            }
        }
    }
}
