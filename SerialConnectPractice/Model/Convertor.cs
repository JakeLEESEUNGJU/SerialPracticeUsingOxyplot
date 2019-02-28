using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialConnectPractice.Model
{
    public class Convertor
    {
        public bool CheckSum()
        {
            return false;
        }
        public byte[] CreatePacket(byte[] data)
        {
            /* 
             * Packet 구조
             * [STX:0x02(1byte)] [Data Length(1byte)] [DATA+0] [DATA+1] .... [DATA+n] [CheckSum(1byte)] [ETX:0x03(1byte)]
             * 
             * Check sum 계산
             * ([Data+0]+[Data+1]+[Data+2] .... +[Data+n]) & 0xFF
             */

            Int32 checkSum = 0;
            List<byte> SendDataList = new List<byte>();
            // 패킷 데이터 채우기
            SendDataList.Add(0x02);                      // STX
            SendDataList.Add((byte)data.Length);            // Data의 길이. 최대 255. 그 이상 할려면 Packet 구조 변경이 필요
            foreach (Byte ch in data)
            {
                SendDataList.Add((Byte)ch);                // string 값 차례대로 넣기
                checkSum += (Byte)ch;                      // checkSum 계산
            }
            SendDataList.Add((Byte)(checkSum & 0xFF));     // CheckSum 1byte. 전체 중 마지막 8bit값만 취함
            SendDataList.Add(0x03);                        // ETX

            return SendDataList.ToArray();       // 길이 반환
        }
        //public List<Byte> CreatePacket(String msg, List<Byte> SendDataList)
        //{
        //    /* 
        //     * Packet 구조
        //     * [STX:0x02(1byte)] [Data Length(1byte)] [DATA+0] [DATA+1] .... [DATA+n] [CheckSum(1byte)] [ETX:0x03(1byte)]
        //     * 
        //     * Check sum 계산
        //     * ([Data+0]+[Data+1]+[Data+2] .... +[Data+n]) & 0xFF
        //     */
        //    Int32 checkSum = 0;

        //    // 패킷 데이터 채우기
        //    SendDataList.Add(0x02);                        // STX
        //    SendDataList.Add((Byte)msg.Length);            // Data의 길이. 최대 255. 그 이상 할려면 Packet 구조 변경이 필요
        //    foreach (Byte ch in msg)
        //    {
        //        SendDataList.Add((Byte)ch);                // string 값 차례대로 넣기
        //        checkSum += (Byte)ch;                      // checkSum 계산
        //    }
        //    SendDataList.Add((Byte)(checkSum & 0xFF));     // CheckSum 1byte. 전체 중 마지막 8bit값만 취함
        //    SendDataList.Add(0x03);                        // ETX

        //    return SendDataList;       // 길이 반환
        //}


    }
}
