using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Neon.Aligner
{
    public static class BinaryParser
    {
        public static T[] To<T>(this byte[] byteBuffer)//, double scaleFactor)
        {
            //length of binary block
            int numDigits = int.Parse(Encoding.ASCII.GetString(byteBuffer, 1, 1));
            int indexData = 2 + numDigits;

            int sizeOfData = int.Parse(Encoding.ASCII.GetString(byteBuffer, 2, numDigits));
            var numValue = sizeOfData / Marshal.SizeOf(default(T));
            T[] data = new T[numValue];
            if (numValue > 0) Buffer.BlockCopy(byteBuffer, indexData, data, 0, sizeOfData);
            return data;
        }



    }
}
