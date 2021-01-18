namespace Muffle.Button
{
    public class MuteButtonFactory
    {
        public MuteButton Create(ArduinoSettings settings)
        {
            if (settings == null || string.IsNullOrEmpty(settings.PortName) || settings.BaudRate == default)
                return new NullMuteButton();

            return new ArduinoButton(settings);
        }
    }
}
