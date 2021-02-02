namespace Muffle.Button
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

        public abstract ButtonStatus Status { get; }
    }

    public class ButtonStatus
    {
        public string ButtonType { get; }
        public string Port { get; }
        public string Baud { get; }
        public bool Connected { get; }

        public ButtonStatus(string buttonType, string port, string baud, bool connected)
        {
            ButtonType = buttonType;
            Port = port;
            Baud = baud;
            Connected = connected;
        }

        public ButtonStatus(string buttonType, string port, int baud, bool connected)
        {
            ButtonType = buttonType;
            Port = port;
            Baud = baud.ToString();
            Connected = connected;
        }
    }
}