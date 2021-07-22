using EntityFramework.Exceptions.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Infrastructure.Persistence;
using System;

namespace ScanApp.Tests.UnitTests
{
    public abstract class SqlLiteInMemoryDbFixture : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _dbContext;
        private readonly ServiceProvider _provider;

        public ApplicationDbContext NewDbContext => _provider.GetService<ApplicationDbContext>();

        public SqlLiteInMemoryDbFixture()
        {
            // UseExceptionProcessor() - replace standard EF Core exception with more detailed ones. (EntityFramework.Exceptions package for SqlLite)
            _provider = new ServiceCollection()
                .AddDbContext<ApplicationDbContext>(o =>
                    o.UseSqlite(_connection).UseExceptionProcessor(),
                    contextLifetime: ServiceLifetime.Transient,
                    optionsLifetime: ServiceLifetime.Singleton)
                .BuildServiceProvider();

            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            _dbContext = NewDbContext;
            _dbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _connection.Close();
        }
    }
}