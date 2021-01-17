namespace MuteButton.Button
{
    public class NullButton : Button
    {
        public override ArduinoResult SetMuteStateTrue()
        {
            return new ArduinoResult.NotConnectedResult("Button is not currently set up.");
        }

        public override ArduinoResult SetMuteStateFalse()
        {
            return new ArduinoResult.NotConnectedResult("Button is not currently set up.");
        }

        public override bool IsConnected() => false;
    }
}