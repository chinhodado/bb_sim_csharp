using System.Collections.Generic;
using System.Windows.Forms;

namespace bb_sim {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            for (int i = 1; i < 2; i++) {
                for (int j = 0; j < 5; j++) {
                    var comboBox = (ComboBox) Controls["p" + i + "f" + "j"];
                    comboBox.DataSource = new BindingSource(FamDatabase.db, null);
                    comboBox.DisplayMember = "Value";
                    comboBox.ValueMember = "Key"; 
                }
            }
            //p1f0.DataSource = new BindingSource(FamDatabase.db, null);
            //p1f0.DisplayMember = "Value";
            //p1f0.ValueMember = "Key"; 
            //GameData data = new GameData {
            //    p1_cardIds = new List<int> { 10088, 10704, 11168, 11099, 10936 },
            //    p2_cardIds = new List<int> { 10571, 21312, 11314, 11038, 11093 },
            //    p1_formation = FormationType.MID_5,
            //    p2_formation = FormationType.MID_5,
            //    p1_warlordSkillIds = null,
            //    p2_warlordSkillIds = null
            //};

            //GameOption option = new GameOption {
            //    battleType = BattleType.NORMAL,
            //    p1RandomMode = RandomBrigType.ALL,
            //    p2RandomMode = RandomBrigType.ALL,
            //    procOrder = ProcOrderType.ANDROID
            //};

            //int p1won = 0, p2won = 0;
            //for (var i = 0; i < 10000; i++) {
            //    BattleModel newBattle = new BattleModel(data, option, "");
            //    var won = newBattle.startBattle();
            //    BattleModel.resetAll();

            //    if (won == 1) {
            //        p1won++;
            //    }
            //    else if (won == 2) {
            //        p2won++;
            //    }
            //}
            
        }
    }
}
