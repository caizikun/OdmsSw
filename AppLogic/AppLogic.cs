using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace Neon.Aligner
{
    public class AppLogic
    {
        #region ---- License ----

        //License
        public class LicenseInfo
        {
            public string Version;

            public string SN;
            public string Date;

            public bool ShowDevUI = false;
            public bool ShowBudMenu = true;
            public bool IsHSB = false;//bonding system
        }

        public static LicenseInfo License;

        static AppLogic()
        {
            License = new LicenseInfo();
        } 
        #endregion



        #region ---- Help File ----

        const string _SystemManualFile = @"help\ODMAS User’s Manual Eng V0.5.pdf";
        const string _MeasureOperationFile = @"help\181126_测试作业说明书.pdf";
        public static void ShowManual(int manIndex)
        {
            var file = manIndex == 1 ? _SystemManualFile : _MeasureOperationFile;
            _openFile(file);
        }
        static void _openFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Manual file not found ({filePath}).");
                return;
            }
            using (var cmd = new Process())
            {
                cmd.StartInfo.FileName = filePath;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = true;
                cmd.StartInfo.Verb = "open";
                cmd.Start();
            }
        }

        #endregion


        public static string GetProductAndTime(Assembly assembly)
        {
            var n = assembly.GetName();
            var v = n.Version;
            var vs = $"V{v.Revision}({v.Major:D04}.{v.Minor / 100:D02}.{v.Minor % 100:D02})";
            //var vs = $"{v.Major:D04}.{v.Minor / 100:D02}.{v.Minor % 100:D02} {v.Build / 100:D02}:{v.Build % 100:D02} B{v.Revision}";
            License.Version = $"{n.Name} {vs}";
            return License.Version;
        }

    }
}
