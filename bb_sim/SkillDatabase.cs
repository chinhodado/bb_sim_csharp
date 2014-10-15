using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace bb_sim {
    class SkillInfo {
        public string name;
        public SkillType type;
        public SkillFunc func;
        public SkillCalcType calc;
        public double arg1;
        public double arg2;
        public double arg3;
        public double arg4;
        public double arg5;
        public SkillRange range;
        public int prob;
        public WardType ward;
        public bool isAutoAttack;
        public string desc;
        public List<int> randSkills; 
    }

    class SkillDatabase {
        private static readonly Dictionary<int, SkillInfo> db;
        static List<int> availableSkillsForSelect;

        static SkillDatabase()  {
            string skills = File.ReadAllText("skills.json");
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                MissingMemberHandling = MissingMemberHandling.Error
            };

            db = JsonConvert.DeserializeObject<Dictionary<int, SkillInfo>>(skills);
        }

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
