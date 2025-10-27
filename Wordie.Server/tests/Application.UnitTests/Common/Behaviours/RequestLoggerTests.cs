using Wordie.Server.Application.Common.Behaviours;
using Wordie.Server.Application.Common.Interfaces;
// Using a local dummy command instead of legacy Todo command
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace Wordie.Server.Application.UnitTests.Common.Behaviours;

public class RequestLoggerTests
{
    // Local dummy command to exercise the logging behaviour without relying on removed Todo types.
    public record DummyCommand : MediatR.IRequest;

    private Mock<ILogger<DummyCommand>> _logger = null!;
    private Mock<IUser> _user = null!;
    private Mock<IIdentityService> _identityService = null!;

    [SetUp]
    public void Setup()
    {
        _logger = new Mock<ILogger<DummyCommand>>();
        _user = new Mock<IUser>();
        _identityService = new Mock<IIdentityService>();
    }

    [Test]
    public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
    {
        _user.Setup(x => x.Id).Returns(Guid.NewGuid().ToString());

        var requestLogger = new LoggingBehaviour<DummyCommand>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new DummyCommand(), new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
    {
        var requestLogger = new LoggingBehaviour<DummyCommand>(_logger.Object, _user.Object, _identityService.Object);

        await requestLogger.Process(new DummyCommand(), new CancellationToken());

        _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Never);
    }
}
