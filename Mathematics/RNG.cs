using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Clockwork.Retro.Mathematics
{
    public static class RNG
    {
        // Constants
        public const uint A = 0x41C64E6D;
        public const uint C = 0x6073;


        // Current seed
        private static uint _seed;


        // Properties
        public static uint CurrentSeed => _seed;


        // Static Constructor
        static RNG()
        {
            DateTime time = DateTime.Now;

            _seed = (uint)(time.Day << 27 | time.Month << 23 | time.Year << 12 | time.Hour << 8 | time.Minute << 4 | time.Second);
        }


        // Func
        public static ushort Next()
        {
            _seed = _seed * A + C;
            return (ushort)(_seed >> 16);
        }
        public static int Next(int max) => Next() % max;
        public static int Next(int min, int max) => min + (Next() % (max - min));

        public static byte NextByte()
        {
            _seed = _seed * A + C;
            return (byte)(_seed >> 24);
        }
        public static byte NextByte(byte max) => (byte)(NextByte() % max);
        public static byte NextByte(byte min, byte max) => (byte)(min + (NextByte() % (max - min)));

        public static float NextFloat() => Next() / 65535f;
        public static float NextFloat(float max) => NextFloat() * max;
        public static float NextFloat(float min, float max) => min + (max - min) * NextFloat();
    }
}
