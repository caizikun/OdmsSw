using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Free302.MyLibrary.Utility;

namespace Free302.TnM.Device.StageTester
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            catch(Exception ex)
            {
                //MyDebug.ShowErrorMessage($"Main() - 안잡힌 예외", ex);
                MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
