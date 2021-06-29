using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScanApp.Infrastructure.Persistence;
using System;

namespace ScanApp.Tests.UnitTests.Application
{
    public abstract class SqlLiteInMemoryDbFixture : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private readonly SqliteConnection _connection;
        private readonly ApplicationDbContext _dbContext;
        private readonly ServiceProvider _provider;

        public ApplicationDbContext NewDbContext => new ApplicationDbContext(Options());

        public SqlLiteInMemoryDbFixture()
        {
            _provider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();

            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            _dbContext = NewDbContext;
            _dbContext.Database.EnsureCreated();
        }

        private DbContextOptions<ApplicationDbContext> Options()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .UseInternalServiceProvider(_provider)
                .Options;
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _connection.Close();
        }
    }
}