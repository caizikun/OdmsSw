using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Neon.Aligner
{
    public class SantecFilter
    {
        const string mScriptFile = "SantecFilter.py";
        const string mScriptFile2 = "FilterForSantecData.py";
        static string mScriptPath = Path.Combine(Application.StartupPath, mScriptFile);
        static string mScriptPath2 = Path.Combine(Application.StartupPath, mScriptFile2);
        static object mLock = new object();

        public static void ApplyFilter(string filePath)
        {
            var dir = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var script = Path.Combine(dir, mScriptFile);

            lock (mLock)
            {
                if (!File.Exists(script)) File.Copy(mScriptPath, script);
            }

            Process cmd = new Process();
            //cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.FileName = "python.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;

            //cmd.StartInfo.Arguments = $" /k \"cd {dir}\"";
            cmd.StartInfo.Arguments = $" \"{mScriptPath}\" \"{filePath}\"";
            cmd.Start();

            //cmd.StandardInput.WriteLine($"python.exe {mScriptPath} {fileName}");
            //cmd.StandardInput.WriteLine("exit");
            //cmd.StandardInput.Flush();
            cmd.StandardInput.Close();

            var msgOut = cmd.StandardOutput.ReadToEnd();
            var msgError = cmd.StandardError.ReadToEnd();
            cmd.WaitForExit();

            if (msgError.Length > 0) Log.Write($"SantecFilter.ApplyFilter({fileName}):\n{msgError}", true);
        }



        public static void ApplyFilterFolder(string _path)
        {
            var script = Path.Combine(_path, mScriptFile2);

            lock (mLock)
            {
                if (!File.Exists(script)) File.Copy(mScriptPath2, script);
            }

            Process cmd = new Process();
            cmd.StartInfo.FileName = script;
            cmd.StartInfo.WorkingDirectory = _path;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            cmd.Start();

            cmd.WaitForExit();

        }


    }//class
}
