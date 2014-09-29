using System;
using System.Collections.Generic;

namespace bb_sim {
    static class Util {
        /**
         * Shuffle a list. The original list will be modified.
         */
        public static void Shuffle<T>(this IList<T> list) {
            Random rng = BBRandom.rnd;
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
