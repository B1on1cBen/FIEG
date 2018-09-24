// Written by Ben Gordon and Shawn Murdoch

using FEIG.CursorHelper;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace FEIG
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D mapTexture;
        Texture2D paletteTexture;
        Texture2D cursorTexture;
        Texture2D hudTexture;
        Texture2D portraitTexture;
        Texture2D unitMapTexture;
        Texture2D iconTexture;
        Texture2D actionBarTexture;
        Texture2D titleScreen;
        Texture2D moveArrowTexture;

        public static SoundEffect moveCursorSound;
        public static SoundEffect confirmSound;
        public static SoundEffect deniedSound;
        public static SoundEffect backSound;
        public static SoundEffect navigateForwardSound;
        public static SoundEffect navigateBackwardSound;
        public static SoundEffect victorySound;
        public static SoundEffect defeatSound;
        public static SoundEffect menuSound;

        public static Song music;
        public static Texture2D moveTileTexture;
        Texture2D pauseMenuTexture;
        private int pauseTimer;
        public static AnimatedTexture moveTileAnimated;
        public static AnimatedTexture attackTileAnimated;

        //--Game States--//
        public enum GameStates
        {
            TitleScreen,
            PlayerTurn,
            EnemyTurn,
            Combat,
            LevelComplete,
            GameOver,
            Pause,
            Quit
        };

        //--Title screen is the default game state--//
        public static GameStates gameState = GameStates.TitleScreen;

        Palette palette;
        Level level;
        public static Cursor cursor;
        HUD hud;
        ActionBar actionBar;
        PauseMenu pauseMenu;

        public static Vector2 windowSize;
        public static SpriteFont font;
        public static SpriteFont hpFont;
        public static SpriteFont promptFont;
        public static SpriteFont endFont;

        public static List<Unit> units = new List<Unit>();
        public static List<int> unitDrawOrder = new List<int>();

        public static bool globalDangerZoneState = false;
        public static bool gameDone = false;

        public static readonly Dictionary<string, CursorInput> Input = new Dictionary<string, CursorInput>()
        {
            {"Confirm",          new CursorInput(new Keys[] {Keys.Z, Keys.Enter},             new Buttons[] {Buttons.A})},
            {"Back",             new CursorInput(new Keys[] {Keys.X, Keys.Escape, Keys.Back}, new Buttons[] {Buttons.B})},
            {"ToggleDangerZone", new CursorInput(new Keys[] {Keys.Space},                     new Buttons[] {Buttons.Y})},
            {"Up",               new CursorInput(new Keys[] {Keys.Up, Keys.W},                new Buttons[] {Buttons.DPadUp})},
            {"Down",             new CursorInput(new Keys[] {Keys.Down, Keys.S},              new Buttons[] {Buttons.DPadDown})},
            {"Left",             new CursorInput(new Keys[] {Keys.Left, Keys.A},              new Buttons[] {Buttons.DPadLeft})},
            {"Right",            new CursorInput(new Keys[] {Keys.Right, Keys.D},             new Buttons[] {Buttons.DPadRight})},
        };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 384;
            graphics.PreferredBackBufferHeight = 640;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Loading textures
            mapTexture = Content.Load<Texture2D>("Textures/Maps/Map1");
            paletteTexture = Content.Load<Texture2D>("Textures/TilePalettes/FIEG_Palette1");
            cursorTexture = Content.Load<Texture2D>("Textures/Cursor");
            hudTexture = Content.Load<Texture2D>("Textures/FEIG HUD 2");
            portraitTexture = Content.Load<Texture2D>("Textures/Portraits/Portraits");
            unitMapTexture = Content.Load<Texture2D>("Textures/Units/Units");
            iconTexture = Content.Load<Texture2D>("Textures/Icons");
            actionBarTexture = Content.Load<Texture2D>("Textures/ActionBar");
            moveTileTexture = Content.Load<Texture2D>("Textures/MoveTiles");
            titleScreen = Content.Load<Texture2D>("Textures/FEIG_Logo");
            moveArrowTexture = Content.Load<Texture2D>("Textures/MoveArrow");
            pauseMenuTexture = Content.Load<Texture2D>("Textures/FEIG Menu");

            moveTileAnimated = new AnimatedTexture(moveTileTexture, 3, 16, new Point(64, 64), new Point(0, 0), AnimatedTexture.LoopType.Horizontal, 100);
            attackTileAnimated = new AnimatedTexture(moveTileTexture, 3, 16, new Point(64, 64), new Point(0, 1), AnimatedTexture.LoopType.Horizontal, 100);

            // Loading Sound Effects
            deniedSound = Content.Load<SoundEffect>("SoundEffects/denied");
            confirmSound = Content.Load<SoundEffect>("SoundEffects/confirm");
            backSound = Content.Load<SoundEffect>("SoundEffects/back");
            moveCursorSound = Content.Load<SoundEffect>("SoundEffects/moveCursor");
            navigateForwardSound = Content.Load<SoundEffect>("SoundEffects/navigateForward");
            navigateBackwardSound = Content.Load<SoundEffect>("SoundEffects/navigateBackward");
            victorySound = Content.Load<SoundEffect>("SoundEffects/victory");
            defeatSound = Content.Load<SoundEffect>("SoundEffects/defeat");
            menuSound = Content.Load<SoundEffect>("SoundEffects/menu");

            // Loading Music
            music = Content.Load<Song>("Music/Winds Across the Plains");

            // Loading fonts
            font = Content.Load<SpriteFont>("Fonts/Munro");
            hpFont = Content.Load<SpriteFont>("Fonts/Lunchtime");
            promptFont = Content.Load<SpriteFont>("Fonts/MunroSmaller");
            endFont = Content.Load<SpriteFont>("Fonts/ComicSans");

            // Initializing objects
            palette = new Palette(paletteTexture, new Point(4, 4), 0, 1, 1, 3, 2, 3, 3, 4);
            level = new Level(palette, mapTexture);
            hud = new HUD(hudTexture, iconTexture, font, hpFont);
            actionBar = new ActionBar(actionBarTexture, promptFont);
            pauseMenu = new PauseMenu(pauseMenuTexture);

            // Setting up the window for the size of the level
            graphics.PreferredBackBufferWidth = Level.levelWidth * Palette.tileSize.X;
            graphics.PreferredBackBufferHeight = Level.levelHeight * Palette.tileSize.Y + HUD.offset + ActionBar.offset;
            graphics.ApplyChanges();

            windowSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);

            // Initializing Units

            #region Red Team
            // Wendy
            units.Add(new Unit(
                "Wendy",
                new Point(0, 0),
                portraitTexture,
                new Rectangle(new Point(0, 0), Unit.portraitSize),
                unitMapTexture,
                new Rectangle(new Point(0, 0), new Point(Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)),
                Team.Red,
                Weapon.Lance,
                new Stats(49, 30, 21, 41, 28),
                MoveType.Armored));
            unitDrawOrder.Add(0);

            // Tiki
            units.Add(new Unit(
                "Tiki",
                new Point(0, 1),
                portraitTexture,
                new Rectangle(new Point(98, 0), Unit.portraitSize),
                unitMapTexture,
                new Rectangle(new Point(64, 0), new Point(Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)),
                Team.Red,
                Weapon.RedDragon,
                new Stats(40, 38, 20, 35, 24),
                MoveType.Infantry));
            unitDrawOrder.Add(1);

            // Titania
            units.Add(new Unit(
                "Titania",
                new Point(0, 4),
                portraitTexture,
                new Rectangle(new Point(196, 0), Unit.portraitSize),
                unitMapTexture,
                new Rectangle(new Point(128, 0), new Point(Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)),
                Team.Red,
                Weapon.Axe,
                new Stats(37, 28, 37, 22, 30),
                MoveType.Cavalry));
            unitDrawOrder.Add(2);

            // Mae
            units.Add(new Unit(
                "Mae",
                new Point(0, 3),
                portraitTexture,
                new Rectangle(new Point(294, 0), Unit.portraitSize),
                unitMapTexture,
                new Rectangle(new Point(192, 0), new Point(Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)),
                Team.Red,
                Weapon.BlueTome,
                new Stats(35, 39, 31, 12, 30),
                MoveType.Infantry));
            unitDrawOrder.Add(3);
            #endregion Red Team

            #region Blue Team
            // Felicia
            units.Add(new Unit(
                "Felicia",
                new Point(2, 4),
                portraitTexture,
                new Rectangle(new Point(0, 98), Unit.portraitSize),
                unitMapTexture,
                new Rectangle(new Point(0, 128), new Point(Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)),
                Team.Blue,
                new Weapon("Plate", 14, 2, WeaponColor.Colorless, DamageType.Def, new Point(1, 4)),
                new Stats(34, 26, 37, 15, 35),
                MoveType.Infantry));
            unitDrawOrder.Add(4);

            // Hector
            units.Add(new Unit(
                "Hector",
                new Point(5, 7),
                portraitTexture,
                new Rectangle(new Point(294, 98), Unit.portraitSize),
                unitMapTexture,
                new Rectangle(new Point(192, 128), new Point(Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)),
                Team.Blue,
                new Weapon("Armads", 16, 1, WeaponColor.Green, DamageType.Def, new Point(0, 2)),
                new Stats(52, 39, 24, 34, 19),
                MoveType.Armored));
            unitDrawOrder.Add(5);

            // Marth
            units.Add(new Unit(
                "Marth",
                new Point(1, 7),
                portraitTexture,
                new Rectangle(new Point(98, 98), Unit.portraitSize),
                unitMapTexture,
                new Rectangle(new Point(64, 128), new Point(Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)),
                Team.Blue,
                new Weapon("Falchion", 16, 1, WeaponColor.Red, DamageType.Def, new Point(0, 1)),
                new Stats(41, 31, 37, 29, 20),
                MoveType.Infantry));
            unitDrawOrder.Add(6);

            // Tana
            units.Add(new Unit(
                "Tana",
                new Point(4, 7),
                portraitTexture,
                new Rectangle(new Point(196, 98), Unit.portraitSize),
                unitMapTexture,
                new Rectangle(new Point(128, 128), new Point(Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)),
                Team.Blue,
                Weapon.Lance,
                new Stats(36, 37, 36, 25, 22),
                MoveType.Flier));
            unitDrawOrder.Add(7);
            #endregion Blue Team

            MoveUnitsToSpawns();

            // Initializing cursor last            
            cursor = new Cursor(new Point(2, 7), cursorTexture, moveArrowTexture);

            // Coupling some things because I didn't organize things well enough here.
            ActionBar.cursor = cursor;
            Cursor.actionBar = actionBar;
            Cursor.pauseMenu = pauseMenu;
            PauseMenu.cursor = cursor;

            actionBar.SetPrompt("Z/Enter - Confirm, X/Backspace - Back,\n" +
                                "Esc - Menu, Arrows - Navigate");
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                MediaPlayer.Stop();
                Exit();
            }

            // Update animated textures so that they actually animate!
            foreach (AnimatedTexture animTexture in AnimatedTexture.AnimatedTextures)
                animTexture.Update(gameTime);

            switch (gameState)
            {
                case GameStates.TitleScreen:
                    if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) || (Keyboard.GetState().IsKeyDown(Keys.Enter)))
                    {
                        MediaPlayer.IsRepeating = true;
                        MediaPlayer.Volume = 0.5f;
                        MediaPlayer.Play(music);
                        gameState = GameStates.PlayerTurn;
                        Unit.enemyAggro = false;
                    }
                    break;

                case GameStates.PlayerTurn:

                    cursor.Update(gameTime);

                    foreach (Unit unit in units)
                    {
                        unit.Update(gameTime);
                    }

                    CheckForEndGame();
                    break;

                case GameStates.EnemyTurn:
                    // Return to player turn;
                    EnemyPhase();
                    ReactivateUnits();

                    // Reselect whatever the cursor is looking at
                    Cursor.MoveOnGrid(new Point(0, 0));
                    gameState = GameStates.PlayerTurn;

                    CheckForEndGame();
                    break;

                case GameStates.LevelComplete:
                    if (!gameDone)
                    {
                        gameDone = true;
                        MediaPlayer.Stop();
                        victorySound.Play();
                    }
                    break;

                case GameStates.GameOver:
                    if (!gameDone)
                    {
                        gameDone = true;
                        MediaPlayer.Stop();
                        defeatSound.Play();
                    }
                    break;

                case GameStates.Pause:
                    pauseMenu.Active = true;
                    if ((Keyboard.GetState().IsKeyDown(Keys.Escape)))
                    {
                        pauseTimer++;
                        if (pauseTimer >= 10)
                        {
                            pauseTimer = 0;
                            pauseMenu.Active = false;
                            gameState = GameStates.PlayerTurn;
                        }
                    }
                    break;

                case GameStates.Quit:
                    Exit();
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            if (gameState == GameStates.TitleScreen)
            {
                //Draw title screen
                spriteBatch.Draw(titleScreen, new Rectangle(0, 0, (int)windowSize.X, (int)windowSize.Y), Color.White);
            }
            else if (gameState == GameStates.GameOver)
            {
                Vector2 endTextLoc = endFont.MeasureString("DEFEAT...");
                spriteBatch.DrawString(endFont, "DEFEAT...", new Vector2(graphics.PreferredBackBufferWidth / 2 - endTextLoc.X / 2, graphics.PreferredBackBufferHeight / 2 - endTextLoc.Y / 2), Color.Red);
            }
            else if (gameState == GameStates.LevelComplete)
            {
                Vector2 endTextLoc = endFont.MeasureString("VICTORY!!!");
                spriteBatch.DrawString(endFont, "VICTORY!!!", new Vector2(graphics.PreferredBackBufferWidth / 2 - endTextLoc.X / 2, graphics.PreferredBackBufferHeight / 2 - endTextLoc.Y / 2), Color.White);
            }
            else
            {
                hud.Draw(spriteBatch); // Rendering the hud on the bottom so that units don't get cut off if they are on the top row
                actionBar.Draw(spriteBatch);
                level.Draw(spriteBatch);

                DrawDangerZone();

                if (!actionBar.Active && cursor.InMoveUnitMode)
                    DrawValidMoveTiles();

                if (!actionBar.Active && cursor.InAttackMode)
                    DrawValidAttackTiles();

                DrawUnits(spriteBatch);
                pauseMenu.Draw(spriteBatch);
            }

            if (gameState == GameStates.PlayerTurn)
                cursor.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawValidMoveTiles()
        {
            foreach (Point point in Cursor.validMoveTiles)
            {
                if (GetUnit(point) == null)
                {
                    spriteBatch.Draw(moveTileAnimated.GetTexture(), new Vector2(point.X * Palette.tileSize.X, point.Y * Palette.tileSize.Y + HUD.offset), null, moveTileAnimated.GetFrameRect());
                }
            }
        }

        protected void DrawValidAttackTiles()
        {
            foreach (Point point in Cursor.selectedUnit.validAttackPoints)
            {
                spriteBatch.Draw(attackTileAnimated.GetTexture(), new Vector2(point.X * Palette.tileSize.X, point.Y * Palette.tileSize.Y + HUD.offset), null, attackTileAnimated.GetFrameRect());

            }
        }

        protected void DrawDangerZone()
        {
            foreach (Unit unit in units)
            {
                if (unit.team == Team.Red && unit.selected && unit.alive)
                {
                    foreach (Point point in unit.validAttackPoints)
                        spriteBatch.Draw(attackTileAnimated.GetTexture(), new Vector2(point.X * Palette.tileSize.X, point.Y * Palette.tileSize.Y + HUD.offset), null, attackTileAnimated.GetFrameRect());
                }
            }
        }

        public static void ToggleDangerZone()
        {
            moveCursorSound.Play();
            globalDangerZoneState = !globalDangerZoneState;

            foreach (Unit unit in units)
            {
                if (unit.team == Team.Red)
                {
                    unit.selected = globalDangerZoneState;
                    unit.UpdateDangerZone();
                }
            }
        }

        protected void MoveUnitsToSpawns()
        {
            int redsSpawned = 0;
            int bluesSpawned = 0;

            for (int i = 0; i < units.Count; i++)
            {
                Unit unit = units[i];

                if (unit.team == Team.Blue)
                {
                    unit.Position = Level.blueSpawns[bluesSpawned];
                    bluesSpawned++;
                }
                else
                {
                    unit.Position = Level.redSpawns[redsSpawned];
                    redsSpawned++;
                }
            }
        }

        protected void DrawUnits(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < units.Count; i++)
            {
                units[unitDrawOrder[i]].Draw(spriteBatch);
            }
        }

        // This prevents drawing issues when units are placed above other units
        public static void RefreshUnitDrawOrder()
        {
            for (int i = 0; i < units.Count - 1; i++)
            {
                if (units[unitDrawOrder[i]].Position.Y > units[unitDrawOrder[i + 1]].Position.Y)
                {
                    int temp = unitDrawOrder[i];
                    unitDrawOrder[i] = unitDrawOrder[i + 1];
                    unitDrawOrder[i + 1] = temp;
                }
            }
        }

        public static Unit GetUnit(Point position)
        {
            foreach (Unit unit in units)
            {
                // Return unit at given position, if they are alive. (Dead units are still there, but we pretend they're not)
                if (unit.Position == position && unit.alive)
                    return unit;
            }

            return null;
        }

        public static void ReactivateUnits()
        {
            foreach (Unit unit in units)
            {
                if (unit.alive)
                    unit.active = true;
            }
        }

        public static void CheckForEndTurn()
        {
            foreach (Unit unit in units)
            {
                if (unit.team == Team.Blue)
                    if (unit.active)
                        return;
            }

            Console.WriteLine("Enemy Phase");
            gameState = GameStates.EnemyTurn;
        }

        public static void EnemyPhase()
        {
            foreach (Unit unit in units)
            {
                if (unit.team == Team.Red)
                {
                    unit.AIRoutine();
                }
            }

            // Update attack tiles after the enemy moves
            UpdateDangerZone();
        }

        public static void UpdateDangerZone()
        {
            foreach (Unit unit in units)
            {
                if (unit.team == Team.Red)
                {
                    if (unit.alive)
                        unit.UpdateDangerZone();
                }
            }
        }

        public static void CheckForEndGame()
        {
            int redCount = 0;
            int blueCount = 0;

            foreach (Unit unit in units)
            {
                if (!unit.alive)
                {
                    if (unit.team == Team.Blue)
                    {
                        blueCount++;
                    }
                    else
                    {
                        redCount++;
                    }
                }
            }

            if (redCount == 4)
                gameState = GameStates.LevelComplete;
            if (blueCount == 4)
                gameState = GameStates.GameOver;
        }
    }
}
