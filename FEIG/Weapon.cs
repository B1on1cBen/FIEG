// Written by Ben Gordon

using Microsoft.Xna.Framework;

namespace FEIG.Units
{
    public enum WeaponColor
    {
        Red,
        Green,
        Blue,
        Colorless
    }

    public enum DamageType
    {
        Def = 3, // Parallel with stats[3], which is def
        Res = 4, // Parallel with stats[4], which is res
        Heal
    }

    public class Weapon
    {
        #region Common Weapons
        // Some common weapons that we will most likely reuse
        public static readonly Weapon Sword = new Weapon("Sword", 16, 1, WeaponColor.Red, DamageType.Def, new Point(0, 1));
        public static readonly Weapon Axe = new Weapon("Axe", 16, 1, WeaponColor.Green, DamageType.Def, new Point(0, 2));
        public static readonly Weapon Lance = new Weapon("Lance", 16, 1, WeaponColor.Blue, DamageType.Def, new Point(0, 3));

        public static readonly Weapon RedTome = new Weapon("Fire", 11, 2, WeaponColor.Red, DamageType.Res, new Point(1, 1));
        public static readonly Weapon GreenTome = new Weapon("Wind", 11, 2, WeaponColor.Green, DamageType.Res, new Point(1, 2));
        public static readonly Weapon BlueTome = new Weapon("Lightning", 11, 2, WeaponColor.Blue, DamageType.Res, new Point(1, 3));

        public static readonly Weapon RedDragon = new Weapon("Flame", 16, 1, WeaponColor.Red, DamageType.Res, new Point(2, 1));
        public static readonly Weapon GreenDragon = new Weapon("Light", 16, 1, WeaponColor.Blue, DamageType.Res, new Point(2, 2));
        public static readonly Weapon BlueDragon = new Weapon("Dark", 16, 1, WeaponColor.Green, DamageType.Res, new Point(2, 2));

        public static readonly Weapon ClericStaff = new Weapon("Staff", 7, 2, WeaponColor.Colorless, DamageType.Heal, new Point(0, 4));
        public static readonly Weapon MageStaff = new Weapon("Staff", 7, 2, WeaponColor.Colorless, DamageType.Res, new Point(0, 4));
        public static readonly Weapon Dagger = new Weapon("Dagger", 9, 2, WeaponColor.Colorless, DamageType.Def, new Point(1, 4));
        public static readonly Weapon Bow = new Weapon("Bow", 9, 2, WeaponColor.Colorless, DamageType.Def, new Point(2, 4));
        #endregion Common Weapons

        public string name;
        public int might;
        public int range;
        public WeaponColor color;
        public DamageType damageType;
        public Point iconIndex;

        public Weapon(string name, int might, int range, WeaponColor color, DamageType damageType, Point iconIndex)
        {
            this.name = name;
            this.might = might;
            this.range = range;
            this.color = color;
            this.damageType = damageType;
            this.iconIndex = iconIndex;
        }
    }
}
