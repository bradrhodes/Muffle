namespace MuteButton.Button
{
    public class ButtonFactory
    {
        public Button Create(ArduinoSettings settings)
        {
            if (settings == null || string.IsNullOrEmpty(settings.Port.PortName) || settings.Port.BaudRate == default)
                return new NullButton();

            return new ArduinoButton(settings);
        }
    }
}
