using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialConnectPractice.Model
{
    public class CheckSum
    {
        public (bool, int) GetCheckSum(Int32 checksum, byte[] data)
        {
            Int32 checksum2 = 0;
            foreach (Byte ch in data)
            {
                checksum2 += (byte)ch;                      // checkSum 계산
            }
            var total = checksum2;
            total = total & 0xFF;
            total = ~total + 1;
            total = total + checksum2;
            total = total & 0xff;

            var IsSame = total == 0;

            return (IsSame, total);
        }
    }
}
