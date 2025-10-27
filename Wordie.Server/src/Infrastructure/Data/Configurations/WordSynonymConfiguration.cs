using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Infrastructure.Data.Configurations;

public class WordSynonymConfiguration : IEntityTypeConfiguration<WordSynonym>
{
    public void Configure(EntityTypeBuilder<WordSynonym> builder)
    {
        builder.HasKey(x => new { x.WordCardId, x.SynonymCardId });

        builder.HasOne(x => x.WordCard)
            .WithMany()
            .HasForeignKey(x => x.WordCardId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SynonymCard)
            .WithMany()
            .HasForeignKey(x => x.SynonymCardId)
            .OnDelete(DeleteBehavior.Restrict);

        // Creator no longer a domain navigation. CreatorId is a string user id; mapping to ApplicationUser (if desired) should be handled in Infrastructure separately.
    }
}
