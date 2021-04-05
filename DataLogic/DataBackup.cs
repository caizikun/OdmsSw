using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Neon.Aligner
{
    class DataBackup
    {
        static string mBackupDir = "backup";
        static readonly object mLock = new object();


        public static void BackupFile(string filePath, string folderName = null)
        {
            string dir = getBackupDir(filePath, folderName);
            var backupFilePath = Path.Combine(dir, Path.GetFileName(filePath));
            if (File.Exists(backupFilePath))
            {
                var fn = Path.GetFileNameWithoutExtension(backupFilePath);
                var time = DateTime.Now.ToString("_yyMMdd_HHmmss");
                backupFilePath = Path.Combine(dir, $"{fn}{time}.txt");
            }
            File.Move(filePath, backupFilePath);
        }


        static string getBackupDir(string filePath, string folderName = null)
        {
            var dir = Path.GetDirectoryName(filePath);
            try
            {
                var backupDir = Path.Combine(Path.GetDirectoryName(filePath), folderName?? mBackupDir);
                if (Directory.Exists(backupDir)) return backupDir;

                lock (mLock)
                {
                    Directory.CreateDirectory(backupDir);
                }
                return backupDir;
            }
            catch
            {
                return dir;
            }
        }

    }//class
}
