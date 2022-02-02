using Backend_Blaupause.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace Schulungstracker.backend.Test
{
    class DatabaseContextMock
    {
        public static DatabaseContext getInstance()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new DatabaseContext(options);
            databaseContext.Database.EnsureCreated();

            return databaseContext;
        }

    }
}
