using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace MuteButton
{
    public delegate void ProcessMessage(string message);
    public class Arduino
    {
        private readonly Lazy<SerialPort> _lazyPort;
        private readonly AppSettings settings;

        private SerialPort Port => _lazyPort.Value;

        public event ProcessMessage ProcessMessage;

        public Arduino(AppSettings settings)
        {
            _lazyPort = new Lazy<SerialPort>(() =>
            {
                var port = new SerialPort
                {
                    PortName = settings.Port.PortName,
                    BaudRate = settings.Port.BaudRate,
                };

                port.DataReceived += RecieveMessage;

                return port;
            });
            this.settings = settings;
        }

        public bool IsConnected() 
        {
            return Port.IsOpen;
        }

        public static IEnumerable<string> GetAvailablePorts()
        {
            // Scan for available ports
            return SerialPort.GetPortNames();
        }

        public void SendMessage(string message)
        {
            if(!Port.IsOpen)
            {
                Port.Open();
            }
            Port.WriteLine($"{message}#");
        }

        private void RecieveMessage(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string s;
            s = sp.ReadLine();

            ProcessMessage?.Invoke(s);
        }
    }

}
