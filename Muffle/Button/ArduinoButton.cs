using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace Muffle.Button
{
    public class ArduinoButton : PhysicalB
    {
        private readonly Lazy<SerialPort> _lazyPort;
        private readonly ArduinoSettings _settings;

        private SerialPort Port => _lazyPort.Value;


        public ArduinoButton(ArduinoSettings settings)
        {
            this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
            if (string.IsNullOrEmpty(settings.Port.PortName))
                throw new ArgumentNullException(nameof(settings.Port.PortName));
            if (settings.Port.BaudRate == default)
                throw new ArgumentNullException(nameof(settings.Port.BaudRate));

            _lazyPort = new Lazy<SerialPort>(() =>
            {
                var port = new SerialPort
                {
                    PortName = settings.Port.PortName,
                    BaudRate = settings.Port.BaudRate,
                };

                port.DataReceived += RecieveMessage;
                port.Open();

                return port;
            });
        }

        public override bool IsConnected() 
        {
            return Port.IsOpen;
        }

        public override ArduinoResult SetMuteStateTrue() =>
            ValidateConnection() switch
            {
                ArduinoResult.NotConnectedResult r => r,
                ArduinoResult.OkResult => SendMessage("mutestatetrue"),
                _ => throw new UnknownArduinoResultException()
            };

        public override ArduinoResult SetMuteStateFalse() => 
            ValidateConnection() switch
            {
                ArduinoResult.NotConnectedResult r => r,
                ArduinoResult.OkResult => SendMessage("mutestatefalse"),
                _ => throw new UnknownArduinoResultException()
            };

        private static IEnumerable<string> GetAvailablePorts()
        {
            // Scan for available ports
            return SerialPort.GetPortNames();
        }

        private ArduinoResult SendMessage(string message)
        {
            var connection = ValidateConnection();

            if (connection is ArduinoResult.NotConnectedResult)
                return connection;

            try
            {
                Port.WriteLine($"{message}#");
                return new ArduinoResult.OkResult();
            }
            catch (Exception ex)
            {
                return new ArduinoResult.SendFailureResult(ex.Message);
            }
        }

        private void RecieveMessage(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string s;
            s = sp.ReadLine();

            OnProcessMessage(s);
        }

        private ArduinoResult ValidateConnection()
        {
            if (!Port.IsOpen)
            {
                try
                {
                    Port.Open();
                }
                catch (Exception ex)
                {
                    return new ArduinoResult.NotConnectedResult($"Could not open port to arduino: {ex.Message}");
                }
            }

            return new ArduinoResult.OkResult();
        }
    }
}