using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialClient.Models
{
    public class ByteCollector
    {

        private readonly byte[] XSV01 = { 0xff, 0xff, 0xff, 0x03 };
        private readonly byte[] XSV02 = { 0xff, 0xff, 0xff, 0x06 };
        private readonly byte[] XBT04 = { 0x00, 0x00, 0x00, 0x04 };
        private readonly byte[] XBT05 = { 0x00, 0x00, 0x00, 0x05 };
        private readonly byte[] XBT07 = { 0x00, 0x00, 0x00, 0x07 };
        private readonly byte[] XBT10 = { 0x00, 0x00, 0x00, 0x02 };


        public Queue<byte> buff;



        void Collect(IList<char> arr)
        {
            if (buff == null)
            {
                buff = new Queue<byte>();
            }
            foreach (var item in arr)
            {
            buff.Enqueue((byte)item);
            }
            
            var header = buff.Take(4).ToArray();
        }



    }
}
