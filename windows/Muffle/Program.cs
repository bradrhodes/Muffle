using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Muffle
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Log.Logger = new LoggerConfiguration()
            //     .WriteTo.Debug()
            //     .WriteTo.Console()
            //     .MinimumLevel.Debug()
            //     .CreateLogger();



            var builder = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<Form1>();
                    services.AddSingleton<IBindLogData, LogDataBinder>();
                    // services.AddLogging(configure => configure.AddConsole());
                    //
                    //
                    // var form1 = services.BuildServiceProvider()
                    //     .GetRequiredService<Form1>();
                    //
                    // var logger = new LoggerConfiguration()
                    //     .WriteTo.Debug()
                    //     .WriteTo.WinformsLoggingSink(form1.LogToListbox)
                    //     .CreateLogger();
                    //
                    // services.AddLogging(x =>
                    // {
                    //     x.AddSerilog(logger);
                    // });
                })
                .UseSerilog((context, services, loggerConfiguration) => 
                    loggerConfiguration
                        .WriteTo.Debug()
                        .WriteTo.WinformsLoggingSink(services.GetService<IBindLogData>())
                );

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                try
                {
                    var form = services.GetRequiredService<Form1>();
                    Application.Run(form);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error occurred.");
                }
            }

        }
    }

    public class LogDataBinder : IBindLogData
    {
        public BindingSource Source { get; }

        public LogDataBinder()
        {
            Source = new BindingSource();
            Source.ListChanged += SourceOnListChanged;
        }

        private void SourceOnListChanged(object sender, ListChangedEventArgs e)
        {
            if(Source.Count >= 25)
                Source.RemoveAt(0);
        }
    }

    public interface IBindLogData
    {
        BindingSource Source { get; }
    }
}

