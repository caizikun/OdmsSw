using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Neon.Aligner
{
    class SerialNumber
    {

        public SerialNumber(string filePath)
        {
            var fn = Path.GetFileNameWithoutExtension(filePath);
            var codes = fn.Split('-');

            FilePath = filePath;
            Wafer = codes[0];
            Bar = $"{codes[1]}-{codes[2]}";
            Chip = codes[3];
            
        }

        public string FilePath;
        public string Wafer;
        public string Bar;
        public string Chip;

        public string WaferBar { get { return $"{Wafer}-{Bar}"; }  }


        public string ChipName { get { return $"{Bar}-{Chip}"; } }


        public override bool Equals(object obj)
        {
            return this.WaferBar.Equals(((SerialNumber)obj).WaferBar);
        }


        public override int GetHashCode()
        {
            return this.WaferBar.GetHashCode();
        }


    }//class
}
