using CommandLineParser.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Common;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Default logger
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .CreateLogger();

            // Command line parser
            var cmdArgs = new CmdLineParserArguments();
            CreateParser(cmdArgs, args);

            try
            {
                Log.Information("Starting up...");
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetService<IInitialDataSeeder>();
                    await seeder.Initialize(cmdArgs.force);
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

        private static CommandLineParser.CommandLineParser CreateParser(CmdLineParserArguments arguments, string[] args)
        {
            var parser = new CommandLineParser.CommandLineParser
            {
                IgnoreCase = true,
                AcceptEqualSignSyntaxForValueArguments = true
            };

            parser.ExtractArgumentAttributes(arguments);
            try
            {
                parser.ParseCommandLine(args);
                return parser;
            }
            catch (CommandLineException ex)
            {
                Log.Error("Could not parse command line arguments - {message}", ex.Message);
                parser.ShowUsage();
                throw;
            }
        }
    }
}