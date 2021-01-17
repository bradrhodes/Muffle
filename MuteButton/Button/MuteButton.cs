namespace MuteButton.Button
{
    public abstract class MuteButton
    {
        public abstract ArduinoResult SetMuteStateTrue();

        public abstract ArduinoResult SetMuteStateFalse();

        public abstract bool IsConnected();

        public event ProcessMessage ProcessMessage;

        protected virtual void OnProcessMessage(string message)
        {
            ProcessMessage?.Invoke(message);
        }
    }
}