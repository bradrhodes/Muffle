namespace Muffle.Button
{
    public class MuteButtonFactory
    {
        public MuteButton Create(ArduinoSettings settings)
        {
            if (settings == null || settings.Port == null || string.IsNullOrEmpty(settings.Port.PortName) || settings.Port.BaudRate == default)
                return new NullMuteButton();

            return new ArduinoButton(settings);
        }
    }
}
