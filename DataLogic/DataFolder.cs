using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Neon.Aligner
{
    class DataFolder
    {
        /// <summary>
        /// create wafer folder in saveFolder 
        /// </summary>
        /// <param name="saveFolder"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetWaferFolder(string saveFolder, string filePath)
        {
            var folder = saveFolder;
            try
            {
                var fn = Path.GetFileName(filePath);
                var wafer = fn.Split('-')[0];
                folder = Path.Combine(saveFolder, wafer);
                Directory.CreateDirectory(folder);
                return folder;
            }
            catch
            {
                return saveFolder;
            }
        }
    }//class
}
