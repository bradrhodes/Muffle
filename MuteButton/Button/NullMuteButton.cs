namespace MuteButton.Button
{
    public class NullMuteButton : MuteButton
    {
        public override ArduinoResult SetMuteStateTrue()
        {
            return new ArduinoResult.NotConnectedResult("MuteButton is not currently set up.");
        }

        public override ArduinoResult SetMuteStateFalse()
        {
            return new ArduinoResult.NotConnectedResult("MuteButton is not currently set up.");
        }

        public override bool IsConnected() => false;
    }
}