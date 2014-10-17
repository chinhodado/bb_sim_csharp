using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace bb_sim {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            for (int i = 1; i <= 2; i++) {
                for (int j = 0; j < 5; j++) {
                    var comboBox = (ComboBox)Controls["p" + i + "f" + j];
                    comboBox.DataSource = new BindingSource(FamDatabase.db, null);
                    comboBox.DisplayMember = "Value";
                    comboBox.ValueMember = "Key"; 
                }
            }
        }

        public Dictionary<string, List<int>> getTierListDictionary() {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            string json = new WebClient().DownloadString("https://www.kimonolabs.com/api/cs76o38w?apikey=ddafaf08128df7d12e4e0f8e044d2372");
            var foo = JsonConvert.DeserializeObject<KimonoJson>(json);
            var tierDict = new Dictionary<string, List<int>>();
            foreach (var entry in foo.results) {
                var tmpFamList = new List<int>();
                tierDict[entry.Key] = tmpFamList;
                foreach (var fam in entry.Value) {
                    try {
                        tmpFamList.Add(FamDatabase.nameToId[fam.name]);
                    }
                    catch (Exception e) {
                        Debug.WriteLine(fam.name);
                    }
                }
            }
            return tierDict;
        }

        public void playGame() {
            p1f0.DataSource = new BindingSource(FamDatabase.db, null);
            p1f0.DisplayMember = "Value";
            p1f0.ValueMember = "Key";

            var cards1 = new List<int>();
            var cards2 = new List<int>();
            for (int i = 0; i < 5; i++) {
                var cb1 = (ComboBox)Controls["p1f" + i];
                var cb2 = (ComboBox)Controls["p2f" + i];
                cards1.Add(((KeyValuePair<int, CardInfo>)cb1.SelectedItem).Key);
                cards2.Add(((KeyValuePair<int, CardInfo>)cb2.SelectedItem).Key);
            }

            GameData data = new GameData {
                p1_cardIds = cards1,
                p2_cardIds = cards2,
                p1_formation = FormationType.MID_5,
                p2_formation = FormationType.MID_5,
                p1_warlordSkillIds = null,
                p2_warlordSkillIds = null
            };

            GameOption option = new GameOption {
                battleType = BattleType.NORMAL,
                p1RandomMode = RandomBrigType.ALL,
                p2RandomMode = RandomBrigType.ALL,
                procOrder = ProcOrderType.ANDROID
            };

            int p1won = 0, p2won = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();

            for (var i = 0; i < 10000; i++) {
                BattleModel newBattle = new BattleModel(data, option, "");
                var won = newBattle.startBattle();
                BattleModel.resetAll();

                if (won == 1) {
                    p1won++;
                }
                else if (won == 2) {
                    p2won++;
                }
            }

            sw.Stop();

            Debug.WriteLine("Elapsed = {0}", sw.Elapsed);
            Debug.WriteLine("p2: " + p2won);
            Debug.WriteLine("p1: " + p1won);
        }

        private void startButton_Click(object sender, EventArgs e) {
            Enabled = false;
            playGame();
            Enabled = true;
        }
    }

    public class FamJson {
        public string name;
    }

    public class KimonoJson {
        public Dictionary<string, List<FamJson>> results;
    }
}
