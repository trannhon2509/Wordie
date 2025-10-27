using Wordie.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Wordie.Server.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<global::Wordie.Server.Domain.Entities.WordSet> WordSets { get; }
    DbSet<global::Wordie.Server.Domain.Entities.WordCard> WordCards { get; }
    DbSet<global::Wordie.Server.Domain.Entities.Tag> Tags { get; }
    DbSet<global::Wordie.Server.Domain.Entities.WordTag> WordTags { get; }
    DbSet<global::Wordie.Server.Domain.Entities.UserWordProgress> UserWordProgresses { get; }
    DbSet<global::Wordie.Server.Domain.Entities.LearningSession> LearningSessions { get; }
    DbSet<global::Wordie.Server.Domain.Entities.WordSynonym> WordSynonyms { get; }
    // Identity's ApplicationUser/Users are managed in the Infrastructure Identity layer.

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
