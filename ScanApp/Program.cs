using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScanApp.Application.Common.Interfaces;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ScanApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            // Default logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .CreateLogger();

            try
            {
                Log.Information("Starting up...");
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetService<IInitialDataSeeder>();
                    await seeder.Initialize(false);
                }

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed, logging unhandled exceptions...");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(BuildConfig)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            var pcWideAppsettingsLocation = $"{Environment.GetEnvironmentVariable("APPSETT_FILE_LOCATION")}\\";
            var currentEnvAppSettingsFileName = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json";

            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(pcWideAppsettingsLocation + currentEnvAppSettingsFileName, optional: true)
                .AddJsonFile(currentEnvAppSettingsFileName, optional: true)
                .AddEnvironmentVariables()
                .AddUserSecrets(typeof(Program).Assembly, optional: true);
        }
    }
}