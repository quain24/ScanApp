using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ScanApp.Infrastructure.Persistence;

namespace ScanApp.Tests.UnitTests
{
    public abstract class SqlLiteInMemoryDbFixture : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private static SqliteConnection _connection;
        private readonly ApplicationDbContext _dbContext;

        public ApplicationDbContext NewDbContext => new ApplicationDbContext(Options());

        public SqlLiteInMemoryDbFixture()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            _dbContext = NewDbContext;
            _dbContext.Database.EnsureCreated();
        }

        private static DbContextOptions<ApplicationDbContext> Options()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _connection.Close();
        }
    }
}