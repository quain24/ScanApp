using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace ScanApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(BuildConfig())
                .CreateLogger();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static IConfigurationRoot BuildConfig()
        {
            var pcWideAppsettingsLocation = $"{Environment.GetEnvironmentVariable("APPSETT_FILE_LOCATION")}\\";
            var currentEnvAppSettingsFileName = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIROMENT") ?? "Production"}.json";

            return new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(pcWideAppsettingsLocation + currentEnvAppSettingsFileName, optional: true)
                .AddJsonFile(currentEnvAppSettingsFileName, optional: true)
                .AddEnvironmentVariables()
                .AddUserSecrets(typeof(Program).Assembly, optional: true)
                .Build();
        }
    }
}