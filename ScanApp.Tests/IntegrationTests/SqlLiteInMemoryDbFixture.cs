using System;
using EntityFramework.Exceptions.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Persistence;
using ScanApp.Infrastructure.Services;

namespace ScanApp.Tests.IntegrationTests
{
    public abstract class SqlLiteInMemoryDbFixture : IDisposable
    {
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
            services
                .AddDbContext<ApplicationDbContext>(o =>
                        o.UseSqlite(_connection).UseExceptionProcessor(),
                    contextLifetime: ServiceLifetime.Transient,
                    optionsLifetime: ServiceLifetime.Singleton);

            services.AddDbContextFactory<ApplicationDbContext>(o =>
                    o.UseSqlite(_connection).UseExceptionProcessor(),
                ServiceLifetime.Transient);

            services.AddSingleton<IContextFactory, AppDbContextFactory>(srv =>
                new AppDbContextFactory(srv.GetRequiredService<IDbContextFactory<ApplicationDbContext>>()));
        }

        public void Dispose()
        {
            _dbContext?.Database.EnsureDeleted();
            _connection?.Close();
        }
    }
}