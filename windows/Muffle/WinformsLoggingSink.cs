using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;

namespace Muffle
{
    public class WinformsLoggingSink : ILogEventSink
    {
        private readonly IBindLogData _logDataBinder;

        public WinformsLoggingSink(IBindLogData logDataBinder)
        {
            _logDataBinder = logDataBinder;
        }
        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage();
            _logDataBinder.Source.Add(message);
        }
    }

    public static class LoggingSinkExtensions
    {
        public static LoggerConfiguration WinformsLoggingSink(this LoggerSinkConfiguration loggerConfiguration,
            IBindLogData logDataBinder,
            IFormatProvider formatProvider = null)
        {
            return loggerConfiguration.Sink(new WinformsLoggingSink(logDataBinder));
        }
    }
}
