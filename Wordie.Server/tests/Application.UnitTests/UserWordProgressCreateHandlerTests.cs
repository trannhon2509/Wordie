using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Wordie.Server.Infrastructure.Data;
using Wordie.Server.Application.UserWordProgress.Commands.CreateUserWordProgress;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Application.UnitTests.UserWordProgress;

public class UserWordProgressCreateHandlerTests
{
    [Test]
    public async Task Handle_ShouldAddUserWordProgress()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "uwp_create_db")
            .Options;

        await using var context = new ApplicationDbContext(options);

        var handler = new CreateUserWordProgressCommandHandler(context);

        var cmd = new CreateUserWordProgressCommand
        {
            UserId = "user-1",
            WordCardId = 1,
            Level = 0,
            NextReviewAt = DateTime.UtcNow.AddDays(1)
        };

        await handler.Handle(cmd, CancellationToken.None);

        var saved = await context.UserWordProgresses.FirstOrDefaultAsync(u => u.UserId == "user-1" && u.WordCardId == 1);

        Assert.IsNotNull(saved);
        Assert.That(saved!.Level, Is.EqualTo(0));
    }
}
