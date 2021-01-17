namespace Muffle.Button
{
    public class ButtonFactory
    {
        public PhysicalB Create(ArduinoSettings settings)
        {
            if (settings == null || string.IsNullOrEmpty(settings.Port.PortName) || settings.Port.BaudRate == default)
                return new NullPhysicalB();

            return new ArduinoButton(settings);
        }
    }
}
