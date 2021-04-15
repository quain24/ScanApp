using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ScanApp.Infrastructure.Persistence;
using System;

namespace ScanApp.Tests.UnitTests.Application
{
    public class SqlLiteInMemoryDbFixture : IDisposable
    {
        private const string InMemoryConnectionString = "DataSource=:memory:";
        private static SqliteConnection _connection;

        public readonly ApplicationDbContext DbContext;
        public ApplicationDbContext NewDbContext => new ApplicationDbContext(Options());

        public SqlLiteInMemoryDbFixture()
        {
            _connection = new SqliteConnection(InMemoryConnectionString);
            _connection.Open();
            DbContext = new ApplicationDbContext(Options());
            DbContext.Database.EnsureCreated();
        }

        private static DbContextOptions<ApplicationDbContext> Options()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            _connection.Close();
        }
    }
}