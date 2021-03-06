﻿// Written by Ben Gordon and Shawn Murdoch

using FEIG.Graphics;
using FEIG.Input;
using FEIG.Map;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FEIG.Units
{
    public enum Team
    {
        Red,
        Blue
    }

    public enum MoveType
    {
        Infantry,
        Armored,
        Flier,
        Cavalry
    }

    public enum TypeMatchUp
    {
        Neutral = 0,
        Advantage = 1,
        Disadvantage = -1
    }

#pragma warning disable CS0618
    public class Unit
    {
        // Tells unit if it can pass Plains, Forest, Mountain, or Water (also including white and black spawns for error cases)
        // Parallel with MoveType enum
        static readonly bool[][] passableTerrain = {
            // Infantry - Can't pass thru mountains or oceans
            new bool[] {true, true, false, false, true, true},

            // Armored - Same as infantry
            new bool[] {true, true, false, false, true, true},

            // Flier - Can pass thru everything
            new bool[] {true, true, true, true, true, true},

            // Cavalry - Similar to infantry, but can't pass thru forests
            new bool[] {true, false, false, false, true, true }
        };

        public static readonly int[] moveRanges = {
            2, 1, 2, 3 // Infantry, Armored, Flier, Calvalry
        };

        // Contains all of the values for modifying damage based on type matchups 
        private static readonly Dictionary<int, float> damageModifiers = new Dictionary<int, float>() {
            // {Type matchup index, damage multiplier}
            { 0,   1},  // Neutral (100%)
            { 1, 1.2f}, // Advantage (120%)
            {-1, 0.8f}  // Disadvantage (80%)
        };

        public static readonly Point portraitSize = new Point(98, 98);
        public static readonly Point mapUnitSize = new Point(64, 64);
        public static Cursor cursor; // A handy reference to the cursor

        // These will not change
        public string name;

        public SubTexture portraitTexture;
        public SubTexture mapTexture;
        public Team team;
        public Weapon weapon;
        public Weapon primaryWeapon;
        public Weapon secondaryWeapon;
        public Stats stats;
        public MoveType moveType;

        // These will change
        public bool active = true;
        public bool selected = false;
        public bool alive = true;
        public int drawOrder;
        private Point position; // This has a public accessor
        private int currentHP; // This has a public accessor
        private int currentCombatRounds = 1;

        public List<Point> validAttackPoints = new List<Point>();
        private List<Point> validMovePoints = new List<Point>();

        public Unit(int drawOrder)
        {
            this.drawOrder = drawOrder;
        }

        public Unit SetName(string name)
        {
            this.name = name;
            return this;
        }

        public Unit SetPosition(Point position)
        {
            this.Position = position;
            return this;
        }

        public Unit SetPortraitSprite(SubTexture portraitTexture)
        {
            this.portraitTexture = portraitTexture;
            return this;
        }

        public Unit SetMapSprite(SubTexture mapTexture)
        {
            this.mapTexture = mapTexture;
            return this;
        }

        public Unit SetTeam(Team team)
        {
            this.team = team;
            return this;
        }

        public Unit SetWeapon(Weapon weapon)
        {
            this.weapon = weapon;
            this.primaryWeapon = weapon;
            return this;
        }

        public Unit SetSecondaryWeapon(Weapon weapon)
        {
            this.secondaryWeapon = weapon;
            return this;
        }

        public Unit SetStats(Stats stats)
        {
            this.stats = stats;
            currentHP = stats.HP;
            return this;
        }

        public Unit SetMoveType(MoveType moveType)
        {
            this.moveType = moveType;
            return this;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (alive)
            {
                // This will be the tint of the unit. White is normal
                Color color = Color.White;

                if (team == Team.Red && selected)
                {
                    // Tints the unit red, because they are an enemy and we have enabled danger zone display
                    color = new Color(255, 100, 100, 255);
                }

                // Tint unit gray because they have used up their action
                if (!active)
                    color = Color.Gray;

                // Draw the unit sprite
                spriteBatch.Draw(mapTexture.texture, new Vector2(Position.X * Level.tileSize.X, Position.Y * Level.tileSize.Y + HUD.offset * Game1.WindowScale.Y - mapUnitSize.Y * Game1.WindowScale.Y), null, mapTexture.rect, null, 0, Game1.WindowScale, color, SpriteEffects.None, 1);
            }
        }

        public void SwapWeapon()
        {
            if (secondaryWeapon == null)
                return;

            if (weapon == primaryWeapon)
                weapon = secondaryWeapon;
            else
                weapon = primaryWeapon;
        }

        public void StartCombat(Unit target)
        {
            currentCombatRounds = (stats.SPD - target.stats.SPD) >= 5 ? 2 : 1;

            if (target.DistanceTo(position) != target.weapon.range)
                target.currentCombatRounds = 0;
            else
                target.currentCombatRounds = (target.stats.SPD - stats.SPD) >= 5 ? 2 : 1;

            Attack(target);
        }

        void Attack(Unit target)
        {
            float myDamage = weapon.might + stats.ATK;
            myDamage *= damageModifiers[(int)GetMyTypeMatchUp(target.weapon)];
            myDamage -= target.stats[(int)weapon.damageType];
            myDamage = MathHelper.Clamp(myDamage, 0, 999);

            Console.WriteLine(name + " attacked " + target.name + "(" + GetMyTypeMatchUp(target.weapon) + ")");
            Console.WriteLine(target.name + " took " + (int)myDamage + "!\n");
            target.CurrentHP -= (int)myDamage;
            currentCombatRounds--;

            if (!target.alive)
                return;

            if (target.currentCombatRounds > 0)
                target.Attack(this);
            else if (currentCombatRounds > 0)
                Attack(target);
        }

        public void AIRoutine()
        {
            if (!active || !alive)
                return;

            validMovePoints.Clear();
            UpdateDangerZone();
            Unit[] targetsInRange = GetTargetableUnits();

            if (targetsInRange.Length <= 0 && !Game1.enemyHasAggro)
            {
                active = false;
                return;
            }

            if (targetsInRange.Length > 0)
                AttackPlayerUnit(targetsInRange);
            else
                AdvanceOnPlayerUnits();
        }

        void AttackPlayerUnit(Unit[] targetsInRange)
        {
            Game1.enemyHasAggro = true;

            Unit target = ChooseTarget(targetsInRange);
            if (target == null)
            {
                active = false;
                return;
            }

            Point movePoint = GetAttackingPosition(target.position);
            if (movePoint == new Point(-1))
            {
                active = false;
                return;
            }
            Position = movePoint;

            StartCombat(target);
            active = false;
        }

        void AdvanceOnPlayerUnits()
        {
            List<Unit> enemies = new List<Unit>();

            foreach (Unit unit in Game1.units)
            {
                if (unit.team != team && unit.alive)
                    enemies.Add(unit);
            }

            Unit target = ChooseTarget(enemies.ToArray());
            if (target == null)
            {
                active = false;
                return;
            }

            int smallestDistance = Level.levelWidth * Level.levelHeight;
            int smallestIndex = 0;

            // Find closest point to target
            for (int i = 0; i < validMovePoints.Count; i++)
            {
                int distance = DistanceBetween(validMovePoints[i], target.position);

                if (smallestDistance < distance)
                {
                    smallestDistance = distance;
                    smallestIndex = i;
                }
            }

            // Go to closest point to target
            Position = validMovePoints[smallestIndex];
            active = false;
        }

        /// <summary>
        /// Sets list of tiles equal to attack range from position
        /// </summary>
        public void UpdateDangerZone()
        {
            List<Point> points = new List<Point>();
            PathFinder pathFinder = new PathFinder();

            // Add in all valid move tiles
            for (int y = 0; y < Level.grid.GetLength(1); y++)
            {
                for (int x = 0; x < Level.grid.GetLength(0); x++)
                {
                    Point movePoint = new Point(x, y);

                    if (pathFinder.ValidPath(this, position, movePoint))
                    {
                        points.Add(movePoint);
                        validMovePoints.Add(movePoint);
                    }
                }
            }

            // Save the current size. (Because we want to use this size and not have it change)
            int count = points.Count;

            // Add in all tiles next to valid move tiles that are within weapon range
            for (int i = 0; i < count; i++)
            {
                for (int y = 0; y < Level.grid.GetLength(1); y++)
                {
                    for (int x = 0; x < Level.grid.GetLength(0); x++)
                    {
                        Point attackPoint = new Point(x, y);

                        if ((DistanceBetween(points[i], attackPoint) == weapon.range) && !points.Contains(attackPoint))
                            points.Add(attackPoint);
                    }
                }
            }

            validAttackPoints.Clear();
            validAttackPoints = points;
        }

        /// <summary>
        /// Sets list of tiles equal to attack range from position
        /// </summary>
        public void UpdateAttackTiles()
        {
            List<Point> points = new List<Point>();

            for (int y = 0; y < Level.grid.GetLength(1); y++)
            {
                for (int x = 0; x < Level.grid.GetLength(0); x++)
                {
                    Point attackPoint = new Point(x, y);

                    if ((DistanceBetween(position, attackPoint) == weapon.range) && !points.Contains(attackPoint))
                        points.Add(attackPoint);
                }
            }

            validAttackPoints.Clear();
            validAttackPoints = points;
        }

        #region Public Accessors
        public Point Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;

                // Reorder the layers for units, in case some are now cut off.
                Game1.RefreshUnitDrawOrder();
            }
        }

        public int CurrentHP
        {
            get { return currentHP; }

            set
            {
                currentHP = value;
                if (currentHP <= 0)
                {
                    alive = false;

                    Game1.CheckForEndGame();

                    validMovePoints.Clear();
                    validAttackPoints.Clear();

                    if (Cursor.hoveredUnit == this)
                    {
                        Cursor.hoveredUnit = null;
                        Cursor.MoveOnGrid(Cursor.Position);
                    }
                }
            }
        }
        #endregion Public Accessors

        #region Helpers
        public bool CanPass(TileType tile)
        {
            return passableTerrain[(int)moveType][(int)tile];
        }

        public bool CanStandOnPoint(Point point)
        {
            // Can move to point if:
            // 1. Within movement range
            // 2. Moving to valid tile type for unit's move type
            // 3. Tile is not already occupied by another unit

            int mod = 0;
            if (moveType == MoveType.Infantry && Level.GetTile(point).type == TileType.Forest)
                mod -= 1;

            if (DistanceTo(point) <= moveRanges[(int)moveType] + mod)
            {
                if (CanPass(Level.grid[point.X, point.Y].type))
                {
                    Unit unit = Game1.GetUnit(point);

                    if (unit == this || unit == null)
                        return true;
                }
            }

            return false;
        }

        private bool CanCounterMe(Unit unit)
        {
            return unit.weapon.range == weapon.range;
        }

        public TypeMatchUp GetMyTypeMatchUp(Weapon otherWeapon)
        {
            if (weapon.color == WeaponColor.Colorless || otherWeapon.color == WeaponColor.Colorless)
                return TypeMatchUp.Neutral;

            if (weapon.color == otherWeapon.color)
                return TypeMatchUp.Neutral;

            // Since color advantages go in a positive cycle, 
            // just look at the next color in the cycle and see if it's the enemy's color. 
            // If not, it means a negative matchup
            int myColor = (int)weapon.color;
            int nextColor = (myColor + 1) % 3;
            int otherColor = (int)otherWeapon.color;

            if (nextColor == otherColor)
                return TypeMatchUp.Advantage;

            return TypeMatchUp.Disadvantage;
        }

        public int DistanceTo(Point point)
        {
            return DistanceBetween(position, point);
        }

        public int DistanceBetween(Point a, Point b)
        {
            return DistanceBetween(a.X, a.Y, b.X, b.Y);
        }

        public int DistanceBetween(int aX, int aY, int bX, int bY)
        {
            return Math.Abs(bX - aX) + Math.Abs(bY - aY);
        }

        private Unit[] GetTargetableUnits()
        {
            List<Unit> targetableUnits = new List<Unit>();

            for (int i = 0; i < validAttackPoints.Count; i++)
            {
                Unit target = Game1.GetUnit(validAttackPoints[i]);

                if (target != null && target.team != team && target.alive)
                    targetableUnits.Add(target);
            }

            return targetableUnits.ToArray();
        }

        private Unit ChooseTarget(Unit[] targets)
        {
            int[] fitnesses = new int[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                // Can they counter me?
                if (!CanCounterMe(targets[i]))
                    fitnesses[i] += 2;
                else
                    fitnesses[i] -= 2;

                // Do I have weapon advantage?
                fitnesses[i] += (int)GetMyTypeMatchUp(targets[i].weapon);
            }

            // Get most fit target
            int maxFitness = 0;
            int fittestIndex = 0;
            for (int i = 0; i < fitnesses.Length; i++)
            {
                if (fitnesses[i] > maxFitness)
                {
                    maxFitness = fitnesses[i];
                    fittestIndex = i;
                }
            }

            if (fittestIndex < targets.Length)
                return targets[fittestIndex];

            return null;
        }

        private Point GetAttackingPosition(Point enemy)
        {
            foreach (Point point in validMovePoints)
            {
                if (DistanceBetween(point, enemy) == weapon.range)
                    return point;
            }

            // Return a "null" point because we can't attack from anywhere.
            // This might happen if ranged units are cornered.
            return new Point(-1, -1);
        }
        #endregion Helpers
    }
}
