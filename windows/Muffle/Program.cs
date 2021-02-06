using System;
using System.Collections;
using System.Collections.Generic;
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

            var builder = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<Form1>();
                    services.AddSingleton<IBindLogData, LogDataBinder>();
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
                    Console.WriteLine($"Error occurred: {ex.Message}");
                }
            }

        }
    }
}

