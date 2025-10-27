using System.Reflection;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Application.WordSets.Commands.CreateWordSet;
using Wordie.Server.Application.WordSets.Commands.DeleteWordSet;
using Wordie.Server.Application.WordSets.Queries.GetWordSets;
using Wordie.Server.Application.WordCards.Commands.CreateWordCard;
using Wordie.Server.Application.Tags.Commands.CreateTag;
using Wordie.Server.Application.WordTags.Commands.CreateWordTag;
using Wordie.Server.Application.WordSynonyms.Commands.CreateWordSynonym;
using Wordie.Server.Application.UserWordProgress.Commands.CreateUserWordProgress;
using Wordie.Server.Application.LearningSessions.Commands.CreateLearningSession;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Wordie.Server.Infrastructure.Data;
using MediatR;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Use in-memory DB for development console
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("DevConsoleDb"));

        // Register IApplicationDbContext -> ApplicationDbContext
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // Simple dev user + identity service (in-memory roles)
        services.AddSingleton<IUser, DevConsole.Services.DevUser>();
        services.AddSingleton<IIdentityService, DevConsole.Services.DevIdentityService>();

        // Register AutoMapper, Validators and MediatR from Application and Infrastructure assemblies
        var appAssembly = typeof(CreateWordSetCommand).Assembly;
        var infraAssembly = typeof(Wordie.Server.Infrastructure.Services.DomainEvents.WordCardCreatedEventHandler).Assembly;

    // Register AutoMapper manually to avoid extension ambiguity
    var mapperConfig = new AutoMapper.MapperConfiguration(cfg => cfg.AddMaps(appAssembly));
    var mapper = mapperConfig.CreateMapper();
    services.AddSingleton(mapper);
        services.AddValidatorsFromAssembly(appAssembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(appAssembly, infraAssembly);
            // Register pipeline behaviors similar to the main app
            cfg.AddOpenRequestPreProcessor(typeof(Wordie.Server.Application.Common.Behaviours.LoggingBehaviour<>));
            cfg.AddOpenBehavior(typeof(Wordie.Server.Application.Common.Behaviours.UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(Wordie.Server.Application.Common.Behaviours.AuthorizationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(Wordie.Server.Application.Common.Behaviours.ValidationBehaviour<,>));
            cfg.AddOpenBehavior(typeof(Wordie.Server.Application.Common.Behaviours.PerformanceBehaviour<,>));
        });
    });

using var host = builder.Build();

// Seed a dev admin user into the dev identity service
var idService = host.Services.GetRequiredService<IIdentityService>();
await idService.CreateUserAsync("devadmin", "password");
await idService.IsInRoleAsync("devadmin", "Admin");

using var scope = host.Services.CreateScope();
var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

Console.WriteLine("Wordie Dev Console — select an option");

var exit = false;
while (!exit)
{
    Console.WriteLine();
    Console.WriteLine("Development Console - select an action:");
    Console.WriteLine("1) Add EF migration (dotnet ef migrations add <Name>)");
    Console.WriteLine("2) Remove last migration (dotnet ef migrations remove)");
    Console.WriteLine("3) List EF migrations (dotnet ef migrations list)");
    Console.WriteLine("4) Update database (dotnet ef database update)");
    Console.WriteLine("5) Drop database (dotnet ef database drop --force)");
    Console.WriteLine("6) Reset database (run seeder with reset -> deletes DB then seeds)");
    Console.WriteLine("7) Seed only (run seeder without reset)");
    Console.WriteLine("8) Build solution (dotnet build)");
    Console.WriteLine("9) Restore solution (dotnet restore)");
    Console.WriteLine("10) Clean solution (dotnet clean)");
    Console.WriteLine("11) Run Web project (dotnet run --project src/Web)");
    Console.WriteLine("12) Run Web project (dotnet watch run --project src/Web)");
    Console.WriteLine("13) Run all tests (dotnet test)");
    Console.WriteLine("14) Run unit tests only (tests/Application.UnitTests)");
    Console.WriteLine("15) Run integration tests only (tests/Infrastructure.IntegrationTests)");
    Console.WriteLine("16) Run specific test by name (requires test name pattern)");
    Console.WriteLine("17) Format code (dotnet format)");
    Console.WriteLine("18) Show Web connection string (src/Web/appsettings.Development.json)");
    Console.WriteLine("0) Exit");
    Console.Write("Select: ");
    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
            {
                Console.Write("Migration name: ");
                var name = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(name)) { Console.WriteLine("Name required."); break; }
                await RunCommandAsync("dotnet", $"ef migrations add {name} --project src/Infrastructure --startup-project src/Web --context ApplicationDbContext");
                break;
            }
            case "2":
            {
                Console.WriteLine("Removing last migration (confirm y/N):");
                var c = Console.ReadLine();
                if (c?.ToLowerInvariant() == "y")
                {
                    await RunCommandAsync("dotnet", "ef migrations remove --project src/Infrastructure --startup-project src/Web --context ApplicationDbContext");
                }
                break;
            }
            case "3":
            {
                await RunCommandAsync("dotnet", "ef migrations list --project src/Infrastructure --startup-project src/Web --context ApplicationDbContext");
                break;
            }
            
            case "8":
            {
                await RunCommandAsync("dotnet", "build Wordie.Server.sln -c Debug");
                break;
            }
            case "9":
            {
                await RunCommandAsync("dotnet", "restore Wordie.Server.sln");
                break;
            }
            case "10":
            {
                await RunCommandAsync("dotnet", "clean Wordie.Server.sln -c Debug");
                break;
            }
            case "11":
            {
                Console.WriteLine("Running Web project (will block until stopped). Press Ctrl+C to stop.");
                await RunCommandInteractiveAsync("dotnet", "run --project src/Web");
                break;
            }
            case "12":
            {
                Console.WriteLine("Running Web project with dotnet watch (will block until stopped). Press Ctrl+C to stop.");
                await RunCommandInteractiveAsync("dotnet", "watch run --project src/Web");
                break;
            }
            case "13":
            {
                await RunCommandAsync("dotnet", "test Wordie.Server.sln");
                break;
            }
            case "14":
            {
                await RunCommandAsync("dotnet", "test tests/Application.UnitTests/Application.UnitTests.csproj");
                break;
            }
            case "15":
            {
                await RunCommandAsync("dotnet", "test tests/Infrastructure.IntegrationTests/Infrastructure.IntegrationTests.csproj");
                break;
            }
            case "16":
            {
                Console.Write("Enter test name pattern: ");
                var pattern = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(pattern)) { Console.WriteLine("Pattern required."); break; }
                await RunCommandAsync("dotnet", $"test Wordie.Server.sln --filter \"FullyQualifiedName~{pattern}\"");
                break;
            }
            case "17":
            {
                await RunCommandAsync("dotnet", "format");
                break;
            }
            case "18":
            {
                var cfg = new ConfigurationBuilder().AddJsonFile("src/Web/appsettings.Development.json", optional: true).Build();
                var conn = cfg.GetConnectionString("Wordie.ServerDb");
                Console.WriteLine($"ConnectionString (Development): {conn}");
                break;
            }
            
            case "0":
                exit = true; break;
            default:
                Console.WriteLine("Unknown option"); break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

Console.WriteLine("Goodbye");

async Task<int> RunCommandAsync(string fileName, string arguments)
{
    var psi = new ProcessStartInfo(fileName, arguments)
    {
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true,
        WorkingDirectory = Directory.GetCurrentDirectory()
    };

    using var proc = Process.Start(psi)!;
    if (proc == null) return -1;

    // stream output
    _ = Task.Run(async () =>
    {
        while (!proc.StandardOutput.EndOfStream)
        {
            var line = await proc.StandardOutput.ReadLineAsync();
            if (line != null) Console.WriteLine(line);
        }
    });

    _ = Task.Run(async () =>
    {
        while (!proc.StandardError.EndOfStream)
        {
            var line = await proc.StandardError.ReadLineAsync();
            if (line != null) Console.Error.WriteLine(line);
        }
    });

    await proc.WaitForExitAsync();
    return proc.ExitCode;
}

async Task<int> RunCommandInteractiveAsync(string fileName, string arguments)
{
    var psi = new ProcessStartInfo(fileName, arguments)
    {
        RedirectStandardOutput = false,
        RedirectStandardError = false,
        UseShellExecute = true,
        CreateNoWindow = false,
        WorkingDirectory = Directory.GetCurrentDirectory()
    };

    using var proc = Process.Start(psi)!;
    if (proc == null) return -1;
    await proc.WaitForExitAsync();
    return proc.ExitCode;
}
