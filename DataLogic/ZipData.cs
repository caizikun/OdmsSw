using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//using System.IO.Compression;
using Ionic.Zip;

namespace Neon.Aligner
{
    public class ZipData
    {
        public static bool UsingZip = SecurityControl.DoShift;

        static byte[] mBytes = {
            168, 255, 201, 255, 152, 255, 177, 255, 143, 255, 167, 255, 170, 255,
            147, 255, 156, 255, 217, 255, 158, 255, 146, 255, 143, 255, 196, 255,
            222, 255, 157, 255, 135, 255, 184, 255, 218, 255, 144, 255, 144, 255,
            189, 255, 191, 255, 183, 255
        };

        public static void Save(string filePath, StringBuilder sb)
        {
            var dir = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileName(filePath);
            var fn = fileName.Split('-');
            var bar = fn[0] + "-" + fn[1] + "-" + fn[2];
            var zipFileName = dir + $@"\{bar}.7z";

            var pwd = buildPwd();

            using (var mZip = new ZipFile(zipFileName))
            {
                mZip.Password = pwd;
                mZip.UpdateEntry(fileName, sb.ToString());
                mZip.Save();
            }
        }

        public static string buildPwd()
        {
            var bytes = new byte[mBytes.Length];
            for (int i = 0; i < bytes.Length; i++) bytes[i] = (byte)(mBytes[i] ^ 0xFF);
            var pwd = Encoding.Unicode.GetString(bytes);
            return pwd;
        }

        /// <summary>
        /// extract and return file list
        /// </summary>
        /// <param name="zipFilePath"></param>
        /// <returns></returns>
        public static string[] Extract(string zipFilePath, string dir)
        {
            var unzipdir = getDir(zipFilePath, dir);
            string[] files;
            using (var zip = new ZipFile(zipFilePath))
            {
                zip.Password = buildPwd();
                zip.ExtractAll(unzipdir, ExtractExistingFileAction.OverwriteSilently);

                files = zip.EntryFileNames.Select(f => Path.Combine(unzipdir, f)).ToArray();
            }

            return files;
        }

        public static int NumFiles(string zipFilePath)
        {
            var zip = new ZipFile(zipFilePath);
            return zip.Entries.Count;
        }

        static string getDir(string zipFilePath, string dir)
        {
            var fn = Path.GetFileNameWithoutExtension(zipFilePath);

            bool saveTextFile = dir != null;

            if (!saveTextFile)
            {
                dir = Path.GetTempPath();
                var wafer = fn.Split('-')[0];
                var time = DateTime.Now.ToString("yyMMdd_HHmmss");
                dir = Path.Combine(dir, $"{wafer}_{time}");
            }

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return dir;
        }

    }//class
}
