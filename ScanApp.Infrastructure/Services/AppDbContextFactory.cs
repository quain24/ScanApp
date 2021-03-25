using System;
using Microsoft.EntityFrameworkCore;
using ScanApp.Application.Common.Interfaces;
using ScanApp.Infrastructure.Persistence;

namespace ScanApp.Infrastructure.Services
{
    public class AppDbContextFactory : IContextFactory
    {
        private readonly IDbContextFactory<ApplicationDbContext> _context;

        public AppDbContextFactory(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _context = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory), "No context factory has been injected!");
        }

        public IApplicationDbContext CreateDbContext()
        {
            return _context.CreateDbContext();
        }
    }
}