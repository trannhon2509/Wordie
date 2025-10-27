using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wordie.Server.Domain.Entities;

namespace Wordie.Server.Infrastructure.Data.Configurations;

public class UserWordProgressConfiguration : IEntityTypeConfiguration<UserWordProgress>
{
    public void Configure(EntityTypeBuilder<UserWordProgress> builder)
    {
        builder.HasKey(x => new { x.UserId, x.WordCardId });

        // User navigation removed from Domain. UserId is a string referencing Identity user id.
        // If you want to setup a relationship to ApplicationUser, do it in the Infrastructure layer mapping against the Identity user entity.

        builder.HasOne(x => x.WordCard)
            .WithMany(w => w.UserProgresses)
            .HasForeignKey(x => x.WordCardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
