using System.Collections.Generic;

namespace bb_sim {
    struct SkillInfo {
        public readonly string name;
        public readonly SkillType type;
        public readonly SkillFunc func;
        public readonly SkillCalcType calc;
        public readonly double arg1;
        public readonly double arg2;
        public readonly double arg3;
        public readonly double arg4;
        public readonly double arg5;
        public readonly SkillRange range;
        public readonly int prob;
        public readonly WardType ward;
        public readonly bool isAutoAttack;
        public readonly string desc;
        public List<int> randSkills; 
    }

    class SkillDatabase {
        private static Dictionary<int, SkillInfo> db;
        static List<int> availableSkillsForSelect;

        public static SkillInfo get(int id) {
            return db[id];
        }

        /**
         * Return a list of ids of the skills available for selection
         */
        public static List<int> getAvailableSkillsForSelect() {

            if (availableSkillsForSelect == null) {
                availableSkillsForSelect = new List<int>();
                foreach (var entry in db) {
                    if (Skill.isAvailableForSelect(entry.Key)) {
                        availableSkillsForSelect.Add(entry.Key);
                    }                
                }
            }

            return availableSkillsForSelect;
        }
    }
}
