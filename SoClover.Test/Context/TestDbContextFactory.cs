using Microsoft.EntityFrameworkCore;
using SoClover.Server.Context;
using System;

namespace SoClover.Tests.Context
{
    public static class TestDbContextFactory
    {
        public static SoCloverDBContext Create()
        {
            var options = new DbContextOptionsBuilder<SoCloverDBContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SoCloverDBContext(options);
            context.Database.EnsureCreated();

            return context;
        }

        public static void Destroy(SoCloverDBContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}