using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Neon.Aligner
{
    public class Log
    {
        const string mFileName = "error";
        static readonly object mLock = new object();

        public static void Write(string msg)
        {
            Write(msg, mFileName, true);
        }
        public static void Write(string msg, string fileName)
        {
            Write(msg, fileName, true);
        }
        public static void Write(string msg, bool append)
        {
            Write(msg, mFileName, append);
        }

        public static void Write(string msg, string fileName, bool append)
        {
            var dir = @".\log";
            try
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
            catch
            {
                dir = ".";
            }

            var fn = $@"{dir}\{fileName}.txt";

            lock (mLock)
            {
				var id2 = System.Threading.Thread.CurrentThread.ManagedThreadId;
                using (var writer = new StreamWriter(fn, append))
                {
                    var now = DateTime.Now;
                    var time = now.ToString("yyyyMMdd HH:mm:ss");
                    writer.WriteLine($"[{id2:00}] [{time}.{now.Millisecond:000}] {msg}");
                    writer.Close();
                }
            }//lock
        }
    }//class
}
