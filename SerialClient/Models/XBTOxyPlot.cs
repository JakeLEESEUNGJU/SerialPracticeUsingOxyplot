using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SerialClient
{
    public class XBTOxyPlot
    {
        private Convertor convertor = new Convertor();
        private CircularQueue<byte> circularQueue;
        public bool FIndheader { get; set; } = false;
        public int Size;
        readonly byte[] XSV01 = { 0xff, 0xff, 0xff, 0x03 };// 1395 word
        readonly byte[] XSV02 = { 0xff, 0xff, 0xff, 0x06 };//3281
        readonly byte[] XBT04 = { 0x00, 0x00, 0x00, 0x04 };//750
        readonly byte[] XBT05 = { 0x00, 0x00, 0x00, 0x05 };//3000
        readonly byte[] XBT07 = { 0x00, 0x00, 0x00, 0x07 };//1250
        readonly byte[] XBT10 = { 0x00, 0x00, 0x00, 0x02 };//330
        string SubTitle { get; set; }

        public XBTOxyPlot()
        {
            circularQueue = new CircularQueue<byte>();
        }
        public async Task<bool> SearchHeader()
        {
            while (FIndheader == false && circularQueue.Count != 0)
            {
                if (FIndheader == true) return true;

                var IsHeader = Task<byte[]>.Factory.StartNew(() => circularQueue.Peek(4));
                await IsHeader;
                var header = IsHeader.Result;

                if (header.SequenceEqual(XSV01))
                {
                    Size = 1395;
                    //circularQueue = new CircularQueue<byte>(8000);
                    SubTitle = "XSV01";
                    FIndheader = true;
                    return true;
                }
                if (header.SequenceEqual(XSV02))
                {

                    Size = 3281;

                   // circularQueue = new CircularQueue<byte>(30000);
                    SubTitle = "XSV02";
                    FIndheader = true;
                    return true;
                }
                if (header.SequenceEqual(XBT04))
                {
                    Size = 750;
                    //circularQueue = new CircularQueue<byte>(6000);
                    SubTitle = "XBT04";
                    FIndheader = true;
                    return true;
                }
                if (header.SequenceEqual(XBT05))
                {
                    Size = 3000;
                    //circularQueue = new CircularQueue<byte>(25000);
                    SubTitle = "XBT05";
                    FIndheader = true;
                    return true;
                }
                if (header.SequenceEqual(XBT07))
                {
                    Size = 1250;
                  //  circularQueue = new CircularQueue<byte>(8000);
                    SubTitle = "XBT07";
                    FIndheader = true;
                    return true;
                }
                if (header.SequenceEqual(XBT10))
                {
                    Size = 330;
                    //circularQueue = new CircularQueue<byte>(Size);
                    SubTitle = "XBT10";
                    FIndheader = true;
                    return true;
                }
                circularQueue.Get(4);
            }
            return true;
        }


        public void write(byte[] buf)
        {

            circularQueue.Write(buf);

        }

        public List<DataPoint> GetChart()
        {
            var chartData = circularQueue.Get(Size * 4 + 4);
            var intData = convertor.HextoDec(chartData);
            var TND = convertor.SortArray(intData);
            FIndheader = false;
            return TND;
            /*var result = CreateDataPoints(TND);
            FIndheader = false;
            return result;
            */
        }
        public PlotModel CreatePlotModel(List<double> x, List<double> y)
        {
            var model = new PlotModel() { Title = "XBTChart" };
            if (SubTitle != null) model.Subtitle = SubTitle;
            
           // var h =CreateDataPoints(convertor.TemperNDepth);
            //var verticalAxis = new LinearAxis { Position = AxisPosition.Left, Minimum = min, Maximum = max };
            //model.Axes.Add(verticalAxis);
            //model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
            //model.Series.Add(new FunctionSeries(x => Math.Sin(x * Math.PI * 4) * Math.Sin(x * Math.PI * 4) * Math.Sqrt(x) * max, 0, 1, 1000));
            return model;
        }

        /*
        private Tuple<List<double>,List<double>> CreateDataPoints(List<Tuple<double, double>> temperNDepth)
        {

            foreach (var tuple in temperNDepth)
            {
                temp.Add(tuple.Item1);
                depth.Add(tuple.Item2);
            }
            var TND = Tuple.Create(temp, depth);
            return TND;

        }
        */
    }
}


