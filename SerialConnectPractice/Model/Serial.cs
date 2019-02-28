using CP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace SerialConnectPractice.Model
{
    public class Serial : IDisposable
    {
        #region 생성자

        public Serial()
        {
            SerialPort = new SerialPortRx();
        }

        #endregion 생성자

        #region Property

        private SerialPortRx SerialPort { get; set; }

        public IObservable<char> DataReceived => SerialPort.DataReceived;

        #endregion Property

        #region Method
        public bool IsOpen()
        {
            return SerialPort.IsOpen;
        }
        public bool OpenSerial(SerialConfiguration serialConfiguration)
        {
            if (SerialPort.IsOpen) SerialPort.Close();

            SerialPort.PortName = serialConfiguration.PortName;
            SerialPort.BaudRate = serialConfiguration.BaudRate;
            SerialPort.DataBits = serialConfiguration.DataBits;
            SerialPort.Parity = serialConfiguration.Parity;
            SerialPort.Handshake = serialConfiguration.handshake;
            SerialPort.StopBits = serialConfiguration.stopBits;
            SerialPort.Open();
            return SerialPort.IsOpen;
        }
        public IObservable<byte[]> BufferUntilByte(IObservable<char> startsWith, IObservable<char> endsWith, int timeOut = 100)
        {
            return Observable.Create<byte[]>(o => {
                var dis = new CompositeDisposable();
                var buf = new List<byte>();

                var startFound = false;
                var elapsedTime = 0;
                var startsWithL = ' ';
                dis.Add(startsWith.Subscribe(sw =>
                {
                    startsWithL = sw;
                    elapsedTime = 0;
                }));
                var endsWithL = ' ';
                var ewd = endsWith.Subscribe(ew => endsWithL = ew);
                dis.Add(ewd);
                var sub = DataReceived.Subscribe(s => {
                    elapsedTime = 0;
                    if (startFound || s == startsWithL)
                    {
                        startFound = true;
                        if(s != endsWithL && s != startsWithL) buf.Add((byte)s);
                        if (s == endsWithL)
                        {
                            o.OnNext(buf.ToArray());
                            startFound = false;
                            buf.Clear();
                        }
                    }
                });
                dis.Add(sub);
                dis.Add(Observable.Interval(TimeSpan.FromMilliseconds(1)).Subscribe(_ => {
                    elapsedTime++;
                    if (elapsedTime > timeOut)
                    {
                        startFound = false;
                        buf.Clear();
                        elapsedTime = 0;
                    }
                }));

                return dis;
            });
        }
        public IObservable<string> BufferUntil(IObservable<char> startWith, IObservable<char> endWith, int timeOut = 100)
        {
            return SerialPort.DataReceived.BufferUntil(startWith, endWith, timeOut);
        }
        public IObservable<bool> WhileIsOpen(TimeSpan timeSpan)
        {
            return SerialPort.WhileIsOpen(timeSpan);
        }

        public void Send(char[] data, int offset, int count)
        {
            SerialPort.Write(data, offset, count);
        }
        public void Send(byte[] data, int offset, int count)
        {
            SerialPort.Write(data, offset, count);
        }
        public void Send(string data)
        {
            SerialPort.Write(data);
        }

        public void Send(byte[] data)
        {
            SerialPort.Write(data);
        }
        public void Send(char[] data)
        {
            SerialPort.Write(data);
        }

        public void Dispose()
        {
            if (SerialPort.IsOpen) SerialPort.Close();
            SerialPort?.Dispose();
        }

        /////////////
        /////
        /// <summary>
        ///
        ///
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


       

        #endregion Method
    }
}