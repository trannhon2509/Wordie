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
        services.AddSingleton<Wordie.Server.Application.Common.Interfaces.IIdentityService, DevConsole.Services.DevIdentityService>();

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
var idService = host.Services.GetRequiredService<Wordie.Server.Application.Common.Interfaces.IIdentityService>();
await idService.CreateUserAsync("devadmin", "password");
await idService.IsInRoleAsync("devadmin", "Admin");

using var scope = host.Services.CreateScope();
var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

Console.WriteLine("Wordie Dev Console — select an option");

var exit = false;
while (!exit)
{
    Console.WriteLine();
    Console.WriteLine("1) Create WordSet");
    Console.WriteLine("2) List WordSets");
    Console.WriteLine("3) Delete WordSet");
    Console.WriteLine("4) Create WordCard (simple)");
    Console.WriteLine("5) Create Tag");
    Console.WriteLine("6) Create WordTag (link tag to card)");
    Console.WriteLine("7) Create WordSynonym");
    Console.WriteLine("8) Create UserWordProgress");
    Console.WriteLine("9) Create LearningSession");
    Console.WriteLine("0) Exit");
    Console.Write("Select: ");
    var choice = Console.ReadLine();

    try
    {
        switch (choice)
        {
            case "1":
            {
                Console.Write("Name: ");
                var name = Console.ReadLine() ?? string.Empty;
                var id = await mediator.Send(new CreateWordSetCommand { Name = name });
                Console.WriteLine($"Created WordSet with Id={id}");
                break;
            }
            case "2":
            {
                var sets = await mediator.Send(new GetWordSetsQuery());
                Console.WriteLine("WordSets:");
                foreach (var s in sets)
                    Console.WriteLine($"{s.Id}: {s.Name}");
                break;
            }
            case "3":
            {
                Console.Write("Id to delete: ");
                if (int.TryParse(Console.ReadLine(), out var id))
                {
                    await mediator.Send(new DeleteWordSetCommand(id));
                    Console.WriteLine("Deleted (if existed).");
                }
                break;
            }
            case "4":
            {
                Console.Write("WordSetId: ");
                var ws = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Term: "); var term = Console.ReadLine() ?? string.Empty;
                Console.Write("Definition: "); var def = Console.ReadLine() ?? string.Empty;
                var cardId = await mediator.Send(new CreateWordCardCommand { WordSetId = ws, Term = term, Definition = def });
                Console.WriteLine($"Created WordCard Id={cardId}");
                break;
            }
            case "5":
            {
                Console.Write("Tag name: "); var tag = Console.ReadLine() ?? string.Empty;
                var tagId = await mediator.Send(new CreateTagCommand { Name = tag });
                Console.WriteLine($"Created Tag Id={tagId}");
                break;
            }
            case "6":
            {
                Console.Write("WordCardId: "); var wc = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("TagId: "); var t = int.Parse(Console.ReadLine() ?? "0");
                await mediator.Send(new CreateWordTagCommand(wc, t));
                Console.WriteLine("Linked tag to card.");
                break;
            }
            case "7":
            {
                Console.Write("WordCardId: "); var wc = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("SynonymCardId: "); var synId = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("CreatorId: "); var creator = Console.ReadLine() ?? string.Empty;
                await mediator.Send(new CreateWordSynonymCommand(wc, synId, creator));
                Console.WriteLine($"Created synonym linking card {wc} <-> {synId}");
                break;
            }
            case "8":
            {
                Console.Write("UserId: "); var uid = Console.ReadLine() ?? string.Empty;
                Console.Write("WordCardId: "); var wc = int.Parse(Console.ReadLine() ?? "0");
                var pw = await mediator.Send(new CreateUserWordProgressCommand { UserId = uid, WordCardId = wc });
                Console.WriteLine($"Created UserWordProgress Id={pw}");
                break;
            }
            case "9":
            {
                Console.Write("UserId: "); var uid = Console.ReadLine() ?? string.Empty;
                var ls = await mediator.Send(new CreateLearningSessionCommand { UserId = uid });
                Console.WriteLine($"Created LearningSession Id={ls}");
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
