using System.IO;
using System.Text.Json;

namespace Muffle
{
    public record ArduinoSettings
    {
        public Port Port { get; init; }

        public ArduinoSettings(string port) => (Port) = Port;
    }

    public record Port
    {
        public string PortName { get; init; }
        public int BaudRate { get; init; }

        public Port(string portName, int baudRate) => (PortName, BaudRate) = (portName, baudRate);
    }

    public class Settings
    {
        private Settings(ArduinoSettings appSettings)
        {
            ArduinoSettings = appSettings;
        }

        public ArduinoSettings ArduinoSettings { get; private set; }

        public void UpdatePort(string portName)
        {
            var newSettings = ArduinoSettings with { Port = ArduinoSettings.Port with { PortName = portName } };
            ArduinoSettings = newSettings;
            Save();
        }

        public static Settings Initialize()
        {
            var appSettings = Load();
            return new Settings(appSettings);
        }

        private static ArduinoSettings Load()
        {
            if(File.Exists("settings.json"))
            {
                var jsonString = File.ReadAllText("settings.json");
                return JsonSerializer.Deserialize<ArduinoSettings>(jsonString);
            }

            return null;
        }

        private void Save()
        {
            var jsonString = JsonSerializer.Serialize(ArduinoSettings);
            File.WriteAllText("settings.json", jsonString);
        }
    }
}