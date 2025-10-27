using Wordie.Server.Domain.Constants;
using Wordie.Server.Domain.Entities;
using Wordie.Server.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using Bogus;

namespace Wordie.Server.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app, bool force = false, bool? resetOverride = null)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync(resetOverride);
        await initialiser.SeedAsync(force);
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _role_manager;
    private readonly IConfiguration _configuration;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _role_manager = roleManager;
        _configuration = configuration;
    }

    public async Task InitialiseAsync(bool? resetOverride = null)
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            var configReset = _configuration?.GetValue<bool?>("Seed:ResetDatabase");
            var reset = resetOverride ?? configReset ?? true;
            if (reset)
            {
                await _context.Database.EnsureDeletedAsync();
            }

            await _context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync(bool force = false)
    {
        try
        {
            await TrySeedAsync(force);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync(bool force = false)
    {
        // read seed count from configuration (fallback to 100)
        var seedCount = _configuration?.GetValue<int?>("Seed:Count") ?? 100;

    // Use Bogus to generate richer fake data
    Randomizer.Seed = new Random(42);
    var faker = new Faker();
        // Ensure default roles (Administrator, Teacher, Student)
        var roles = new[] { Roles.Administrator, "Teacher", "Student" };

        foreach (var roleName in roles)
        {
            if (_role_manager.Roles.All(r => r.Name != roleName))
            {
                await _role_manager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Ensure default users for each role
        var userDefinitions = new[]
        {
            new { UserName = "administrator@localhost", Email = "administrator@localhost", Roles = new[] { Roles.Administrator } },
            new { UserName = "teacher@localhost", Email = "teacher@localhost", Roles = new[] { "Teacher" } },
            new { UserName = "student@localhost", Email = "student@localhost", Roles = new[] { "Student" } },
        };

        foreach (var u in userDefinitions)
        {
            if (_userManager.Users.All(x => x.UserName != u.UserName))
            {
                var appUser = new ApplicationUser { UserName = u.UserName, Email = u.Email, EmailConfirmed = true };
                await _userManager.CreateAsync(appUser, "Password1!");
                if (u.Roles?.Length > 0)
                {
                    await _userManager.AddToRolesAsync(appUser, u.Roles);
                }
            }
        }

        // Persist identity changes so we can reference created users
        await _context.SaveChangesAsync();

        // If domain data already exists, skip bulk seeding to keep idempotent
        if (!force && (_context.WordSets.Any() || _context.WordCards.Any() || _context.Tags.Any()))
        {
            return;
        }

        // Get user ids to associate seeded data (prefer student user for user-scoped data)
        var student = await _userManager.FindByEmailAsync("student@localhost");
        var admin = await _userManager.FindByEmailAsync("administrator@localhost");
        var studentId = student?.Id ?? string.Empty;
        var adminId = admin?.Id ?? string.Empty;

    // Seed rows per entity using configured seedCount

        // WordSets using Bogus
        var wordSets = Enumerable.Range(1, seedCount).Select(i => new WordSet
        {
            Name = faker.Company.CatchPhrase() + (i > seedCount ? $" {i}" : string.Empty),
            Description = faker.Lorem.Sentence(6),
            IsSystem = false,
            CreatorId = adminId,
            CreatedAt = DateTime.UtcNow
        }).ToList();
        _context.WordSets.AddRange(wordSets);
        await _context.SaveChangesAsync();

        // Tags using Bogus
        var tags = Enumerable.Range(1, seedCount).Select(i => new Tag
        {
            Name = faker.Hacker.Noun() + (i > seedCount ? $"-{i}" : string.Empty),
            IsSystem = false,
            CreatorId = adminId,
            CreatedAt = DateTime.UtcNow
        }).ToList();
        _context.Tags.AddRange(tags);
        await _context.SaveChangesAsync();

        // WordCards using Bogus (assign to saved wordSets)
        var wordCards = new List<WordCard>(seedCount);
        var setsCount = wordSets.Count;
        for (int i = 1; i <= seedCount; i++)
        {
            var set = wordSets[(i - 1) % setsCount];
            var term = faker.Lorem.Word() + (i % 10 == 0 ? $"-{i}" : string.Empty);
            wordCards.Add(new WordCard
            {
                WordSetId = set.Id,
                Term = term,
                Definition = faker.Lorem.Sentence(8),
                Example = faker.Lorem.Sentence(10),
                PartOfSpeech = faker.PickRandom(new[] { "n", "v", "adj", "adv" }),
                Pronunciation = null,
                Note = faker.Lorem.Sentence(6),
                CreatedAt = DateTime.UtcNow
            });
        }
        _context.WordCards.AddRange(wordCards);
        await _context.SaveChangesAsync();

        // WordTags - link each word card to a tag (round-robin)
        var wordTags = new List<WordTag>(seedCount);
        var tagsCount = tags.Count;
        for (int i = 0; i < seedCount; i++)
        {
            var wc = wordCards[i % wordCards.Count];
            var tg = tags[i % tagsCount];
            wordTags.Add(new WordTag { WordCardId = wc.Id, TagId = tg.Id });
        }
        _context.WordTags.AddRange(wordTags);
        await _context.SaveChangesAsync();

        // WordSynonyms - pair successive cards with some randomness
        var synonyms = new List<WordSynonym>(seedCount);
        for (int i = 0; i < seedCount; i++)
        {
            var a = wordCards[i % wordCards.Count];
            var b = wordCards[(i + faker.Random.Int(1, 5)) % wordCards.Count];
            synonyms.Add(new WordSynonym { WordCardId = a.Id, SynonymCardId = b.Id, CreatorId = adminId, CreatedAt = DateTime.UtcNow });
        }
        _context.WordSynonyms.AddRange(synonyms);
        await _context.SaveChangesAsync();

        // UserWordProgress - assign to student user across distinct word cards (no duplicate keys) using Bogus
        var progresses = new List<UserWordProgress>(seedCount);
        for (int i = 0; i < seedCount; i++)
        {
            var wc = wordCards[i % wordCards.Count];
            progresses.Add(new UserWordProgress
            {
                UserId = studentId,
                WordCardId = wc.Id,
                Level = faker.Random.Int(0, 5),
                LastReviewedAt = DateTime.UtcNow.AddDays(-faker.Random.Int(0, 30)),
                NextReviewAt = DateTime.UtcNow.AddDays(faker.Random.Int(1, 30)),
                CorrectCount = faker.Random.Int(0, 20),
                IncorrectCount = faker.Random.Int(0, 10)
            });
        }
        _context.UserWordProgresses.AddRange(progresses);
        await _context.SaveChangesAsync();

        // LearningSessions - create sessions for student using Bogus
        var sessions = new List<LearningSession>(seedCount);
        for (int i = 0; i < seedCount; i++)
        {
            var set = wordSets[faker.Random.Int(0, wordSets.Count - 1)];
            var started = DateTime.UtcNow.AddDays(-faker.Random.Int(0, 30));
            sessions.Add(new LearningSession
            {
                UserId = studentId,
                WordSetId = set.Id,
                StartedAt = started,
                EndedAt = started.AddMinutes(faker.Random.Int(5, 120)),
                WordsStudied = faker.Random.Int(1, 50),
                CorrectAnswers = faker.Random.Int(0, 50),
                IncorrectAnswers = faker.Random.Int(0, 50)
            });
        }
        _context.LearningSessions.AddRange(sessions);
        await _context.SaveChangesAsync();

        // RefreshTokens - create tokens for student using Bogus
        var refreshTokens = Enumerable.Range(1, seedCount).Select(i => new RefreshToken
        {
            TokenHash = faker.Random.AlphaNumeric(40),
            UserId = studentId,
            Created = DateTime.UtcNow.AddMinutes(-faker.Random.Int(0, 60)),
            Expires = DateTime.UtcNow.AddDays(faker.Random.Int(7, 90))
        }).ToList();
        _context.RefreshTokens.AddRange(refreshTokens);
        await _context.SaveChangesAsync();
    }
}
