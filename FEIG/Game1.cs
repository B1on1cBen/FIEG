// Written by Ben Gordon and Shawn Murdoch

using FEIG.Graphics;
using FEIG.Map;
using FEIG.Input;
using FEIG.Units;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using FEIG.UI;

namespace FEIG
{
#pragma warning disable CS0618 // For spritebatch thing that apparently isn't needed anymore?? Probably just that particular overload.
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static readonly Vector2 WindowScale = new Vector2(.5f, .5f);

        Texture2D mapTexture;
        Texture2D plainsTexture;
        Texture2D forestTexture;
        AnimatedTexture waterTextureAnimated;
        Texture2D mountainsTexture;
        Texture2D cursorTexture;
        Texture2D hudTexture;
        Texture2D portraitTexture;
        Texture2D unitMapTexture;
        Texture2D iconTexture;
        Texture2D actionBarTexture;
        Texture2D titleScreen;
        Texture2D moveArrowTexture;
        Texture2D pauseMenuTexture;

        public static Texture2D moveTileTexture;
        public static AnimatedTexture cursorTextureAnimated;
        public static AnimatedTexture moveTileAnimated;
        public static AnimatedTexture attackTileAnimated;

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

        public enum GameStates
        {
            TitleScreen,
            PlayerTurn,
            EnemyTurn,
            Combat,
            LevelComplete,
            GameOver,
            Quit
        };

        public static GameStates gameState = GameStates.TitleScreen;

        Level level;
        public static Cursor cursor;
        HUD hud;
        ActionBar actionBar;
        PauseMenu pauseMenu;

        public static Point windowSize;
        public static SpriteFont font;
        public static SpriteFont hpFont;
        public static SpriteFont promptFont;
        public static SpriteFont endFont;

        public static List<Unit> units = new List<Unit>();
        public static bool globalDangerZoneState = false;
        public static bool gameDone = false;
        public static bool enemyHasAggro = false;

        public static readonly Dictionary<string, CursorInput> Input = new Dictionary<string, CursorInput>()
        {
            {"Confirm",          new CursorInput(new Keys[] {Keys.Z, Keys.Enter},             new Buttons[] {Buttons.A})},
            {"Back",             new CursorInput(new Keys[] {Keys.X, Keys.Escape, Keys.Back}, new Buttons[] {Buttons.B})},
            {"SwitchWeapon",     new CursorInput(new Keys[] {Keys.C},                         new Buttons[] {Buttons.X})},
            {"ToggleDangerZone", new CursorInput(new Keys[] {Keys.Space},                     new Buttons[] {Buttons.Y})},
            {"Up",               new CursorInput(new Keys[] {Keys.Up, Keys.W},                new Buttons[] {Buttons.DPadUp})},
            {"Down",             new CursorInput(new Keys[] {Keys.Down, Keys.S},              new Buttons[] {Buttons.DPadDown})},
            {"Left",             new CursorInput(new Keys[] {Keys.Left, Keys.A},              new Buttons[] {Buttons.DPadLeft})},
            {"Right",            new CursorInput(new Keys[] {Keys.Right, Keys.D},             new Buttons[] {Buttons.DPadRight})}
        };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected void LoadTextures()
        {
            mapTexture = Content.Load<Texture2D>("Textures/Maps/Map1");

            plainsTexture = Content.Load<Texture2D>("Textures/TilePalettes/FIEG_PlainsTiles");
            forestTexture = Content.Load<Texture2D>("Textures/TilePalettes/FIEG_ForestTiles");
            mountainsTexture = Content.Load<Texture2D>("Textures/TilePalettes/FIEG_MountainTiles");
            waterTextureAnimated = new AnimatedTexture(new SpriteSheet(Content.Load<Texture2D>("Textures/TilePalettes/FIEG_WaterTiles"), new Point(2, 2), new Point(64)), 500);

            cursorTexture = Content.Load<Texture2D>("Textures/Cursor");
            cursorTextureAnimated = new AnimatedTexture(new SpriteSheet(cursorTexture, new Point(1, 2), new Point(64)), 500, includeInList: false);

            hudTexture = Content.Load<Texture2D>("Textures/FEIG HUD 2");
            portraitTexture = Content.Load<Texture2D>("Textures/Portraits/Portraits");
            unitMapTexture = Content.Load<Texture2D>("Textures/Units/Units");
            iconTexture = Content.Load<Texture2D>("Textures/Icons");
            actionBarTexture = Content.Load<Texture2D>("Textures/ActionBar");
            titleScreen = Content.Load<Texture2D>("Textures/FEIG_Logo");
            moveArrowTexture = Content.Load<Texture2D>("Textures/MoveArrow");
            pauseMenuTexture = Content.Load<Texture2D>("Textures/FEIG Menu");

            moveTileTexture = Content.Load<Texture2D>("Textures/MoveTiles");
            moveTileAnimated = new AnimatedTexture(new SpriteSheet(moveTileTexture, new Point(3, 16), new Point(64)), new Point(0, 0), AnimatedTexture.LoopType.Horizontal, 100);
            attackTileAnimated = new AnimatedTexture(new SpriteSheet(moveTileTexture, new Point(3, 16), new Point(64)), new Point(0, 1), AnimatedTexture.LoopType.Horizontal, 100);
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
            font = Content.Load<SpriteFont>("Fonts/half-scale/MunroHalf");
            hpFont = Content.Load<SpriteFont>("Fonts/half-scale/LunchtimeHalf");
            promptFont = Content.Load<SpriteFont>("Fonts/half-scale/MunroSmallerHalf");
            endFont = Content.Load<SpriteFont>("Fonts/half-scale/ComicSansHalf");
        }

        protected void InitializeRedTeam()
        {
            // WENDY
            units.Add(new Unit(0)
                .SetName("Gwen")
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
        }

        protected void InitializeBlueTeam()
        {
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

            #region Veronica
            // VERONICA
            //units.Add(new Unit(7)
            //    .SetName("Veronica")
            //    .SetPortraitSprite(new SubTexture(portraitTexture, new Rectangle(new Point(196, 98), Unit.portraitSize)))
            //    .SetMapSprite(new SubTexture(unitMapTexture, new Rectangle(128, 128, Unit.mapUnitSize.X, Unit.mapUnitSize.Y * 2)))
            //    .SetTeam(Team.Blue)
            //    .SetWeapon(new Weapon("Glaive", 16, 1, WeaponColor.Blue, DamageType.Def, new Point(0, 3)))
            //    .SetSecondaryWeapon(new Weapon("Chakram", 14, 2, WeaponColor.Green, DamageType.Def, new Point(0, 2)))
            //    .SetStats(new Stats(hp: 37, atk: 28, spd: 37, def: 26, res: 29))
            //    .SetMoveType(MoveType.Infantry)
            //);
            #endregion Veronica
        }

        protected void InitializeUnits()
        {
            InitializeRedTeam();
            InitializeBlueTeam();
            MoveUnitsToSpawns();
        }

        protected void InitializeLevel()
        {
            level = new Level(
                mapTexture,
                new TileSet(new Color(214, 233, 185), TileType.Plains, new SpriteSheet(plainsTexture, new Point(1, 1), new Point(64))),
                new TileSet(new Color(76, 138, 103), TileType.Forest, new SpriteSheet(forestTexture, new Point(3, 1), new Point(64))),
                new TileSet(new Color(114, 109, 108), TileType.Mountain, new SpriteSheet(mountainsTexture, new Point(3, 1), new Point(64))),
                new AnimatedTileSet(new Color(76, 76, 255), TileType.Water, waterTextureAnimated)
            );

            Level.tileSize = new Point((int)(Level.tileSize.X * WindowScale.X), (int)(Level.tileSize.Y * WindowScale.Y));
        }

        protected void InitializeMenus()
        {
            pauseMenu = new PauseMenu(pauseMenuTexture);
            hud = new HUD(hudTexture, iconTexture, font, hpFont);
            actionBar = new ActionBar(actionBarTexture, promptFont, "Z/Enter - Confirm, X/Backspace - Back,\n" +
                                                                    "Esc - Menu, Arrows - Navigate");
        }

        protected void SetupWindow()
        {
            int width = Level.levelWidth * Level.tileSize.X;
            int height = Level.levelHeight * Level.tileSize.Y + (int)((HUD.offset + ActionBar.offset) * WindowScale.Y);
            windowSize = new Point(width, height);
            graphics.PreferredBackBufferWidth = windowSize.X;
            graphics.PreferredBackBufferHeight = windowSize.Y;
            graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            LoadTextures();
            InitializeLevel();
            LoadSounds();
            LoadMusic();
            LoadFonts();
            SetupWindow();
            InitializeUnits();
            InitializeMenus();

            cursor = new Cursor(new Point(2, 7), cursorTextureAnimated, moveArrowTexture, actionBar, pauseMenu);
            ActionBar.cursor = cursor;
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
                enemyHasAggro = false;
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

        protected void UpdateAnimatedTextures(GameTime gameTime)
        {
            foreach (AnimatedTexture animTexture in AnimatedTexture.AnimatedTextures)
                animTexture.Update(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            // HACK: Makes closing the game easier while testing.
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
                spriteBatch.Draw(titleScreen, new Rectangle(0, 0, windowSize.X, windowSize.Y), Color.White);
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
            // Rendering the hud on the bottom so that units don't get cut off if they are on the top row
            hud.Draw(spriteBatch); 
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
            foreach (Point point in cursor.validMoveTiles)
            {
                if (GetUnit(point) == null)
                    this.spriteBatch.Draw(moveTileAnimated.GetTexture(), new Vector2(point.X * Level.tileSize.X, point.Y * Level.tileSize.Y + HUD.offset * WindowScale.Y), null, moveTileAnimated.GetFrameRect(), scale: WindowScale);
            }
        }

        protected void DrawValidAttackTiles()
        {
            foreach (Point point in Cursor.selectedUnit.validAttackPoints)
                this.spriteBatch.Draw(attackTileAnimated.GetTexture(), new Vector2(point.X * Level.tileSize.X, point.Y * Level.tileSize.Y + HUD.offset * WindowScale.Y), null, attackTileAnimated.GetFrameRect(), scale: WindowScale);
        }

        protected void DrawDangerZone()
        {
            foreach (Unit unit in units)
            {
                if (unit.team == Team.Red && unit.selected && unit.alive)
                {
                    foreach (Point point in unit.validAttackPoints)
                        spriteBatch.Draw(attackTileAnimated.GetTexture(), new Vector2(point.X * Level.tileSize.X, point.Y * Level.tileSize.Y + HUD.offset * WindowScale.Y), null, attackTileAnimated.GetFrameRect(), scale: WindowScale);
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
