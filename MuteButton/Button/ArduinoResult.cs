using System;

namespace MuteButton.Button
{
    public abstract class ArduinoResult
    {
        public class NotConnectedResult : ArduinoResult
        {
            public string Message { get; }

            public NotConnectedResult(string message)
            {
                Message = message ?? throw new ArgumentNullException(nameof(message));
            }
        }

        public class SendFailureResult : ArduinoResult
        {
            public string Message { get; }

            public SendFailureResult(string message)
            {
                Message = message;
            }
        }

        public class OkResult : ArduinoResult
        {
        }
    }
}