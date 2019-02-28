using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialClient
{
    public class Convertor
    {
        //XBT File;

        public int[] HextoDec(byte[] XBT)
        {
            int[] Data = new int[XBT.Length];
            for (int i = 0; i < XBT.Length; i++)
            {
                Data[i] = XBT[i];
            }
            return Data;
        }
        public List<DataPoint> TemperNDepth = new List<DataPoint>();

        

        int Temper = 0, Depth = 0;
        public List<DataPoint> SortArray(int[] intData)
        {
            int count=1;

            for (int i = 0; i < intData.Length; i++)
            {
                if (count == 32)
                {
                    Calculate(Temper, Depth);
                    count = 1;
                    continue;
                }
                if(count%8==0||count==14|| count == 15|| count == 16|| count == 30||count == 31) {
                    count++;
                        continue;
                }
                if (count < 16)
                {
                    Temper += intData[i];
                }else if (count > 16)
                {
                    Depth += intData[i];
                }
                count++;
            }
            return TemperNDepth;
        }

        private void Calculate(int temper, int depth)
        {
               
             var TemperatureData = ((temper / 4095 * 72 - 6.0) * 5 / 9);
            TemperatureData= Math.Round(TemperatureData, 2);
            var DepthData = (depth * 0.3048 * 2);
            DepthData = Math.Round(DepthData, 1);
            TemperNDepth.Add(new DataPoint(TemperatureData, DepthData));

        }
    }
}
