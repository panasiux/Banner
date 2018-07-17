using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BannerWebApp
{
    public class Program
    {
        private static IConfigurationRoot config;
        public static void Main(string[] args)
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", true)
                .Build();
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.AddLog4Net();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .UseStartup<Startup>()
                .UseConfiguration(config)
                .Build();
    }
}
