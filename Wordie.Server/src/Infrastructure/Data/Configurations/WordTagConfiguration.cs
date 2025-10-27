using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Infrastructure.Data.Configurations;

public class WordTagConfiguration : IEntityTypeConfiguration<WordTag>
{
    public void Configure(EntityTypeBuilder<WordTag> builder)
    {
        builder.HasKey(x => new { x.WordCardId, x.TagId });

        builder.HasOne(wt => wt.WordCard)
            .WithMany(w => w.WordTags)
            .HasForeignKey(wt => wt.WordCardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wt => wt.Tag)
            .WithMany(t => t.WordTags)
            .HasForeignKey(wt => wt.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
