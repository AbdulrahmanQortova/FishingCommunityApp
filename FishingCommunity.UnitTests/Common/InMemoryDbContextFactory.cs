using FishingCommunity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FishingCommunity.UnitTests.Common;

public static class InMemoryDbContextFactory
{
    public static ApplicationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
            .Options;

        return new ApplicationDbContext(options);
    }
}