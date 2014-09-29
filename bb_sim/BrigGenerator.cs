using System.Collections.Generic;

namespace bb_sim {
    static class BrigGenerator {
        /**
         * Return a randomly generated brig
         */
        public static List<int> getBrig(RandomBrigType randomMode, string tierListString, bool isBloodclash) {
            var randomList = FamiliarDatabase.getRandomFamList(randomMode, tierListString);
            var maxIndex = isBloodclash? 9 : 4;
            List<int> brigIds = new List<int>(new int[maxIndex + 1]);

            if (isBloodclash) {
                // choose a random index for the WL
                var randIndex = BBRandom.getInt(0, maxIndex + 1);
                brigIds[randIndex] = BBRandom.getRandomListItem(FamiliarDatabase.getWarlordList());
            }

            for (var i = 0; i <= maxIndex; i++) {
                // if a spot is vacant (i.e. has no WL), put a random fam in there
                if (brigIds[i] == 0) {
                    brigIds[i] = BBRandom.getRandomListItem(randomList);
                }
            }

            return brigIds;
        }
    } 
}
