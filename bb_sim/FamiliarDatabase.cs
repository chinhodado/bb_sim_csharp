using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace bb_sim {
    // a simple class that acts like a lazy store of different familiar lists
    static class FamiliarDatabase {
        public static readonly Dictionary<int, CardInfo> famDatabase = new Dictionary<int, CardInfo>();  

        // contains ids of fam in different tiers
        static Dictionary<string, List<int>> tierList;

        // contains ids of all fam
        static List<int> allIdList;

        static List<int> getTierList(string tierToGet, string allTierString) {
            if (tierList == null) {
                // parse and make the whole tier list
                tierList = new Dictionary<string, List<int>>();
                //var allTierList = JSON.parse(allTierString);
                //var tierArray = ["tierX", "tierS+", "tierS", "tierA+", "tierA", "tierB", "tierC"];

                //for (var i = 0; i < tierArray.length; i++) {
                //    var tierNameList = [];
                //    var tier = tierArray[i];

                //    for (var j = 0; j < allTierList[tier].length; j++) {
                //        tierNameList.push(allTierList[tier][j].name);
                //    }

                //    tierList[tier] = new List<int>();
        
                //    for (var key in famDatabase) {
                //        if (famDatabase.hasOwnProperty(key)) {
                //            var name = famDatabase[key].fullName;
                //            if (tierNameList.indexOf(name) != -1) {
                //                this.tierList[tier].push(key);
                //            }
                //        }
                //    }
                //}            
            }

            return tierList[tierToGet];    
        }

        /**
         * Get a list of ids of all familiars, except the warlords
         */
        static List<int> getAllFamiliarList() {
            if (allIdList == null) {
                allIdList = new List<int>();

                //for (var key in famDatabase) {
                //    if (famDatabase.hasOwnProperty(key) && !famDatabase[key].isWarlord) {
                //        this.allIdList.push(key);
                //    }
                //}
            }

            return allIdList;
        }

        public static List<int> getRandomFamList(RandomBrigType type, string allTierString) {
            var tierX = getTierList("tierX", allTierString);
            var tierSP = getTierList("tierS+", allTierString);
            var tierS = getTierList("tierS", allTierString);
            var tierAP = getTierList("tierA+", allTierString);
            var tierA = getTierList("tierA", allTierString);

            switch (type) {
                case RandomBrigType.ALL:
                    return getAllFamiliarList();
                case RandomBrigType.X_ONLY:
                    return tierX;
                case RandomBrigType.SP_ONLY:
                    return tierSP;
                case RandomBrigType.SP_UP:
                    return tierSP.Concat(tierX).ToList();
                case RandomBrigType.S_ONLY:
                    return tierS;
                case RandomBrigType.S_UP:
                    return tierS.Concat(tierSP).Concat(tierX).ToList();
                case RandomBrigType.AP_ONLY:
                    return tierAP;
                case RandomBrigType.AP_UP:
                    return tierAP.Concat(tierS).Concat(tierSP).Concat(tierX).ToList();
                case RandomBrigType.A_ONLY:
                    return tierA;
                case RandomBrigType.A_UP:
                    return tierA.Concat(tierAP).Concat(tierS).Concat(tierSP).Concat(tierX).ToList();
                default:
                    throw new Exception("Invalid brig random type");
            }
        }

        /**
         * Get a list of ids of all warlords
         */

        public static List<int> getWarlordList() {
            return new List<int> {1, 2, 3, 4, 5, 6, 7, 8};
        }
    }

    class CardInfo {
        public string name;
        public string fullName;
        public bool isMounted;
        public bool isWarlord;
        public string img;
        public int[] stats;
        public List<int> skills; 
        public int autoAttack;

        public override string ToString() {
            return fullName;
        }
    }

    class FamDatabase {
        public static Dictionary<int, CardInfo> db;
        public static Dictionary<string, int> nameToId = new Dictionary<string, int>(); 

        static FamDatabase() {
            string fams = File.ReadAllText("cards.json");
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                MissingMemberHandling = MissingMemberHandling.Error
            };

            db = JsonConvert.DeserializeObject<Dictionary<int, CardInfo>>(fams);

            foreach (var entry in db) {
                nameToId[entry.Value.fullName] = entry.Key;
            }
        }

        public static CardInfo get(int id) {
            return db[id];
        }
    }
}
