using System;
using Microsoft.Extensions.Logging;

namespace Muffle.Button
{
    public class MuteButtonFactory
    {
        private readonly ILogger _logger;

        public MuteButtonFactory(ILogger logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(_logger));
        }
        public MuteButton Create(ArduinoSettings settings)
        {
            if (settings == null || string.IsNullOrEmpty(settings.PortName) || settings.BaudRate == default)
            {
                _logger.LogInformation("Creating null mute button.");
                return new NullMuteButton();
            }

            _logger.LogInformation("Creating arduino button.");
            return new ArduinoButton(settings, _logger);
        }
    }
}
