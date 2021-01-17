namespace MuteButton.Button
{
    public class ButtonFactory
    {
        public MuteButton Create(ArduinoSettings settings)
        {
            if (settings == null || string.IsNullOrEmpty(settings.Port.PortName) || settings.Port.BaudRate == default)
                return new NullMuteButton();

            return new ArduinoButton(settings);
        }
    }
}
