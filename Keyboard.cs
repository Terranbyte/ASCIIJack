using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blackjack
{
    class Keyboard
    {
        private static ConsoleKey prevKey;
        private static bool prevState;

        public static bool IsAnyKeyDown()
        {
            if (!Console.KeyAvailable)
            {
                return false;
            }

            return true;
        }

        public static bool IsKeyPressed(ConsoleKey Key, bool halt = false)
        {
            if (!Console.KeyAvailable && !halt)
            {
                prevKey = ConsoleKey.F24;
                return false;
            }

            if (Console.ReadKey(true).Key != Key)
            {
                return false;
            }

            prevKey = Key;
            return true;
        }
    }
}
