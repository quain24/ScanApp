using EntityFramework.Exceptions.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Persistence;
using ScanApp.Infrastructure.Services;
using Serilog;
using Serilog.Events;
using System;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;
using Version = ScanApp.Domain.ValueObjects.Version;

namespace ScanApp.Tests.IntegrationTests
{
    public abstract class SqlLiteInMemoryDbFixture : IDisposable
    {
        /// <summary>
        /// Plays role of a logger sink if provided.
        /// </summary>
        protected ITestOutputHelper Output { get; init; }

        private ServiceCollection _serviceCollection;

        /// <summary>
        /// Service collection of this instance. It can be modified until first actual call for <see cref="Provider"/> or <see cref="NewDbContext"/> was made.
        /// </summary>
        protected ServiceCollection ServiceCollection
        {
            get
            {
                if (_serviceCollection is null)
                {
                    _serviceCollection = new ServiceCollection();
                    ConfigureServices(_serviceCollection);
                }
                return _serviceCollection;
            }
        }

        private ServiceProvider _provider;

        /// <summary>
        /// Gets the configured service provider.<para/>
        /// You <b>cannot</b> reconfigure services by using <see cref="ServiceCollection"/> after call for provider has been made.
        /// </summary>
        protected ServiceProvider Provider => _provider ??= ServiceCollection.BuildServiceProvider();

        /// <summary>
        /// Gets new instance of configured DbContext.
        /// You <b>cannot</b> reconfigure services by using <see cref="ServiceCollection"/> after call for NewDbContext has been made.
        /// </summary>
        protected ApplicationDbContext NewDbContext
        {
            get
            {
                if (_dbContext is null)
                    InitializeDatabase();
                return Provider.GetService<ApplicationDbContext>();
            }
        }

        private const string InMemoryConnectionString = "DataSource=:memory:";
        private SqliteConnection _connection;
        private ApplicationDbContext _dbContext;

        private void InitializeDatabase()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            _dbContext = Provider.GetRequiredService<ApplicationDbContext>();
            _dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Override when additional service registration is needed.<br/>
        /// If you want to keep pre-configured SqlLite database, call this base method in your override.<para/>
        /// Mind that <see cref="Provider"/> object is not available for use in this method - it is instantiated after this method has ran to completion.
        /// </summary>
        /// <param name="services">Internally provided instance of service collection for use throughout this <see cref="SqlLiteInMemoryDbFixture"/> instance life.</param>
        protected virtual void ConfigureServices(ServiceCollection services)
        {
            // UseExceptionProcessor() - replace standard EF Core exception with more detailed ones. (EntityFramework.Exceptions package for SqlLite)
            var sqlConfiguration = new Action<DbContextOptionsBuilder>(o =>
            {
                o.UseSqlite(_connection);
                o.UseExceptionProcessor();
                o.EnableDetailedErrors();
                o.EnableSensitiveDataLogging();
            });

            services.AddDbContext<ApplicationDbContext, AppDbContextStub>(sqlConfiguration,
                contextLifetime: ServiceLifetime.Transient,
                optionsLifetime: ServiceLifetime.Singleton);

            services.AddDbContextFactory<ApplicationDbContext, DbContextFactoryStub>(sqlConfiguration,
            ServiceLifetime.Transient);

            services.AddSingleton<IContextFactory, AppDbContextFactory>(srv =>
                new AppDbContextFactory(srv.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()));

            services.AddLogging(c => c.AddSerilog(new LoggerConfiguration()
                .WriteTo.TestOutput(Output ?? new TestOutputHelper())
                .Enrich.FromLogContext()
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Error)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
                .CreateLogger()));
        }

        public void Dispose()
        {
            _dbContext?.Database.EnsureDeleted();
            _connection?.Close();
        }
    }

    public class AppDbContextStub : ApplicationDbContext
    {
        public AppDbContextStub(DbContextOptions<AppDbContextStub> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            if (Database.IsSqlite())
            {
                var timestampProperties = builder.Model
                    .GetEntityTypes()
                    .SelectMany(t => t.GetProperties())
                    .Where(p => p.ClrType == typeof(Version)
                                && p.ValueGenerated == ValueGenerated.OnAddOrUpdate);

                foreach (var property in timestampProperties)
                {
                    property.SetValueConverter(new SqliteTimestampConverter());
                    property.SetDefaultValueSql("CURRENT_TIMESTAMP");
                }
            }
        }
    }

    internal class SqliteTimestampConverter : ValueConverter<Version, string>
    {
        public SqliteTimestampConverter() : base(
            v => (v == null || v == Version.Empty) ? null : v.Value,
            v => v == null ? Version.Empty : Version.Create(v))
        { }
    }

    internal class DbContextFactoryStub : IDbContextFactory<ApplicationDbContext>
    {
        private readonly DbContextOptions<AppDbContextStub> _options;

        public DbContextFactoryStub(DbContextOptions<AppDbContextStub> options)
        {
            _options = options;
        }

        public ApplicationDbContext CreateDbContext()
        {
            return new AppDbContextStub(_options);
        }
    }
}