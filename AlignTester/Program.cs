using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Neon.Aligner;

namespace AlignTester
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Neon.Aligner.Ca3000.Test();
            //AlignLogic.Test_NextChip();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new PcForm());
            //Application.Run(new Form1());
            //Application.Run(new DataControlForm());
            //Application.Run(new TestForm());

            Application.Run(new Neon.Aligner.Test.TestForm());
        }
    }
}
