using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Muffle
{
    public class WinformsLoggingSink : ILogEventSink
    {
        private readonly LoggingPassthroughMethod _loggingPassthroughMethod;

        public WinformsLoggingSink(LoggingPassthroughMethod loggingPassthroughMethod)
        {
            _loggingPassthroughMethod = loggingPassthroughMethod;
        }
        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage();
            _loggingPassthroughMethod(message);
        }
    }

    public delegate void LoggingPassthroughMethod(string message);

    public static class LoggingSinkExtensions
    {
        public static LoggerConfiguration WinformsLoggingSink(this LoggerSinkConfiguration loggerConfiguration,
            LoggingPassthroughMethod passthrough,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new WinformsLoggingSink(passthrough));
        }
    }
}
