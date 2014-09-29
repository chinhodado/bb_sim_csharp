using System;
using System.Collections.Generic;

namespace bb_sim {
    static class BBRandom {
        public static readonly Random rnd = new Random();

        /**
         * Get a random number between [0, 1)
         */
        public static double get() {
            return rnd.NextDouble();
        }

        /**
         * Get a random number between [min, max)
         */
        public static double get(double min, double max) {
            return rnd.NextDouble() * (max - min) + min;
        }

        /**
         * Get a random integer between [min, max)
         */
        public static int getInt(int min, int max) {
            return rnd.Next(min, max);
        }

        /**
         * Get a random item from a list 
         */
        public static T getRandomListItem<T>(List<T> list) {
            int r = rnd.Next(list.Count);
            return list[r];
        }
    }
}
