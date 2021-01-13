using System.IO;
using System.Text.Json;

namespace MuteButton
{
    public record AppSettings
    {
        public Port Port { get; }

        public AppSettings(string port) => (Port) = Port;
    }

    public record Port
    {
        public string PortName { get; }
        public int BaudRate { get; }

        public Port(string portName, int baudRate) => (PortName, BaudRate) = (portName, baudRate);
    }

    public static class Settings
    {
        public static AppSettings Load()
        {
            if(File.Exists("settings.json"))
            {
                var jsonString = File.ReadAllText("settings.json");
                return JsonSerializer.Deserialize<AppSettings>(jsonString);
            }

            return null;
        }

        public static void Save(this AppSettings settings)
        {
            var jsonString = JsonSerializer.Serialize(settings);
            File.WriteAllText("settings.json", jsonString);
        }
    }
}