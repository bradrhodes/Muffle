using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace MuteButton
{
    public class Arduino
    {
        private readonly Lazy<SerialPort> _lazyPort;
        private readonly AppSettings settings;

        private SerialPort Port => _lazyPort.Value;

        public Arduino(AppSettings settings)
        {
            _lazyPort = new Lazy<SerialPort>(() => new SerialPort
            {
                PortName = settings.Port.PortName,
                BaudRate = settings.Port.BaudRate
            });
            this.settings = settings;
        }

        public bool IsConnected() 
        {
            return Port.IsOpen;
        }

        public IEnumerable<string> GetAvailablePorts()
        {
            // Scan for available ports
            return SerialPort.GetPortNames();
        }
    }

}
