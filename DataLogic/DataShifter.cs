using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Neon.Aligner
{
    class DataShifter
    {
        public static void runShiftFile(string filePath, double shift, bool doBackup)
        {
            var sb = new StringBuilder();
            readFile(filePath, shift, sb);
            saveFile(filePath, sb, doBackup);
        }

        private static void readFile(string filePath, double shift, StringBuilder sb)
        {
            using (var sr = new StreamReader(filePath))
            {
                while (true)
                {
                    if (sr.EndOfStream) break;

                    var line = sr.ReadLine();
                    if (line == null) break;

                    var values = line.Split(new char[] { ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length < 1) break;

                    sb.Append($"{values[0]}, ");
                    for (int i = 1; i < values.Length; i++)
                    {
                        var value = Math.Round(shift + double.Parse(values[i]), 3);
                        sb.Append($"{value:F03}, ");
                    }
                    sb.Remove(sb.Length - 2, 2);
                    sb.AppendLine();
                }
                sr.Close();
            }
        }

        private static void saveFile(string filePath, StringBuilder sb, bool doBackup)
        {
            if (doBackup) DataBackup.BackupFile(filePath);

            using (var sw = new StreamWriter(filePath))
            {
                sw.Write(sb.ToString());
                sw.Close();
            }
        }
        
    }//class
}
