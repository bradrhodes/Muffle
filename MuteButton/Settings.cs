using System.IO;
using System.Text.Json;

namespace MuteButton
{
    public record AppSettings
    {
        public Port Port { get; init; }

        public AppSettings(string port) => (Port) = Port;
    }

    public record Port
    {
        public string PortName { get; init; }
        public int BaudRate { get; init; }

        public Port(string portName, int baudRate) => (PortName, BaudRate) = (portName, baudRate);
    }

    public class Settings
    {
        private Settings(AppSettings appSettings)
        {
            AppSettings = appSettings;
        }

        public AppSettings AppSettings { get; private set; }

        public void UpdatePort(string portName)
        {
            var newSettings = AppSettings with { Port = AppSettings.Port with { PortName = portName } };
            AppSettings = newSettings;
            Save();
        }

        public static Settings Initialize()
        {
            var appSettings = Load();
            return new Settings(appSettings);
        }

        private static AppSettings Load()
        {
            if(File.Exists("settings.json"))
            {
                var jsonString = File.ReadAllText("settings.json");
                return JsonSerializer.Deserialize<AppSettings>(jsonString);
            }

            return null;
        }

        private void Save()
        {
            var jsonString = JsonSerializer.Serialize(AppSettings);
            File.WriteAllText("settings.json", jsonString);
        }
    }
}