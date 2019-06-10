// Written by Ben Gordon and Shawn Murdoch

using FEIG.Graphics;
using FEIG.Map;
using FEIG.UI;
using FEIG.Units;
using FEIG.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace FEIG.Input
{
#pragma warning disable CS0618
    public class Cursor
    {
        // This is sort of like a cursor context, but it's really only a flag. 
        // The cursor still moves on the map, so we keep the mapContext, rather 
        // than making a new context for each flag (Messy!)
        private enum MapCursorMode
        {
            MoveCursor,
            MoveUnit,
            AttackUnit
        }

        // Allows you to hold an input to continuously press it while it is held.
        private SimpleTimer holdTimer = new SimpleTimer(200); // How long to hold before turbo activates
        private SimpleTimer turboTimer = new SimpleTimer(50); // How often turbo repeats

        public static Tile hoveredTile;
        public static Unit hoveredUnit;
        public static Unit selectedUnit;

        public bool active = true;

        private static Point position;
        private static MapCursorMode mapCursorMode;
        private static ActionBar actionBar;
        private static PauseMenu pauseMenu;

        private static KeyboardState prevKeyboardState;
        private static GamePadState prevGamePadState;

        // The movement key currently being held. (Can only be one)
        private static Keys heldKey;
        private static Keys prevHeldKey;
        private static Buttons heldButton;
        private static Buttons prevheldButton;

        private static bool inputHeldThisFrame = false;

        private AnimatedTexture texture; // Make the cursor grow and shrink because it looks really really nice
        private readonly Texture2D moveArrowTexture;
        private Point selectionPreviousPos; // Used to undo movement for units

        // This context moves the cursor along the grid.
        private readonly CursorContext mapContext = new CursorContext(new Dictionary<CursorInput, CursorAction>
        {
            {Game1.Input["Up"],    new CursorAction(() => MoveOnGrid(new Point( 0, -1)), true)},
            {Game1.Input["Down"],  new CursorAction(() => MoveOnGrid(new Point( 0,  1)), true)},
            {Game1.Input["Left"],  new CursorAction(() => MoveOnGrid(new Point(-1,  0)), true)},
            {Game1.Input["Right"], new CursorAction(() => MoveOnGrid(new Point( 1,  0)), true)},
            {Game1.Input["ToggleDangerZone"], new CursorAction(() => Game1.ToggleDangerZone(), false)},
            {Game1.Input["SwitchWeapon"], new CursorAction(() => SwapHoveredUnitWeapon(), false)}
        });

        // This context will move the cursor along the action bar
        private readonly CursorContext actionBarContext = new CursorContext(new Dictionary<CursorInput, CursorAction>
        {
            {Game1.Input["Left"], new CursorAction(() => actionBar.OnLeft(),  true)},
            {Game1.Input["Right"], new CursorAction(() => actionBar.OnRight(), true)},
        });

        private readonly CursorContext pauseMenuContext = new CursorContext(new Dictionary<CursorInput, CursorAction>
        {
            {Game1.Input["Up"], new CursorAction(() => pauseMenu.OnUp(),  true)},
            {Game1.Input["Down"], new CursorAction(() => pauseMenu.OnDown(), true)},
        });

        // The context the cursor is currently using
        private CursorContext currentContext;

        // Where the cursor can go when in "move unit" mode
        public List<Point> validMoveTiles = new List<Point>();
        // Where the cursor can go when in "attack" mode
        public List<Point> validAttackTiles = new List<Point>();

        public Cursor(Point startingPosition, AnimatedTexture texture, Texture2D moveArrowTexture, ActionBar actionBar, PauseMenu pauseMenu)
        {
            currentContext = mapContext;
            mapCursorMode = MapCursorMode.MoveCursor;
            this.texture = texture;
            this.moveArrowTexture = moveArrowTexture;
            Cursor.actionBar = actionBar;
            Cursor.pauseMenu = pauseMenu;

            position = startingPosition;
            hoveredUnit = Game1.GetUnit(startingPosition);
            hoveredTile = Level.GetTile(startingPosition);

            prevKeyboardState = Keyboard.GetState();
            prevGamePadState = GamePad.GetState(0);
        }

        public void Update(GameTime gameTime)
        {
            inputHeldThisFrame = false;

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

                    if (key.Key.IsPressed())
                        key.Value.action();

                    if (key.Value.useTurbo && key.Key.IsHeld())
                        UpdateHoldingKey(key, gameTime);
                }

                prevKeyboardState = Keyboard.GetState();
                prevGamePadState = GamePad.GetState(0);
                prevHeldKey = heldKey;
                prevheldButton = heldButton;
                heldKey = Keys.None;
                heldButton = Buttons.BigButton;
            }
        }

        void UpdateHoldingKey(KeyValuePair<CursorInput, CursorAction> key, GameTime gameTime)
        {
            // Reset timers if new key is held
            if (key.Key.lastHoldWasKey)
            {
                if (prevHeldKey != heldKey)
                {
                    holdTimer.Reset();
                    turboTimer.Reset();
                }
            }
            else
            {
                if (prevheldButton != heldButton)
                {
                    holdTimer.Reset();
                    turboTimer.Reset();
                }
            }

            if (holdTimer.TimeLeft <= 0)
            {
                if (turboTimer.TimeLeft <= 0)
                {
                    key.Value.action();
                    turboTimer.Reset();
                }
                else
                    turboTimer.Tick(gameTime);
            }
            else
                holdTimer.Tick(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (currentContext == mapContext)
            {
                Vector2 drawPos = new Vector2(position.X * Level.tileSize.X, position.Y * Level.tileSize.Y + HUD.offset);

                if (InMoveUnitMode)
                    spriteBatch.Draw(moveArrowTexture, drawPos, null, new Rectangle(64, 0, 64, 64), null, 0, Game1.WindowScale, null, SpriteEffects.None, 0);
                else if (mapCursorMode == MapCursorMode.AttackUnit)
                    spriteBatch.Draw(moveArrowTexture, drawPos, null, new Rectangle(64, 64, 64, 64), null, 0, Game1.WindowScale, null, SpriteEffects.None, 0);
                else
                    spriteBatch.Draw(texture.GetTexture(), drawPos, null, texture.GetFrameRect(), scale: Game1.WindowScale);
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

                    RegisterValidMoveTiles();
                }
                else
                    ToggleDangerZone(hoveredUnit);
            }
            else
                Game1.deniedSound.Play();

            if (selectedUnit != null && selectedUnit.team == Team.Blue)
            {
                mapCursorMode = MapCursorMode.MoveUnit;
                Game1.menuSound.Play();
            }
        }

        private static void SwapHoveredUnitWeapon()
        {
            if (hoveredUnit != null && hoveredUnit.team == Team.Blue)
                hoveredUnit.SwapWeapon();
        }

        private void RegisterValidMoveTiles()
        {
            validMoveTiles.Clear();
            PathFinder pathFinder = new PathFinder();

            for (int y = 0; y < Level.grid.GetLength(1); y++)
            {
                for (int x = 0; x < Level.grid.GetLength(0); x++)
                {
                    if (pathFinder.ValidPath(selectedUnit, selectionPreviousPos, new Point(x, y)))
                        validMoveTiles.Add(new Point(x, y));
                }
            }
        }

        private void ToggleDangerZone(Unit target)
        {
            Game1.confirmSound.Play();

            // Toggle selection to show danger zone
            target.selected = !target.selected;
            target.UpdateDangerZone();

            // Reset global danger zone so that the next time we toggle it, everything will turn on
            Game1.globalDangerZoneState = false;
        }

        public void OnConfirm()
        {
            if (currentContext == mapContext)
            {
                switch (mapCursorMode)
                {
                    case MapCursorMode.MoveCursor:
                        OnSelectUnit();
                        break;

                    case MapCursorMode.MoveUnit:
                        OnConfirmMoveUnit();
                        break;

                    case MapCursorMode.AttackUnit:
                        OnConfirmAttackUnit();
                        break;
                }
            }
            else if (currentContext == actionBarContext)
                actionBar.OnConfirm();
            else if (currentContext == pauseMenuContext)
            {
                pauseMenu.OnConfirm();
                currentContext = mapContext;
            }
        }

        private void OnConfirmMoveUnit()
        {
            if (validMoveTiles.Contains(position))
            {
                currentContext = actionBarContext;
                actionBar.Active = true;
                Game1.confirmSound.Play();
            }
            else
                Game1.deniedSound.Play();
        }

        private void OnConfirmAttackUnit()
        {
            if (selectedUnit.validAttackPoints.Contains(position))
            {
                Unit unit = Game1.GetUnit(position);
                if (unit != null && unit.team == Team.Red)
                {
                    selectedUnit.StartCombat(unit);
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
        }

        public void OnBack()
        {
            if (currentContext == mapContext)
            {
                switch (mapCursorMode)
                {
                    case MapCursorMode.MoveCursor:
                        OnBackMoveCursor();
                        break;

                    case MapCursorMode.MoveUnit:
                        OnBackMoveUnit();
                        break;

                    case MapCursorMode.AttackUnit:
                        OnBackAttackUnit();
                        break;
                }
            }
            else if (currentContext == actionBarContext)
            {
                OnBackActionBar();
            }
            else if (currentContext == pauseMenuContext)
            {
                OnBackPauseMenu();
            }
        }

        private void OnBackMoveCursor()
        {
            currentContext = pauseMenuContext;
            Game1.menuSound.Play();
            pauseMenu.Active = true;
        }

        private void OnBackMoveUnit()
        {
            validMoveTiles.Clear();
            selectedUnit.validAttackPoints.Clear();
            selectedUnit.Position = selectionPreviousPos;
            selectedUnit = null;
            hoveredUnit = Game1.GetUnit(position);
            mapCursorMode = MapCursorMode.MoveCursor;

            Game1.backSound.Play();
        }

        private void OnBackAttackUnit()
        {
            selectedUnit.validAttackPoints.Clear();
            currentContext = actionBarContext;
            actionBar.Active = true;
            Game1.backSound.Play();
        }

        private void OnBackActionBar()
        {
            selectedUnit.Position = selectionPreviousPos;
            Position = selectedUnit.Position;
            currentContext = mapContext;
            actionBar.Active = false;
            mapCursorMode = MapCursorMode.MoveUnit;
            Game1.backSound.Play();
        }

        private void OnBackPauseMenu()
        {
            currentContext = mapContext;
            mapCursorMode = MapCursorMode.MoveCursor;
            Game1.backSound.Play();
            pauseMenu.Active = false;
        }

        public static bool KeyPressed(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key) && !prevKeyboardState.IsKeyDown(key);
        }

        public static bool KeyHeld(Keys key)
        {
            bool held = Keyboard.GetState().IsKeyDown(key) && prevKeyboardState.IsKeyDown(key);

            if (held && !inputHeldThisFrame)
            {
                inputHeldThisFrame = true;
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

            if (held && !inputHeldThisFrame)
            {
                inputHeldThisFrame = true;
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

        public bool InMoveUnitMode
        {
            get { return currentContext == mapContext && mapCursorMode == MapCursorMode.MoveUnit; }
        }

        public bool InAttackMode
        {
            get { return currentContext == mapContext && mapCursorMode == MapCursorMode.AttackUnit; }
        }
    }
}
