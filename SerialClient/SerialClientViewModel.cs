using CP.IO.Ports;
using Infrastructure;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LinearAxis = OxyPlot.Axes.LinearAxis;

namespace SerialClient
{
    public static class SerialConstants
    {
        public static readonly Parity[] Parities = (Parity[])Enum.GetValues(typeof(Parity));
        public static readonly Handshake[] Handshakes = (Handshake[])Enum.GetValues(typeof(Handshake));
        public static readonly int[] DataBits = new int[] { 8, 7, 6 };
        public static readonly StopBits[] StopBits = (StopBits[])Enum.GetValues(typeof(StopBits));

        public static readonly int[] Baudrates = new int[] { 300, 600, 1200, 1800, 2400, 4800, 7200, 9600, 14400, 19200, 38400, 57600, 115200, 230400, 460800, 921600 };
    }
    public class SerialClientViewModel : BindableBase
    {


       


        #region structor
        private SerialConfiguration configuration = new SerialConfiguration()
        {
            PortName = SerialPort.GetPortNames().FirstOrDefault() ?? "",
            DataBits = 8,
            handshake = Handshake.None,
            Parity = Parity.None,
            BaudRate = 115200,
            stopBits = StopBits.One
        };
        #endregion


        #region Property
        private Serial serial = new Serial();
        CompositeDisposable disposable = new CompositeDisposable();
        public string ReceiveTB { get; set; } = "";
        public string TempTB { get; set; } = "";
        public int[] SerialDataBits { get { return SerialConstants.DataBits; } }
        public int DataBits { get { return configuration.DataBits; } set { SetProperty(ref configuration.DataBits, value); } }
        public StopBits[] SerialStopBits { get { return SerialConstants.StopBits; } }
        public StopBits StopBits { get { return configuration.stopBits; } set { SetProperty(ref configuration.stopBits, value); } }
        public int[] Baudrates { get { return SerialConstants.Baudrates; } }
        public int Baudrate { get { return configuration.BaudRate; } set { SetProperty(ref configuration.BaudRate, value); } }
        public string[] Ports { get { return SerialPort.GetPortNames(); } }
        public string Port { get { return configuration.PortName; } set { SetProperty(ref configuration.PortName, value); } }
        public Handshake[] Handshakes { get { return SerialConstants.Handshakes; } }
        public Handshake Handshake { get { return configuration.handshake; } set { SetProperty(ref configuration.handshake, value); } }
        public Parity[] Parities { get { return SerialConstants.Parities; } }
        public Parity ParityBit { get { return configuration.Parity; } set { SetProperty(ref configuration.Parity, value); } }

        public DelegateCommand SendCommand { get; }
        public DelegateCommand ConnectionCommand { get; }
        #endregion Property


        #region method
     
        private IDisposable ReceiveChecker = null;

        private void UpdateChecker()
        {
            if (ReceiveChecker != null)
            {
                ReceiveChecker.Dispose();
                ReceiveChecker = null;
            }
            ReceiveChecker = serial.DataReceived
                .Timeout(TimeSpan.FromSeconds(3))

                .Subscribe(_ => { }, _ =>
                {
                    IsConnected = serial.IsOpen();
                    if (IsConnected)
                    {
                        serial.Send(hex);
                        IsOn = false;
                        ReceiveChecker.Dispose();
                    }
                });
        }
        public bool IsOn;
        public ObservableCollection<DataPoint> Data { get; set; } = new ObservableCollection<DataPoint>();
        public bool FindHeader { get; set; } = false;

        public List<double> Temperature { get; set; }
        public List<double> Depth { get; set; }
        private void OpenServer()
        {

            IsConnected = !IsConnected;
            ConnectStatus = $"연결 {(!IsConnected ? "시작" : "중지")}";
            RaisePropertyChanged(nameof(ConnectStatus));
            if (IsConnected == false)
            {
                receiverSubscription?.Dispose();
                return;
            }
            //serial.
            // using(Serial serial = new Serial())
            //{


            if (senderSubscription != null) senderSubscription.Dispose();
            if (receiverSubscription != null) receiverSubscription.Dispose();
            var startChar = (0x02).AsObservable();
            var endChar = (0x03).AsObservable();
            serial.OpenSerial(configuration);
            receiverSubscription = serial
                .DataReceived1()
                .Subscribe(v => {
                    //                    var buf = v.Cast<byte>().ToArray();
                    var buf = Encoding.UTF8.GetBytes(v.ToArray());


                    Oxy.write(buf);

                    var IsHeader = Oxy.SearchHeader();
                    if (IsHeader.Result == true)
                    {
                        Data = new ObservableCollection<DataPoint>(Oxy.GetChart());
                        RaisePropertyChanged(nameof(Data));
                        //Temperature = chart.Item1;
                        //Depth = chart.Item2;
                        //RaisePropertyChanged(nameof(Temperature));
                        //RaisePropertyChanged(nameof(Depth));
                    }



                    //if (IsOn == false)
                    //{
                    //    serial.Send(hex);

                    //    IsOn = true;
                    //    UpdateChecker();
                    //}
                    //byte[] source = v.Select(o => (byte)o).ToArray();
                    //    byte checksum = source[source.Length - 1];
                    //var data = source.Skip(1).Take(source.Length - 2).ToArray();
                    ////var checksum2 = (byte)(data.Aggregate(0,(c, d) => c + d) & 0xFF);
                    //var Checksum = new CheckSum();
                    //var result = Checksum.GetCheckSum(checksum, data);
                    //var bytetostring = BitConverter.ToString(source);
                    //ReceiveTB += "받은 데이터 : "+bytetostring + " 체크섬 : " + result.Item1 + " bool "+ result.Item2 + "  \n" ;
                    //if (ReceiveTB.Length >= 3000)
                    //{
                    //    ReceiveTB = "받은 데이터 : " + bytetostring + " 체크섬 : " + result.Item1 + " bool " + result.Item2 + "  \n"; 
                    //}
                    //RaisePropertyChanged(nameof(ReceiveTB));
                });
          
            //serial.RecvDataList
            //blahblah
            //}
        }

     
        private IDisposable senderSubscription = null;
        private IDisposable receiverSubscription = null;


        public string ConnectStatus { get; set; } = "연결 시작";
        public bool IsConnected { get; set; } = false;
        public bool IsSend { get; set; } = false;
        public string SendingStatus { get; set; } = "전송시작";

        private readonly byte[] hex = new byte[] { 0x02, 0x00, 0x13, 0x02, 0x21, 0x22, 0x33, 0x32, 0x00, 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x1e, 0x00, 0x03 };
        private void SendSerial()
        {
            IsSend = !IsSend;
            SendingStatus = $"전송 {(!IsSend ? "시작" : "중지")}";
            serial.Send(hex);
            RaisePropertyChanged(nameof(IsSend));
            RaisePropertyChanged(nameof(SendingStatus));
        }
        public void Dispose()
        {
            disposable.Dispose();
        }

        #endregion method


        /////
        ///
        /////




    public SerialClientViewModel()
        {
            
        SendCommand = new DelegateCommand(SendSerial);
        ConnectionCommand = new DelegateCommand(OpenServer);
        RaisePropertyChanged(nameof(Ports));
    }

        


        XBTOxyPlot Oxy = new XBTOxyPlot();
    }

}

