using System.Reflection;
using Wordie.Server.Application.Common.Interfaces;
using Wordie.Server.Domain.Entities;
using Wordie.Server.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Wordie.Server.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<WordSet> WordSets => Set<WordSet>();
    public DbSet<WordCard> WordCards => Set<WordCard>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<WordTag> WordTags => Set<WordTag>();
    public DbSet<UserWordProgress> UserWordProgresses => Set<UserWordProgress>();
    public DbSet<LearningSession> LearningSessions => Set<LearningSession>();
    public DbSet<WordSynonym> WordSynonyms => Set<WordSynonym>();
    // Note: IdentityDbContext already exposes ApplicationUser/Users for identity.
    // We don't add a domain 'User' DbSet here to avoid conflicts with Identity.
    
    // Store refresh tokens issued to users
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
