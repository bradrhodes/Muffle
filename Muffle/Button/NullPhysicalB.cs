namespace Muffle.Button
{
    public class NullPhysicalB : PhysicalB
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