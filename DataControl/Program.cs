using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neon.Aligner;
using System.Windows.Forms;

namespace DataControlUi
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
			//NoiseShifter.Test();


            var f = new DataControlForm();
            Application.Run(f);
        }
    }
}
