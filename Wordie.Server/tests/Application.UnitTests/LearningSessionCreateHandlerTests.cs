using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Wordie.Server.Infrastructure.Data;
using Wordie.Server.Application.LearningSessions.Commands.CreateLearningSession;

namespace Wordie.Server.Application.UnitTests.LearningSessions;

public class LearningSessionCreateHandlerTests
{
    [Test]
    public async Task Handle_ShouldCreateLearningSession()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "ls_create_db")
            .Options;

        await using var context = new ApplicationDbContext(options);

        var handler = new CreateLearningSessionCommandHandler(context);

        var cmd = new CreateLearningSessionCommand
        {
            UserId = "user-2",
            WordSetId = 1
        };

        var id = await handler.Handle(cmd, CancellationToken.None);

        var saved = await context.LearningSessions.FindAsync(id);

        Assert.IsNotNull(saved);
        Assert.That(saved!.UserId, Is.EqualTo("user-2"));
    }
}
