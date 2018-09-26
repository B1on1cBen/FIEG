// Written By Ben Gordon

namespace FEIG.Units
{
    public struct Stats
    {
        private int[] stats;

        public Stats(int hp, int atk, int spd, int def, int res)
        {
            stats = new int[5] {
                hp,
                atk,
                spd,
                def,
                res
            };
        }

        public int HP
        {
            get { return stats[0]; }
        }

        public int ATK
        {
            get { return stats[1]; }
        }

        public int SPD
        {
            get { return stats[2]; }
        }

        public int DEF
        {
            get { return stats[3]; }
        }

        public int RES
        {
            get { return stats[4]; }
        }

        // Get stats by index
        public int this[int index]
        {
            get
            {
                return stats[index];
            }
        }
    }
}
