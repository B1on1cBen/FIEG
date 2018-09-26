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

        protected void LoadTextures()
        {
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
        }

        protected void LoadSounds()
        {
            deniedSound = Content.Load<SoundEffect>("SoundEffects/denied");
            confirmSound = Content.Load<SoundEffect>("SoundEffects/confirm");
            backSound = Content.Load<SoundEffect>("SoundEffects/back");
            moveCursorSound = Content.Load<SoundEffect>("SoundEffects/moveCursor");
            navigateForwardSound = Content.Load<SoundEffect>("SoundEffects/navigateForward");
            navigateBackwardSound = Content.Load<SoundEffect>("SoundEffects/navigateBackward");
            victorySound = Content.Load<SoundEffect>("SoundEffects/victory");
            defeatSound = Content.Load<SoundEffect>("SoundEffects/defeat");
            menuSound = Content.Load<SoundEffect>("SoundEffects/menu");
        }

        protected void LoadMusic()
        {
            music = Content.Load<Song>("Music/Winds Across the Plains");
        }

        protected void LoadFonts()
        {
            font = Content.Load<SpriteFont>("Fonts/Munro");
            hpFont = Content.Load<SpriteFont>("Fonts/Lunchtime");
            promptFont = Content.Load<SpriteFont>("Fonts/MunroSmaller");
            endFont = Content.Load<SpriteFont>("Fonts/ComicSans");
        }

        protected void InitializeUnits()
        {
            #region Red Team
            // WENDY
            units.Add(new Unit(0)
                .SetName("Wendy")
                .SetPortraitSprite(new SubTexture(portraitTexture, new Rectangle(new Point(0, 0), Unit.portraitSize)))
                .SetMapSprite(new SubTexture(unitMapTexture, new Rectangle(new Point(0, 0), new Point(Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2))))
                .SetTeam(Team.Red)
                .SetWeapon(Weapon.Lance)
                .SetStats(new Stats(hp: 49, atk: 30, spd: 21, def: 41, res: 28))
                .SetMoveType(MoveType.Armored)
            );

            // TIKI
            units.Add(new Unit(1)
                .SetName("Tiki")
                .SetPortraitSprite(new SubTexture(portraitTexture, new Rectangle(new Point(98, 0), Unit.portraitSize)))
                .SetMapSprite(new SubTexture(unitMapTexture, new Rectangle(64, 0, Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)))
                .SetTeam(Team.Red)
                .SetWeapon(Weapon.RedDragon)
                .SetStats(new Stats(hp: 40, atk: 38, spd: 20, def: 35, res: 24))
                .SetMoveType(MoveType.Infantry)
            );

            // TITANIA
            units.Add(new Unit(2)
                .SetName("Titania")
                .SetPortraitSprite(new SubTexture(portraitTexture, new Rectangle(new Point(196, 0), Unit.portraitSize)))
                .SetMapSprite(new SubTexture(unitMapTexture, new Rectangle(128, 0, Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)))
                .SetTeam(Team.Red)
                .SetWeapon(Weapon.Axe)
                .SetStats(new Stats(hp: 37, atk: 28, spd: 37, def: 22, res: 30))
                .SetMoveType(MoveType.Cavalry)
            );

            // MAE
            units.Add(new Unit(3)
                .SetName("Mae")
                .SetPortraitSprite(new SubTexture(portraitTexture, new Rectangle(new Point(294, 0), Unit.portraitSize)))
                .SetMapSprite(new SubTexture(unitMapTexture, new Rectangle(192, 0, Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)))
                .SetTeam(Team.Red)
                .SetWeapon(Weapon.BlueTome)
                .SetStats(new Stats(hp: 35, atk: 39, spd: 31, def: 12, res: 30))
                .SetMoveType(MoveType.Infantry)
            );
            #endregion Red Team

            #region Blue Team
            // FELICIA
            units.Add(new Unit(4)
                .SetName("Felicia")
                .SetPortraitSprite(new SubTexture(portraitTexture, new Rectangle(new Point(0, 98), Unit.portraitSize)))
                .SetMapSprite(new SubTexture(unitMapTexture, new Rectangle(0, 128, Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)))
                .SetTeam(Team.Blue)
                .SetWeapon(new Weapon("Plate", 14, 2, WeaponColor.Colorless, DamageType.Def, new Point(1, 4)))
                .SetStats(new Stats(hp: 34, atk: 26, spd: 37, def: 15, res: 35))
                .SetMoveType(MoveType.Infantry)
            );

            // HECTOR
            units.Add(new Unit(5)
                .SetName("Hector")
                .SetPortraitSprite(new SubTexture(portraitTexture, new Rectangle(new Point(294, 98), Unit.portraitSize)))
                .SetMapSprite(new SubTexture(unitMapTexture, new Rectangle(192, 128, Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)))
                .SetTeam(Team.Blue)
                .SetWeapon(new Weapon("Armads", 16, 1, WeaponColor.Green, DamageType.Def, new Point(0, 2)))
                .SetStats(new Stats(hp: 52, atk: 39, spd: 24, def: 34, res: 19))
                .SetMoveType(MoveType.Armored)
            );

            // MARTH
            units.Add(new Unit(6)
                .SetName("Marth")
                .SetPortraitSprite(new SubTexture(portraitTexture, new Rectangle(new Point(98, 98), Unit.portraitSize)))
                .SetMapSprite(new SubTexture(unitMapTexture, new Rectangle(64, 128, Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)))
                .SetTeam(Team.Blue)
                .SetWeapon(new Weapon("Falchion", 16, 1, WeaponColor.Red, DamageType.Def, new Point(0, 1)))
                .SetStats(new Stats(hp: 41, atk: 31, spd: 37, def: 29, res: 20))
                .SetMoveType(MoveType.Infantry)
            );

            // TANA
            units.Add(new Unit(7)
                .SetName("Tana")
                .SetPortraitSprite(new SubTexture(portraitTexture, new Rectangle(new Point(196, 98), Unit.portraitSize)))
                .SetMapSprite(new SubTexture(unitMapTexture, new Rectangle(128, 128, Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)))
                .SetTeam(Team.Blue)
                .SetWeapon(Weapon.Lance)
                .SetStats(new Stats(hp: 36, atk: 37, spd: 36, def: 25, res: 22))
                .SetMoveType(MoveType.Flier)
            );
            #endregion Blue Team
        }

        protected void InitializeObjects()
        {
            palette = new Palette(paletteTexture, new Point(4, 4), 0, 1, 1, 3, 2, 3, 3, 4);
            level = new Level(palette, mapTexture);
            hud = new HUD(hudTexture, iconTexture, font, hpFont);
            actionBar = new ActionBar(actionBarTexture, promptFont);
            pauseMenu = new PauseMenu(pauseMenuTexture);
        }

        protected void SetupWindow()
        {
            graphics.PreferredBackBufferWidth = Level.levelWidth * Palette.tileSize.X;
            graphics.PreferredBackBufferHeight = Level.levelHeight * Palette.tileSize.Y + HUD.offset + ActionBar.offset;
            graphics.ApplyChanges();

            windowSize = new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTextures();
            LoadSounds();
            LoadMusic();
            LoadFonts();
            InitializeObjects();
            SetupWindow();
            InitializeUnits();
            MoveUnitsToSpawns();

            // Initializing cursor last            
            cursor = new Cursor(new Point(2, 7), cursorTexture, moveArrowTexture);

            // Coupling some things because I didn't organize things well enough here.
            ActionBar.cursor = cursor;
            actionBar.SetPrompt("Z/Enter - Confirm, X/Backspace - Back,\n" +
                                "Esc - Menu, Arrows - Navigate");

            Cursor.actionBar = actionBar;
            Cursor.pauseMenu = pauseMenu;
            PauseMenu.cursor = cursor;
        }

        protected override void UnloadContent() { }

        protected void UpdateTitleScreen()
        {
            if ((GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed) || (Keyboard.GetState().IsKeyDown(Keys.Enter)))
            {
                MediaPlayer.IsRepeating = true;
                MediaPlayer.Volume = 0.5f;
                MediaPlayer.Play(music);
                gameState = GameStates.PlayerTurn;
                Unit.enemyAggro = false;
            }
        }

        protected void UpdatePlayerTurn(GameTime gameTime)
        {
            cursor.Update(gameTime);
            CheckForEndGame();
        }

        protected void UpdateEnemyTurn()
        {
            // Return to player turn;
            EnemyPhase();
            ReactivateUnits();

            // Reselect whatever the cursor is looking at
            Cursor.MoveOnGrid(new Point(0, 0));
            gameState = GameStates.PlayerTurn;

            CheckForEndGame();
        }

        protected void UpdateLevelComplete()
        {
            if (!gameDone)
            {
                gameDone = true;
                MediaPlayer.Stop();
                victorySound.Play();
            }
        }

        protected void UpdateGameOver()
        {
            if (!gameDone)
            {
                gameDone = true;
                MediaPlayer.Stop();
                defeatSound.Play();
            }
        }

        protected void UpdatePause()
        {
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
        }

        protected void UpdateAnimatedTextures(GameTime gameTime)
        {
            foreach (AnimatedTexture animTexture in AnimatedTexture.AnimatedTextures)
                animTexture.Update(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                MediaPlayer.Stop();
                Exit();
            }

            UpdateAnimatedTextures(gameTime);

            switch (gameState)
            {
                case GameStates.TitleScreen:
                    UpdateTitleScreen();
                    break;

                case GameStates.PlayerTurn:
                    UpdatePlayerTurn(gameTime);
                    break;

                case GameStates.EnemyTurn:
                    UpdateEnemyTurn();
                    break;

                case GameStates.LevelComplete:
                    UpdateLevelComplete();
                    break;

                case GameStates.GameOver:
                    UpdateGameOver();
                    break;

                case GameStates.Pause:
                    UpdatePause();
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
                DrawGame();
            }

            if (gameState == GameStates.PlayerTurn)
                cursor.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawGame()
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

        protected void DrawValidMoveTiles()
        {
            foreach (Point point in Cursor.validMoveTiles)
            {
                if (GetUnit(point) == null)
                    spriteBatch.Draw(moveTileAnimated.GetTexture(), new Vector2(point.X * Palette.tileSize.X, point.Y * Palette.tileSize.Y + HUD.offset), null, moveTileAnimated.GetFrameRect());
            }
        }

        protected void DrawValidAttackTiles()
        {
            foreach (Point point in Cursor.selectedUnit.validAttackPoints)
                spriteBatch.Draw(attackTileAnimated.GetTexture(), new Vector2(point.X * Palette.tileSize.X, point.Y * Palette.tileSize.Y + HUD.offset), null, attackTileAnimated.GetFrameRect());
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
                for (int j = 0; j < units.Count; j++)
                {
                    if (units[j].drawOrder == i)
                        units[j].Draw(spriteBatch);
                }
            }
        }

        // This prevents drawing issues when units are placed above other units
        public static void RefreshUnitDrawOrder()
        {
            for (int i = 0; i < units.Count; i++)
            {
                for (int j = 0; j < units.Count; j++)
                {
                    if (i == j)
                        continue;

                    if (units[i].Position.Y > units[j].Position.Y && units[i].drawOrder < units[j].drawOrder)
                    {
                        units[i].drawOrder = MathHelper.Clamp(units[i].drawOrder + 1, 0, units.Count - 1);
                        units[j].drawOrder = units[i].drawOrder - 1;
                    }
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
                if (unit.team == Team.Blue && unit.active)
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
                    unit.AIRoutine();
            }

            UpdateDangerZone();
        }

        public static void UpdateDangerZone()
        {
            foreach (Unit unit in units)
            {
                if (unit.team == Team.Red && unit.alive)
                    unit.UpdateDangerZone();
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
                        blueCount++;
                    else
                        redCount++;
                }
            }

            if (redCount == 4)
                gameState = GameStates.LevelComplete;

            if (blueCount == 4)
                gameState = GameStates.GameOver;
        }
    }
}
