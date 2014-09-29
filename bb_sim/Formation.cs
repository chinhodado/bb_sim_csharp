using System.Collections.Generic;

namespace bb_sim {
    class Formation {
        // 1: front, 2: mid, 3: rear
        private static readonly Dictionary<FormationType, int[]> FORMATION_CONFIG = new Dictionary<FormationType, int[]> {
            {FormationType.SKEIN_5,  new [] {3, 2, 1, 2, 3}}, 
            {FormationType.VALLEY_5, new [] {1, 2, 3, 2, 1}},
            {FormationType.TOOTH_5,  new [] {1, 3, 1, 3, 1}},
            {FormationType.WAVE_5,   new [] {3, 1, 2, 1, 3}},
            {FormationType.FRONT_5,  new [] {1, 1, 1, 1, 1}},
            {FormationType.MID_5,    new [] {2, 2, 2, 2, 2}},
            {FormationType.REAR_5,   new [] {3, 3, 3, 3, 3}},
            {FormationType.PIKE_5,   new [] {3, 3, 1, 3, 3}},
            {FormationType.SHIELD_5, new [] {1, 1, 3, 1, 1}},
            {FormationType.PINCER_5, new [] {3, 1, 3, 1, 3}},
            {FormationType.SAW_5,    new [] {1, 3, 2, 3, 1}},
            {FormationType.HYDRA_5,  new [] {3, 3, 1, 1, 1}},
        };

        static readonly Dictionary<FormationRow, int[]> ANDROID_PROC_ORDER = new Dictionary<FormationRow, int[]> {
            {FormationRow.FRONT, new [] {11, 15, 14, 13, 12}},
            {FormationRow.MID,   new [] {6, 10, 9, 8, 7}},
            {FormationRow.REAR,  new [] {1, 5, 4, 3, 2}},
        };

        static readonly Dictionary<FormationRow, int[]> IOS_PROC_ORDER = new Dictionary<FormationRow, int[]> {
            {FormationRow.FRONT, new [] {11, 12, 13, 14, 15}},
            {FormationRow.MID,   new [] {6, 7, 8, 9, 10}},
            {FormationRow.REAR,  new [] {1, 2, 3, 4, 5}},
        };

        readonly FormationType type;

        public Formation(FormationType type) {
            this.type = type;
        }

        public static int getProcIndex(FormationRow row, int column, ProcOrderType type) {
            var order = (type == ProcOrderType.ANDROID)? ANDROID_PROC_ORDER : IOS_PROC_ORDER;

            return order[row][column];
        }
    
        /**
         * Given a position (from 0-5), return the row of the familiar currently at that position based
         * on the current formation
         */
        public FormationRow getCardRow(int position) {
            return (FormationRow) FORMATION_CONFIG[type][position];
        }
    
        /**
         * Return the config array of the current formation
         */
        public int[] getFormationConfig() {
            return FORMATION_CONFIG[type];
        }
    }
}
