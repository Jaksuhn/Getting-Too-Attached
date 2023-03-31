using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GettingTooAttached.Helpers
{
    internal static class Throttler
    {
        static long NextCommandAt = 0;
        internal static bool Throttle(int ms)
        {
            if (Environment.TickCount64 > NextCommandAt)
            {
                if (ms > 0)
                {
                    NextCommandAt = Environment.TickCount64 + ms;
                }
                return true;
            }
            return false;
        }

        internal static void Rethrottle(int ms)
        {
            if (NextCommandAt - Environment.TickCount64 < ms)
            {
                NextCommandAt = Environment.TickCount64 + ms;
            }
        }
    }
}