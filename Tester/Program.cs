using Neon.Aligner;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Tester
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }


        public static void InitOsw(decimal com, decimal als, decimal tls, bool isNeon)
        {
            var osw1 = new Osw((int)als, (int)tls, isNeon);
            osw1.Connect((int)com);
            _serial = osw1.Serial;
        }

        static SerialPort _serial;

        public static void InitSerial(decimal com, decimal bps, string newLine)
        {
            SerialPort mSerial = new SerialPort();

            mSerial.PortName = $"COM{com}";
            mSerial.BaudRate = (int)bps;

            mSerial.Handshake = Handshake.RequestToSend;

            mSerial.DataBits = 8;
            mSerial.StopBits = StopBits.One;
            mSerial.Parity = Parity.None;
            mSerial.ReadTimeout = 5000;
            mSerial.NewLine = newLine;
            mSerial.Open();

            _serial = mSerial;
        }


        public static  void write(string msg, bool removeNL)
        {
            //Send~
            _serial.DiscardInBuffer();
            var byteBuffer = Encoding.ASCII.GetBytes($"{msg}{(removeNL ? "" : _serial.NewLine)}");
            _serial.Write(byteBuffer, 0, byteBuffer.Length);

            Thread.Sleep(100);
        }

        public static string readString()
        {
            var byteBuffer = new byte[_serial.BytesToRead];
            _serial.Read(byteBuffer, 0, _serial.BytesToRead);
            var strResponse = Encoding.ASCII.GetString(byteBuffer).Trim();
            _serial.DiscardInBuffer();
            return strResponse;
        }

        public static string query(string cmd, bool removeNL)
        {
            write(cmd, removeNL);
            var response = readString();
            return response;
        }

    }//class
}
