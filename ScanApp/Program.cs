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
using System.Threading.Tasks;

namespace ScanApp
{
    public class Program
    {
        private static string CurrentEnvAppSettingsName { get; } = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json";

        public static async Task Main(string[] args)
        {
            // Default logger
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            BuildConfigPcWideCustomGlobals(builder);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .CreateLogger();

            try
            {
                Log.Information("Starting up...");
                var cmdArgs = ParseCommandLineArgs(args);

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
                .ConfigureAppConfiguration(BuildConfigPcWideCustomGlobals)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(CurrentEnvAppSettingsName, optional: true)
                .AddEnvironmentVariables()
                .AddUserSecrets(typeof(Program).Assembly, optional: true);
        }

        private static void BuildConfigPcWideCustomGlobals(IConfigurationBuilder builder)
        {
            var filePath = Environment.GetEnvironmentVariable("APPSETT_FILE_LOCATION") switch
            {
                { } loc when loc.EndsWith("\\") => loc + CurrentEnvAppSettingsName,
                { } loc => loc + "\\" + CurrentEnvAppSettingsName,
                _ => null
            };

            if (filePath is not null)
                builder.AddJsonFile(filePath, optional: true, reloadOnChange: true);
        }

        private static CmdLineParserArguments ParseCommandLineArgs(string[] args)
        {
            var parser = new CommandLineParser.CommandLineParser
            {
                IgnoreCase = true,
                AcceptEqualSignSyntaxForValueArguments = true
            };

            var arguments = new CmdLineParserArguments();
            parser.ExtractArgumentAttributes(arguments);
            try
            {
                parser.ParseCommandLine(args);
                return arguments;
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