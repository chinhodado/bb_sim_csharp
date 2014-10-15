using System;
using System.Windows.Forms;

namespace bb_sim {
    static class Program {
        /// <summary>
        /// The main entry point for the application. Based on 168e760
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
